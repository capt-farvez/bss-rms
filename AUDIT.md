# BSS Restaurant Demo App - Architecture Audit

## Overview
The existing demo application is an Angular 18 restaurant management system built with NgZorro Ant Design components. It uses Tailwind CSS for styling and connects to a live REST API at https://restaurantapi.bssoln.com.

## Application Routes

| Route | Component | Description |
|-------|-----------|-------------|
| `/` (root) | LoginComponent | Login page with authentication |
| `/dashboard` | DashboardComponent | Main layout with sidebar navigation |
| `/dashboard/employees` | EmployeesComponent | Employee management (CRUD) |
| `/dashboard/tables` | TablesComponent | Restaurant table management |
| `/dashboard/foods` | FoodsComponent | Food menu management |
| `/dashboard/new-order` | NewOrderComponent | Create new orders |
| `/dashboard/orders` | OrdersComponent | View and manage existing orders |

## Data Models

### 1. Employee
- **Core fields**: id, designation, joinDate, amountSold
- **User info**: userName, email, fullName, phoneNumber, firstName, middleName, lastName
- **Additional**: fatherName, motherName, spouseName, dob, address, nid, image, genderId

### 2. Table
- **Core fields**: id, tableNumber, numberOfSeats, isOccupied, image
- **Relations**: employees[] (assigned staff)

### 3. Food Item
- **Core fields**: id, name, description, price
- **Pricing**: discountType, discount, discountPrice
- **Visual**: image

### 4. Order
- **Core fields**: id, orderNumber, amount, orderStatus, orderTime
- **Relations**: table, orderedBy (customer), orderTakenBy (employee), orderItems[]

### 5. Order Item
- **Core fields**: id, quantity, unitPrice, totalPrice
- **Relations**: food, foodPackage

## Services Architecture

### Backend Services
- `EmployeeBackendService` - Employee CRUD operations
- `TableBackendService` - Table management
- `FoodBackendService` - Food menu management
- `NewOrderBackendService` - Order creation
- `OrdersBackendService` - Order retrieval and updates

### Core Services
- `AuthService` - Authentication and login state management
- `MessageService` - Global messaging/notifications
- `AuthInterceptorService` - HTTP interceptor for auth tokens

## Assets to Copy

### Images & Icons
- `/public/background.jpg` - Background image
- `/public/chef_green.png` - Logo/branding image
- `/public/favicon.ico` - Application favicon
- `/public/select_table.svg` - Table selection icon

### Login Assets
- `/public/login_assets/food_image.png`
- `/public/login_assets/foods.png`
- `/public/login_assets/login_form_image.png`
- `/public/login_assets/login_illustration_1.svg`
- `/public/login_assets/login_illustration_2.svg`
- `/public/login_assets/login_illustration_3.svg`
- `/public/login_assets/login_illustration_4.svg`
- `/public/login_assets/wood_table_transparent.png`
- `/public/login_assets/table.jpg`

### Fonts
- `/public/Overpass-Regular.woff`
- `/public/Overpass-Regular.woff2`

## Global Styles & CSS Variables

### CSS Variables (from styles.css)
```css
--bg-primary-400: #66bb6a;
--bg-primary-600: #43a047;
--bg-primary-700: #388e3c;
--bg-secondary-light: #E8ECE7;
--bg-secondary-light-100: #C8E6C9;
--bg-ternary-light: #F1FAF5;
--text-primary: white;
--text-dark-primary: #424242;
--text-dark-secondary: #616161;
--bg-warning-50: #ffebee;
--bg-warning-100: #ffcdd2;
--bg-warning-200: #ef9a9a;
--bg-warning-300: #e57373;
--bg-warning-400: #ef5350;
--bg-warning-600: #e53935;
--bg-warning-700: #ff1744;
```

### Key Style Classes
- `.button-primary` - Primary action buttons
- `.button-secondary` - Secondary buttons
- `.drop-down-button` - Dropdown menu buttons
- `.edit-button` - Edit action buttons
- `.data-table` - Table styling
- `.ant-form-*` - Form controls from NgZorro

## Dependencies

### UI Libraries
- **ng-zorro-antd** (v18.2.1) - Primary UI component library
- **tailwindcss** (v3.4.15) - Utility-first CSS framework
- **flowbite** (v2.5.2) - Tailwind components
- **primeng** (v17.18.12) - Additional UI components
- **@angular/material** (v18.0.0) - Material design components

### Core Dependencies
- **@angular/core** (v18.0.0) - Angular framework
- **@angular/forms** (v18.0.0) - Reactive forms
- **@angular/router** (v18.0.0) - Routing
- **rxjs** (v7.8.0) - Reactive programming

## API Configuration
- **Base URL**: https://restaurantapi.bssoln.com
- **Authentication**: Token-based via HTTP interceptor
- **Endpoints**:
  - `/api/Employee/datatable/` - Employee list
  - `/api/Table/datatable/` - Table list
  - `/api/Food/datatable/` - Food list
  - `/api/Order/datatable/` - Order list
  - `/api/auth/login` - Login endpoint

## Key Features to Preserve

1. **Lazy Loading**: All dashboard routes use lazy loading
2. **Signals**: State management using Angular signals
3. **Reactive Forms**: Form handling with validation
4. **Real-time validation**: Async server-side validation on forms
5. **Pagination**: Server-side pagination for all lists
6. **Image Upload**: Base64 image upload for employees, tables, foods
7. **Modal Forms**: Add/Edit operations use modal overlays
8. **Responsive Design**: Mobile-friendly layouts

## Migration Notes

### From Demo to New Structure Mapping
- `/dashboard/employees/*` → `/features/employees/`
- `/dashboard/tables/*` → `/features/tables/`
- `/dashboard/foods/*` → `/features/foods/`
- `/dashboard/orders/*` → `/features/orders/`
- `/dashboard/new-order/*` → `/features/orders/new-order/`
- `/login/*` → `/features/auth/`
- Backend services → `/core/services/api/`
- Interfaces → `/shared/models/`
- Layout components → `/ui/layouts/`

## Build Configuration
- Uses standalone components pattern
- Bootstrap via `bootstrapApplication` in main.ts
- Tailwind CSS configured with PostCSS
- Angular CLI v18 with esbuild