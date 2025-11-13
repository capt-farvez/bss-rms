import { Component, effect, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableService } from '../../../core/services/table.service';
import { NzTableModule, NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzModalService } from 'ng-zorro-antd/modal';
import { AddTableComponent } from '../add-table/add-table.component';
import { AssignEmployeeToTableComponent } from '../assign-employee-to-table/assign-employee-to-table.component';
import { AvatarTooltipComponent } from '../avatar-tooltip/avatar-tooltip.component';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { API_BASE_URL } from '../../../app.config';

@Component({
  selector: 'app-tables-list',
  standalone: true,
  imports: [
    CommonModule,
    NzTableModule,
    NzAvatarModule,
    NzIconModule,
    NzToolTipModule,
    AddTableComponent,
    AssignEmployeeToTableComponent,
    AvatarTooltipComponent
  ],
  providers: [NzModalService],
  templateUrl: './tables-list.component.html',
  styleUrl: './tables-list.component.scss'
})
export class TablesListComponent implements OnInit {
  protected tableService = inject(TableService);
  modal = inject(NzModalService);
  listOfTables = this.tableService.listOfTables;
  listOfEmployees = this.tableService.listOfEmployees;
  imageBaseUrl = inject(API_BASE_URL) + '/images/table/';
  responsive = inject(BreakpointObserver);
  tableWidthConfig = ['120px', '140px', '110px', '160px', 'auto', '180px'];

  pageSize = 10;
  pageIndex = 1;

  constructor() {
    this.tableService.isSendingRequest.set(true);
    effect(() => {
      if (this.tableService.triggerRefresh()) {
        this.ngOnInit();
        this.tableService.triggerRefresh.set(false);
      }
      if (!this.tableService.isLoadingTables() && !this.tableService.isLoadingEmployees()) {
        this.tableService.isSendingRequest.set(false);
      }
    }, { allowSignalWrites: true });
  }

  ngOnInit() {
    this.loadDataFromServer(this.pageIndex, this.pageSize);

    this.responsive.observe([Breakpoints.Large, Breakpoints.XLarge])
      .subscribe(result => {
        this.tableWidthConfig = ['120px', '100px', '70px', '110px', 'auto', '180px'];
        if (result.matches) {
          this.tableWidthConfig = ['120px', '160px', '130px', '180px', 'auto', '180px'];
        }
      });
  }

  onQueryParamsChange(params: NzTableQueryParams): void {
    this.pageIndex = params.pageIndex;
    this.pageSize = params.pageSize;
    this.loadDataFromServer(this.pageIndex, this.pageSize);
  }

  loadDataFromServer(pageIndex: number, pageSize: number): void {
    this.tableService.getListOfTables('', pageIndex.toString(), pageSize.toString(), '');
    this.tableService.loadFullListOfEmployees();
  }

  editTable(id: number) {
    this.tableService.getTableById(id);
  }

  deleteTable(id: number) {
    this.modal.confirm({
      nzTitle: 'Are you sure you want to delete this table?',
      nzContent: 'This action cannot be undone.',
      nzOkText: 'Yes, Delete',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Cancel',
      nzOnOk: () => {
        this.tableService.deleteTable(id);
        if (this.tableService.listOfTables().length <= 1) {
          this.pageIndex = Math.max(1, this.pageIndex - 1);
        }
      }
    });
  }

  getImageUrl(imageUrl: string) {
    return this.imageBaseUrl + imageUrl;
  }

  showAssignModal(tableId: string, tableName: string, image: string, numberOfSeats: number) {
    this.tableService.assignTableId.set(tableId);
    this.tableService.assignTableImage.set(this.getImageUrl(image));
    this.tableService.assignTableSeats.set(numberOfSeats);
    this.tableService.assignTableName.set(tableName);
    this.tableService.showAssignModal.set(true);
    this.tableService.loadListOfAvailableEmployees(tableId);
  }

  protected readonly String = String;
}
