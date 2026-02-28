import { Injectable, signal, inject } from '@angular/core';
import { HttpClient, HttpEventType, HttpParams } from '@angular/common/http';
import { NzMessageService } from 'ng-zorro-antd/message';
import { API_BASE_URL } from '../../app.config';

export interface SalesRevenueStats {
  todaysSales: number;
  monthlySales: number;
  yearlySales: number;
  totalSales: number;
  todaysSalesAmount: number;
  monthlySalesAmount: number;
  yearlySalesAmount: number;
  totalSalesAmount: number;
  todaysExpenses: number;
  monthlyExpenses: number;
  yearlyExpenses: number;
  totalExpenses: number;
  todaysRevenue: number;
  monthlyRevenue: number;
  yearlyRevenue: number;
  totalRevenue: number;
}

export interface DashboardStats {
  totalOrders: number;
  totalRevenue: number;
  totalEmployees: number;
  totalFoods: number;
  todaysOrders: number;
  todaysRevenue: number;
  totalTables: number;
  occupiedTables: number;
  salesRevenue: SalesRevenueStats;
  recentOrders: RecentOrder[];
  topSellingFoods: TopSellingFood[];
}

export interface RecentOrder {
  id: string;
  orderNumber: string;
  amount: number;
  orderStatus: string;
  orderTime: string;
  tableNumber: string;
}

export interface TopSellingFood {
  id: number;
  name: string;
  price: number;
  image: string;
  totalQuantitySold: number;
  totalRevenue: number;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private baseUrl = inject(API_BASE_URL);
  private http = inject(HttpClient);
  private message = inject(NzMessageService);

  dashboardStats = signal<DashboardStats | null>(null);
  isSendingRequest = signal(false);

  getStats(month?: number, year?: number) {
    let params = new HttpParams();
    if (month) params = params.append('Month', month.toString());
    if (year) params = params.append('Year', year.toString());

    this.http.get<DashboardStats>(`${this.baseUrl}/api/Dashboard/stats`, {
      params,
      observe: 'events'
    }).subscribe({
      next: (data) => {
        switch (data.type) {
          case HttpEventType.Sent:
            this.isSendingRequest.set(true);
            break;
          case HttpEventType.Response:
            if (data.status === 200) {
              this.dashboardStats.set(data.body);
              this.isSendingRequest.set(false);
            }
            break;
        }
      },
      error: (err) => {
        this.message.error('Failed to load dashboard stats.');
        this.isSendingRequest.set(false);
      },
      complete: () => {
        this.isSendingRequest.set(false);
      }
    });
  }

  getFoodImage(image: string): string {
    return `${this.baseUrl}/images/food/${image}`;
  }
}
