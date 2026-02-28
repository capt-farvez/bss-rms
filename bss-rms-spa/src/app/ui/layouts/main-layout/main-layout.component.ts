import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, NavigationEnd, RouterOutlet, ActivatedRoute } from '@angular/router';
import { filter } from 'rxjs';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import {
  NzLayoutModule,
  NzLayoutComponent,
  NzSiderComponent,
  NzHeaderComponent,
  NzContentComponent,
  NzFooterComponent
} from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { AuthService } from '../../../core/services/auth.service';
import { ProfileComponent } from '../../profile/profile.component';
import { API_BASE_URL } from '../../../app.config';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    NzLayoutModule,
    NzMenuModule,
    NzIconModule,
    NzAvatarModule,
    NzDropDownModule,
    ProfileComponent
  ],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss'
})
export class MainLayoutComponent implements OnInit {
  protected authService = inject(AuthService);
  private baseUrl = inject(API_BASE_URL);
  private router = inject(Router);
  private activatedRoute = inject(ActivatedRoute);
  private responsive = inject(BreakpointObserver);

  sideBarItems = [
    {
      ItemName: 'Dashboard',
      IconName: 'dashboard',
    },
    {
      ItemName: 'Employees',
      IconName: 'idcard',
    },
    {
      ItemName: 'Tables',
      IconName: 'group',
    },
    {
      ItemName: 'Foods',
      IconName: 'pie-chart',
    },
    {
      ItemName: 'New Order',
      IconName: 'plus-square',
    },
    {
      ItemName: 'Orders',
      IconName: 'ordered-list',
    },
    {
      ItemName: 'Expenses',
      IconName: 'dollar',
    },
    {
      ItemName: 'Report & Analytics',
      IconName: 'area-chart',
    },
  ];

  currentComponent = signal('');
  isCollapsed = true;

  ngOnInit() {
    let childRoute = this.activatedRoute.firstChild;
    if (childRoute) {
      console.log('Activated Child Route:', childRoute.snapshot.routeConfig?.path);
      if (childRoute.snapshot.routeConfig?.path) {
        switch (childRoute.snapshot.routeConfig?.path){
          case 'home':
            this.currentComponent.set('Dashboard');
            break;
          case 'employees':
            this.currentComponent.set('Employees');
            break;
          case 'tables':
            this.currentComponent.set('Tables');
            break;
          case 'new-order':
            this.currentComponent.set('New Order');
            break;
          case 'foods':
            this.currentComponent.set('Foods');
            break;
          case 'orders':
            this.currentComponent.set('Orders');
            break;
          case 'expenses':
            this.currentComponent.set('Expenses');
            break;
          case 'reports':
            this.currentComponent.set('Report & Analytics');
            break;
        }
      }
    }
    else{
      this.currentComponent.set('Dashboard');
      this.router.navigate(['dashboard/home']);
    }

    // Fetch user profile if authenticated
    if (this.authService.isAuthenticated()){
      this.authService.getUserProfile().subscribe();
    }

    this.isCollapsed = true;
    this.responsive.observe([Breakpoints.Large, ])
      .subscribe(result => {
        this.isCollapsed = true;
        if (result.matches) {
          this.isCollapsed = false;
        }
      });
  }

  onLogout() {
    this.authService.logout();
  }

  setComponent(menuItemName: string) {
    this.currentComponent.set(menuItemName);
    switch (this.currentComponent()){
      case 'Dashboard':
        this.router.navigate(['dashboard/home']);
        break;
      case 'Employees':
        this.router.navigate(['dashboard/employees']);
        break;
      case 'Tables':
        this.router.navigate(['dashboard/tables']);
        break;
      case 'Foods':
        this.router.navigate(['dashboard/foods']);
        break;
      case 'New Order':
        this.router.navigate(['dashboard/new-order']);
        break;
      case 'Orders':
        this.router.navigate(['dashboard/orders']);
        break;
      case 'Expenses':
        this.router.navigate(['dashboard/expenses']);
        break;
      case 'Report & Analytics':
        this.router.navigate(['dashboard/reports']);
        break;
    }
  }

  showSider() {
    this.isCollapsed = false;
  }

  hideSider() {
    this.isCollapsed = true;
  }

  getUserImage(image: string) {
    if (!image) return '';
    return `${this.baseUrl}/images/user/${image}`;
  }
}
