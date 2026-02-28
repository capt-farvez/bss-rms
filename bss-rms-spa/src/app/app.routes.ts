import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./ui/layouts/main-layout/main-layout.component').then(m => m.MainLayoutComponent),
    canActivate: [authGuard],
    children: [
      {
        path: '',
        redirectTo: 'employees',
        pathMatch: 'full'
      },
      {
        path: 'employees',
        loadComponent: () => import('./features/employees/employees-list/employees-list.component').then(m => m.EmployeesListComponent)
      },
      {
        path: 'tables',
        loadComponent: () => import('./features/tables/tables-list/tables-list.component').then(m => m.TablesListComponent)
      },
      {
        path: 'foods',
        loadComponent: () => import('./features/foods/foods-list/foods-list.component').then(m => m.FoodsListComponent)
      },
      {
        path: 'new-order',
        loadComponent: () => import('./features/orders/new-order/new-order.component').then(m => m.NewOrderComponent)
      },
      {
        path: 'orders',
        loadComponent: () => import('./features/orders/orders-list/orders-list.component').then(m => m.OrdersListComponent)
      },
      {
        path: 'expenses',
        loadComponent: () => import('./features/expenses/expenses-list/expenses-list.component').then(m => m.ExpensesListComponent)
      },
      {
        path: 'reports',
        loadComponent: () => import('./features/reports/reports.component').then(m => m.ReportsComponent)
      }
    ]
  }
];
