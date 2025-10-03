import { Injectable, inject, signal, effect } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { tap, catchError, switchMap, filter, take } from 'rxjs/operators';
import { User, LoginRequest, LoginResponse, AuthState } from '../../shared/models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  // API Base URL
  private readonly API_URL = 'https://restaurantapi.bssoln.com/api';

  // Token refresh management
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

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
  showProfile = signal(false);

  constructor() {
    // Check for stored auth data on initialization
    this.checkStoredAuth();

    // Effect to persist auth state
    effect(() => {
      const state = this.authState();
      if (state.isAuthenticated && state.token) {
        localStorage.setItem('authToken', state.token);
        localStorage.setItem('refreshToken', state.refreshToken || '');
        localStorage.setItem('loginStatus', 'valid');
        if (state.user) {
          localStorage.setItem('currentUser', JSON.stringify(state.user));
        }
      }
    }, { allowSignalWrites: true });
  }

  private checkStoredAuth() {
    const token = localStorage.getItem('authToken');
    const refreshToken = localStorage.getItem('refreshToken');
    const loginStatus = localStorage.getItem('loginStatus');
    const userStr = localStorage.getItem('currentUser');

    if (loginStatus === 'valid' && token) {
      let user: User | null = null;
      if (userStr) {
        try {
          user = JSON.parse(userStr) as User;
        } catch (error) {
          console.error('Failed to parse stored user:', error);
        }
      }

      this.authState.update(state => ({
        ...state,
        user,
        token,
        refreshToken,
        isAuthenticated: true
      }));

      // Fetch user profile if we don't have it
      if (!user) {
        this.getUserProfile().subscribe();
      }
    }
  }

  login(credentials: LoginRequest): Observable<any> {
    this.authState.update(state => ({ ...state, isLoading: true, error: null }));

    return this.http.post<any>(`${this.API_URL}/Auth/SignIn`, credentials).pipe(
      tap(response => {
        // Store auth data
        const token = response.token;
        const refreshToken = response.refreshToken;

        localStorage.setItem('authToken', token);
        localStorage.setItem('refreshToken', refreshToken);
        localStorage.setItem('loginStatus', 'valid');

        // Update state
        this.authState.update(state => ({
          ...state,
          token,
          refreshToken,
          isAuthenticated: true,
          isLoading: false,
          error: null
        }));

        // Fetch user profile after login
        this.getUserProfile().subscribe();
      }),
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'Login failed';

        if (error.status === 401) {
          errorMessage = 'Invalid username or password';
        } else if (error.status === 0) {
          errorMessage = 'Unable to connect to server';
        } else if (error.error?.message) {
          errorMessage = error.error.message;
        }

        this.authState.update(state => ({
          ...state,
          isLoading: false,
          error: errorMessage
        }));

        return throwError(() => new Error(errorMessage));
      })
    );
  }

  getUserProfile(): Observable<User> {
    return this.http.get<User>(`${this.API_URL}/Auth/profile`).pipe(
      tap(user => {
        this.authState.update(state => ({
          ...state,
          user
        }));
        localStorage.setItem('currentUser', JSON.stringify(user));
      }),
      catchError(error => {
        console.error('Failed to fetch user profile:', error);
        return throwError(() => error);
      })
    );
  }

  refreshAccessToken(): Observable<any> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      this.logout();
      return throwError(() => new Error('No refresh token available'));
    }

    return this.http.post<any>(`${this.API_URL}/Auth/refreshToken`, { refreshToken }).pipe(
      tap(response => {
        const token = response.token || response.accessToken;
        const newRefreshToken = response.refreshToken;

        localStorage.setItem('authToken', token);
        localStorage.setItem('refreshToken', newRefreshToken);

        this.authState.update(state => ({
          ...state,
          token,
          refreshToken: newRefreshToken
        }));
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
    localStorage.removeItem('authToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('currentUser');
    localStorage.removeItem('loginStatus');
  }

  getToken(): string | null {
    return localStorage.getItem('authToken');
  }

  getRefreshToken(): string | null {
    return localStorage.getItem('refreshToken');
  }

  isAuthenticated(): boolean {
    return this.authState().isAuthenticated;
  }

  getCurrentUser(): User | null {
    return this.authState().user;
  }

  currentUserProfile() {
    const user = this.authState().user;
    return {
      id: user?.id || '',
      fullName: user?.fullName || '',
      email: user?.email || '',
      image: user?.image || '',
      userName: user?.userName || '',
      phoneNumber: user?.phoneNumber || ''
    };
  }
}