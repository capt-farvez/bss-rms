import { Component, effect, inject, Injector } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NzButtonComponent } from 'ng-zorro-antd/button';
import { NzColDirective, NzRowDirective } from 'ng-zorro-antd/grid';
import { NzFormControlComponent, NzFormDirective, NzFormItemComponent, NzFormLabelComponent } from 'ng-zorro-antd/form';
import { NzIconDirective } from 'ng-zorro-antd/icon';
import { NzInputDirective, NzInputGroupComponent } from 'ng-zorro-antd/input';
import { NzModalComponent, NzModalContentDirective, NzModalFooterDirective } from 'ng-zorro-antd/modal';
import { NzOptionComponent, NzSelectComponent } from 'ng-zorro-antd/select';
import { NzAvatarComponent } from 'ng-zorro-antd/avatar';
import { toObservable } from '@angular/core/rxjs-interop';
import { TableService } from '../../../core/services/table.service';
import { Employee } from '../../../core/models/employee.interface';
import { API_BASE_URL } from '../../../app.config';

@Component({
  selector: 'app-assign-employee-to-table',
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule,
    NzButtonComponent,
    NzColDirective,
    NzFormControlComponent,
    NzFormDirective,
    NzFormItemComponent,
    NzFormLabelComponent,
    NzIconDirective,
    NzInputDirective,
    NzModalComponent,
    NzOptionComponent,
    NzRowDirective,
    NzSelectComponent,
    NzModalContentDirective,
    NzModalFooterDirective,
    NzInputGroupComponent,
    NzAvatarComponent,
  ],
  templateUrl: './assign-employee-to-table.component.html',
  styleUrl: './assign-employee-to-table.component.scss'
})
export class AssignEmployeeToTableComponent {
  tableService = inject(TableService);
  private baseUrl = inject(API_BASE_URL);
  modalWidth = "600px";
  inputValue?: string;
  listOfAvailableEmployees: Employee[] = this.tableService.listOfAvailableEmployees();
  private injector = inject(Injector);
  selectedEmployee = '';
  listOfSelectedEmployees: Employee[] = [];

  ngOnInit(): void {
  }

  constructor() {
    toObservable(this.tableService.showAssignModal, {
      injector: this.injector
    }).subscribe(
      value => {
        if (value) {
          this.tableService.loadListOfAvailableEmployees(this.tableService.assignTableId());
        } else {
          // Modal closed (service closes it on success) — clear local selections
          this.inputValue = "";
          this.listOfSelectedEmployees = [];
        }
      }
    );

    toObservable(this.tableService.listOfAvailableEmployees, {
      injector: this.injector
    }).subscribe(
      value => {
        this.listOfAvailableEmployees = value;
      }
    );

    toObservable(this.tableService.assignTableId, {
      injector: this.injector
    }).subscribe(
      value => {
        // Handle table ID changes if needed
      }
    );
  }

  onChange(e: Event): void {
    const value = (e.target as HTMLInputElement).value;
    this.listOfAvailableEmployees = this.tableService.listOfAvailableEmployees().filter((emp) => {
      if ((emp.user.firstName.toLowerCase() + emp.user.middleName.toLowerCase() + emp.user.lastName.toLowerCase()).includes(value.toLowerCase())) {
        return emp;
      }
      return;
    });
  }

  submitForm() {
    // Not used - handleOk is used instead
  }

  handleCancel() {
    this.tableService.showAssignModal.set(false);
    this.inputValue = "";
    this.listOfSelectedEmployees = [];
  }

  handleOk() {
    if (this.listOfSelectedEmployees.length > 0) {
      this.tableService.assignEmployeeToTable(this.listOfSelectedEmployees, this.tableService.assignTableId());
    }
  }

  employeeSelected(id: string) {
    this.selectedEmployee = id;
    console.log(this.selectedEmployee);
  }

  getEmployeeImageUrl(imageName: string | null | undefined): string {
    if (!imageName || imageName === 'null') {
      return ''; // Return empty string to use the nzIcon fallback
    }
    return `${this.baseUrl}/images/user/${imageName}`;
  }

  getEmployeeFullName(emp: Employee): string {
    const firstName = emp.user.firstName || '';
    const middleName = emp.user.middleName || '';
    const lastName = emp.user.lastName || '';
    return `${firstName} ${middleName} ${lastName}`.trim();
  }
}
