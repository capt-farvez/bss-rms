import { ApplicationConfig, provideZoneChangeDetection, importProvidersFrom, InjectionToken, isDevMode } from '@angular/core';
import { provideRouter, withComponentInputBinding, withRouterConfig } from '@angular/router';
import { provideHttpClient, withInterceptorsFromDi, HTTP_INTERCEPTORS } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { FormsModule } from '@angular/forms';
import { AuthInterceptor } from './core/interceptors/auth.interceptor';

// Import API configuration from JSON files
import appSettingsProd from '../configs/appsettings.json';
import appSettingsDev from '../configs/appsettings.development.json';

// API Configuration
export const API_BASE_URL = new InjectionToken<string>('API_BASE_URL');
const apiBaseUrl = isDevMode() ? appSettingsDev.baseUrl : appSettingsProd.baseUrl;

// NgZorro imports
import { en_US, provideNzI18n } from 'ng-zorro-antd/i18n';
import { registerLocaleData } from '@angular/common';
import en from '@angular/common/locales/en';
import { provideNzIcons } from 'ng-zorro-antd/icon';

// Icons
import { IconDefinition } from '@ant-design/icons-angular';
import {
  AccountBookFill,
  AlertFill,
  AlertOutline,
  UserOutline,
  TeamOutline,
  MenuOutline,
  AndroidOutline,
  MenuFoldOutline,
  MenuUnfoldOutline,
  UsergroupAddOutline,
  IdcardOutline,
  GroupOutline,
  OrderedListOutline,
  PlusSquareOutline,
  PieChartOutline,
  PoweroffOutline,
  UserAddOutline,
  LoadingOutline,
  PlusOutline,
  PictureTwoTone,
  WarningOutline,
  PlusCircleOutline,
  CloseCircleOutline,
  PlusCircleTwoTone,
  StarTwoTone,
  CloseOutline,
  ShoppingCartOutline,
  StopOutline,
  EditOutline,
  LogoutOutline,
  EyeOutline,
  EyeInvisibleOutline,
  DashboardOutline,
  DollarOutline,
  CoffeeOutline,
  CalendarOutline,
  CheckCircleOutline,
  InboxOutline,
} from '@ant-design/icons-angular/icons';

import { routes } from './app.routes';

registerLocaleData(en);

const icons: IconDefinition[] = [
  AccountBookFill,
  AlertOutline,
  AlertFill,
  UserOutline,
  TeamOutline,
  MenuOutline,
  AndroidOutline,
  MenuFoldOutline,
  MenuUnfoldOutline,
  UsergroupAddOutline,
  IdcardOutline,
  GroupOutline,
  OrderedListOutline,
  PlusSquareOutline,
  PieChartOutline,
  PoweroffOutline,
  UserAddOutline,
  LoadingOutline,
  PlusOutline,
  PictureTwoTone,
  WarningOutline,
  PlusCircleOutline,
  CloseCircleOutline,
  PlusCircleTwoTone,
  StarTwoTone,
  CloseOutline,
  ShoppingCartOutline,
  StopOutline,
  EditOutline,
  LogoutOutline,
  EyeOutline,
  EyeInvisibleOutline,
  DashboardOutline,
  DollarOutline,
  CoffeeOutline,
  CalendarOutline,
  CheckCircleOutline,
  InboxOutline,
];

export const appConfig: ApplicationConfig = {
  providers: [
    provideNzIcons(icons),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding(), withRouterConfig({
      paramsInheritanceStrategy: 'always',
    })),
    provideHttpClient(withInterceptorsFromDi()),
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: API_BASE_URL, useValue: apiBaseUrl },
    provideNzI18n(en_US),
    importProvidersFrom(FormsModule),
    provideAnimationsAsync(),
  ]
};
