import { API_BASE_URL } from '../../app.config';
import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule,
    NzModalModule,
    NzButtonModule,
    NzAvatarModule,
    NzIconModule
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent {
  authService = inject(AuthService);
  private baseUrl = inject(API_BASE_URL);

  constructor() {
    // Debug: Log current user profile data
    console.log('Current User Profile:', this.authService.currentUserProfile());
    console.log('Current User (full):', this.authService.getCurrentUser());
  }

  handleOk() {
    this.authService.showProfile.set(false);
  }

  getUserImage(image: string) {
    if (!image) return '';
    return `${this.baseUrl}/images/user/${image}`;
  }
}