import { Component, inject, Injector, input, InputSignal } from '@angular/core';
import { NzButtonComponent } from 'ng-zorro-antd/button';
import { NzTooltipDirective } from 'ng-zorro-antd/tooltip';
import { NzAvatarComponent } from 'ng-zorro-antd/avatar';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzIconDirective } from 'ng-zorro-antd/icon';
import { NzContextMenuService, NzDropdownMenuComponent } from 'ng-zorro-antd/dropdown';
import { NzMenuDirective, NzMenuItemComponent, NzSubMenuComponent } from 'ng-zorro-antd/menu';
import { toObservable } from '@angular/core/rxjs-interop';
import { TableService } from '../../../core/services/table.service';
import { Employee } from '../../../core/models/employee.interface';

@Component({
  selector: 'app-avatar-tooltip',
  standalone: true,
  imports: [
    NzButtonComponent,
    NzTooltipDirective,
    NzAvatarComponent,
    NzIconDirective,
    NzDropdownMenuComponent,
    NzMenuDirective,
    NzMenuItemComponent,
    NzSubMenuComponent,
  ],
  providers: [NzModalService],
  templateUrl: './avatar-tooltip.component.html',
  styleUrl: './avatar-tooltip.component.scss',
})
export class AvatarTooltipComponent {
  modal = inject(NzModalService);
  employeeId: InputSignal<string> = input.required<string>();
  employeeName: InputSignal<string> = input.required<string>();
  employeeList: InputSignal<Employee[]> = input.required<Employee[]>();
  employeeImageUrl = '';
  nzContextMenuService = inject(NzContextMenuService);
  tableService = inject(TableService);
  employeeTableId = input.required<string>();
  tableName = input.required<string>();
  private injector = inject(Injector);

  constructor() {
    toObservable(this.tableService.listOfEmployees, {
      injector: this.injector
    }).subscribe(
      value => {
        if (value) {
          this.ngOnInit();
        }
      }
    );
  }

  ngOnInit(): void {
    this.employeeList().forEach((employee: Employee) => {
      if (employee.id === this.employeeId()) {
        this.employeeImageUrl = `https://restaurantapi.bssoln.com/images/user/` + `/` + employee.user.image;
      }
    });
  }

  contextMenu($event: MouseEvent, menu: NzDropdownMenuComponent): void {
    this.nzContextMenuService.create($event, menu);
  }

  removeEmployeeFromTable() {
    this.tableService.removeEmployeeFromTable(this.employeeTableId());
  }

  showDeleteConfirm(): void {
    this.modal.confirm({
      nzTitle: `Remove "${this.employeeName()}" from ${this.tableName()}?`,
      nzOkText: 'Confirm Remove',
      nzOnOk: () => this.removeEmployeeFromTable(),
      nzCancelText: 'Cancel Operation',
      nzOnCancel: () => {},
      nzCentered: true,
    });
  }
}
