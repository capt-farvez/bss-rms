import { Component, computed, effect, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NzCarouselModule } from 'ng-zorro-antd/carousel';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NzCarouselModule,
    NzIconModule,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  protected chef_logo = '/chef_green.png';
  showPassword = signal(false);

  loginStatus = computed(() => {
    const authState = this.authService.currentUser();
    if (authState.isLoading) return 'loading';
    if (authState.error) return 'invalid';
    if (authState.isAuthenticated) return 'valid';
    return null;
  });

  loggingIn = computed(() => this.authService.currentUser().isLoading);

  loginForm = new FormGroup({
    email: new FormControl('admin@mail.com', [Validators.required]),
    password: new FormControl('Admin@123', [Validators.required]),
  });

  constructor() {
    effect(() => {
      if (this.loginStatus() === 'valid') {
        setTimeout(() => {
          this.router.navigateByUrl('/dashboard');
        }, 1000);
      }
    });
  }

  togglePasswordVisibility() {
    this.showPassword.update(value => !value);
  }

  onSubmit() {
    if (this.loginForm.valid) {
      const formData = {
        userName: this.loginForm.value.email!,
        password: this.loginForm.value.password!
      };

      this.authService.login(formData).subscribe({
        next: () => {
          // Navigation will be handled by the effect
        },
        error: (error) => {
          console.error('Login failed:', error);
        }
      });
    }
  }
}
