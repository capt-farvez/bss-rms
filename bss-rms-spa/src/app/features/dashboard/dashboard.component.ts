import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { DashboardService } from '../../core/services/dashboard.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    DatePipe,
    DecimalPipe,
    NzSpinModule,
    NzIconModule,
    NzTableModule,
    NzEmptyModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  protected dashboardService = inject(DashboardService);

  ngOnInit() {
    this.dashboardService.getStats();
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Pending': return '#f59e0b';
      case 'Confirmed': return '#3b82f6';
      case 'Preparing': return '#8b5cf6';
      case 'PreparedToServe': return '#06b6d4';
      case 'Served': return '#10b981';
      case 'Paid': return '#66bb6a';
      default: return '#6b7280';
    }
  }

  getStatusLabel(status: string): string {
    if (status === 'PreparedToServe') return 'Prepared To Serve';
    return status;
  }

  getFoodImage(image: string): string {
    return this.dashboardService.getFoodImage(image);
  }
}
