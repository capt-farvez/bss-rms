import { Component, effect, inject, Injector, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { EmployeeService } from '../../../core/services/employee.service';
import { NzTableModule, NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzAvatarComponent } from 'ng-zorro-antd/avatar';
import { NzIconDirective } from 'ng-zorro-antd/icon';
import { NzModalService } from 'ng-zorro-antd/modal';
import { toObservable } from '@angular/core/rxjs-interop';
import { NzTooltipDirective } from 'ng-zorro-antd/tooltip';
import { AddEmployeeComponent } from '../add-employee/add-employee.component';
import { API_BASE_URL } from '../../../app.config';

export type ThemeType = "fill" | "outline" | "twotone"

@Component({
  selector: 'app-employees-list',
  standalone: true,
  providers: [NzModalService],
  imports: [
    NzTableModule,
    NzAvatarComponent,
    NzIconDirective,
    AddEmployeeComponent,
    NzTooltipDirective,
    DatePipe
  ],
  templateUrl: './employees-list.component.html',
  styleUrl: './employees-list.component.scss'
})
export class EmployeesListComponent implements OnInit {
  protected employeeService = inject(EmployeeService);
  listOfEmployees = this.employeeService.listOfEmployees;
  imageBaseUrl = inject(API_BASE_URL) + '/images/user/';
  private injector = inject(Injector);

  ngOnInit() {
    this.loadDataFromServer(this.pageIndex, this.pageSize);
  }

  pageSize = 10;
  pageIndex = 1;
  starMarked: ThemeType = 'twotone';

  constructor() {
    effect(() => {
      if (this.employeeService.triggerRefresh()) {
        this.ngOnInit();
        this.employeeService.triggerRefresh.set(false);
      }
    }, { allowSignalWrites: true });
  }

  onQueryParamsChange(params: NzTableQueryParams): void {
    this.pageIndex = params.pageIndex;
    this.pageSize = params.pageSize;
    this.loadDataFromServer(this.pageIndex, this.pageSize);
  }

  loadDataFromServer(
    pageIndex: number,
    pageSize: number,
  ): void {
    this.employeeService.getListOfEmployees('', pageIndex.toString(), pageSize.toString(), '');
  }

  editEmployee(id: string) {
    this.employeeService.getEmployeeById(id);
  }

  deleteUser(id: string) {
    this.employeeService.deleteEmployee(id);
    if (this.employeeService.listOfEmployees().length <= 1) {
      this.pageIndex = Math.max(1, this.pageIndex - 1);
    }
  }

  getImageUrl(imageUrl: string) {
    return this.imageBaseUrl + "/" + imageUrl;
  }

  toggleStar() {
    if (this.starMarked === 'fill') {
      this.starMarked = 'twotone';
    }
    else {
      this.starMarked = 'fill';
    }
  }
}