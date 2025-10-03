import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { User, LoginRequest, LoginResponse, AuthState } from '../../shared/models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  // Using signals for reactive state management
  private authState = signal<AuthState>({
    user: null,
    token: null,
    refreshToken: null,
    isAuthenticated: false,
    isLoading: false,
    error: null
  });

  // Public readonly signals
  currentUser = this.authState.asReadonly();

  constructor() {
    // Check for stored auth data on initialization
    this.checkStoredAuth();
  }

  private checkStoredAuth() {
    const token = localStorage.getItem('auth_token');
    const userStr = localStorage.getItem('current_user');

    if (token && userStr) {
      try {
        const user = JSON.parse(userStr) as User;
        this.authState.update(state => ({
          ...state,
          user,
          token,
          isAuthenticated: true
        }));
      } catch (error) {
        this.clearAuthData();
      }
    }
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    this.authState.update(state => ({ ...state, isLoading: true, error: null }));

    // For now, return mock data - will be replaced with actual API call
    const mockResponse: LoginResponse = {
      token: 'mock-jwt-token',
      refreshToken: 'mock-refresh-token',
      user: {
        id: '1',
        userName: credentials.userName,
        email: 'admin@bss-restaurant.com',
        fullName: 'Admin User',
        phoneNumber: '1234567890',
        firstName: 'Admin',
        lastName: 'User'
      },
      expiresAt: new Date(Date.now() + 3600000).toISOString() // 1 hour from now
    };

    return of(mockResponse).pipe(
      tap(response => {
        // Store auth data
        localStorage.setItem('auth_token', response.token);
        localStorage.setItem('refresh_token', response.refreshToken);
        localStorage.setItem('current_user', JSON.stringify(response.user));

        // Update state
        this.authState.update(state => ({
          ...state,
          user: response.user,
          token: response.token,
          refreshToken: response.refreshToken,
          isAuthenticated: true,
          isLoading: false
        }));
      }),
      catchError(error => {
        this.authState.update(state => ({
          ...state,
          isLoading: false,
          error: error.message || 'Login failed'
        }));
        throw error;
      })
    );
  }

  logout() {
    this.clearAuthData();
    this.authState.set({
      user: null,
      token: null,
      refreshToken: null,
      isAuthenticated: false,
      isLoading: false,
      error: null
    });
    this.router.navigate(['/login']);
  }

  private clearAuthData() {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('current_user');
  }

  getToken(): string | null {
    return this.authState().token;
  }

  isAuthenticated(): boolean {
    return this.authState().isAuthenticated;
  }

  getCurrentUser(): User | null {
    return this.authState().user;
  }
}