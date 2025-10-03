import { Component, Input, Output, EventEmitter, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NzHeaderComponent } from 'ng-zorro-antd/layout';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { MenuItem } from '../../layouts/main-layout/main-layout.component';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    CommonModule,
    NzHeaderComponent,
    NzIconModule,
    NzAvatarModule,
    NzDropDownModule,
    NzMenuModule
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  @Input() isCollapsed = false;
  @Input() currentRoute = '';
  @Input() menuItems: MenuItem[] = [];
  @Output() toggleSidebar = new EventEmitter<void>();
  @Output() logout = new EventEmitter<void>();

  private authService = inject(AuthService);

  // Get user from auth service
  currentUser = computed(() => {
    const user = this.authService.getCurrentUser();
    return {
      email: user?.email || 'Loading...',
      fullName: user?.fullName || 'User',
      image: user?.image || null
    };
  });

  getPageTitle(): string {
    const menuItem = this.menuItems.find(item => this.currentRoute.includes(item.route));
    if (!menuItem) return '';

    switch (menuItem.name) {
      case 'Employees':
        return 'Employee Management';
      case 'Tables':
        return 'Table Management';
      case 'Foods':
        return 'Food Item Management';
      case 'New Order':
        return 'Create New Order';
      case 'Orders':
        return 'Order Management';
      default:
        return menuItem.name;
    }
  }

  getUserImage(image: string | null): string {
    return image || '';
  }

  onToggleSidebar() {
    this.toggleSidebar.emit();
  }

  onLogout() {
    this.logout.emit();
  }

  showProfile() {
    // Will be implemented later
    console.log('Show profile');
  }
}
