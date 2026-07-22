import { effect, inject, Injectable, signal } from '@angular/core';
import { HttpClient, HttpEventType, HttpParams } from '@angular/common/http';
import {
  CreateTable,
  ResponseAvailableEmployees,
  ResponseTableList,
  Table,
  AssignEmployeeToTable,
  UpdateTable
} from '../models/table.interface';
import { Employee } from '../models/employee.interface';
import { NzMessageService } from 'ng-zorro-antd/message';
import { API_BASE_URL } from '../../app.config';

function getSanitizedListOfTable(data: ResponseTableList | null) {
  return data!.data;
}

function getAvailableEmployee(data: ResponseAvailableEmployees[] | null, listOfEmployees: Employee[]): Employee[] {
  let employees: Employee[] = [];
  listOfEmployees.forEach(employee => {
    if (data) {
      data.forEach(item => {
        if (item.employeeId == employee.id) {
          employees.push(employee);
        }
      });
    }
  });
  return employees;
}

@Injectable({ providedIn: 'root' })
export class TableService {
  private baseUrl = inject(API_BASE_URL);
  
  private httpClient = inject(HttpClient);
  private messageService = inject(NzMessageService);

  // State signals
  isSendingRequest = signal(false);
  triggerRefresh = signal(false);
  listOfTables = signal<Table[]>([]);
  listOfEmployees = signal<Employee[]>([]);
  totalTables = signal(10);
  showAddModal = signal(false);
  showAssignModal = signal(false);

  // Edit mode signals
  selectedTable = signal<Table | null>(null);
  isEditMode = signal(false);

  // Assign employee modal signals
  listOfAvailableEmployees = signal<Employee[]>([]);
  assignTableId = signal('');
  assignTableNumber = signal('');  // Table number/name for display
  assignTableName = signal('');
  assignTableImage = signal('');
  assignTableSeats = signal(0);
  isLoadingEmployees = signal(false);
  isLoadingTables = signal(false);

  constructor() {
    effect(() => {
      // Any side effects can be handled here
    });
  }

  getListOfTables(sortBy: string, page: string, per_page: string, search: string) {
    let params = new HttpParams()
      .append('Sort', sortBy)
      .append('Page', page)
      .append('Per_Page', per_page);

    this.httpClient.get<ResponseTableList>(`${this.baseUrl}/api/Table/datatable`, { observe: 'events', params: params })
      .pipe()
      .subscribe({
        next: (data) => {
          switch (data.type) {
            case HttpEventType.Response:
              if ((data.status) === 200) {
                this.listOfTables.set(getSanitizedListOfTable(data.body));
                this.totalTables.set(data.body!.totalRecords);
              }
              break;
            case HttpEventType.Sent:
              this.isSendingRequest.set(true);
              this.isLoadingTables.set(true);
              break;
          }
        },
        error: (err) => {
          this.messageService.error('Error Processing The Request. Please Try Again...');
          this.isLoadingTables.set(false);
        },
        complete: () => {
          this.isLoadingTables.set(false);
        }
      });
  }

  loadFullListOfEmployees() {
    let params = new HttpParams()
      .append('Sort', '')
      .append('Page', '1')
      .append('Per_Page', '10000');

    this.httpClient.get<any>(`${this.baseUrl}/api/Employee/datatable/`, { observe: 'events', params: params })
      .pipe()
      .subscribe({
        next: (data) => {
          switch (data.type) {
            case HttpEventType.Response:
              if ((data.status) === 200) {
                this.listOfEmployees.set(data.body!.data);
              }
              break;
            case HttpEventType.Sent:
              this.isSendingRequest.set(true);
              this.isLoadingEmployees.set(true);
              break;
          }
        },
        error: (err) => {
          this.messageService.error('Error Processing The Request. Please Try Again...');
          this.isLoadingEmployees.set(false);
        },
        complete: () => {
          this.isLoadingEmployees.set(false);
        }
      });
  }

  deleteTable(id: number) {
    this.httpClient.delete(`${this.baseUrl}/api/Table/delete/${id}`, { observe: 'events' })
      .pipe()
      .subscribe({
        next: (data) => {
          switch (data.type) {
            case HttpEventType.Response:
              // API delete endpoints return 200 with a body, not 204
              if (data.status >= 200 && data.status < 300) {
                this.messageService.success('Table Removed Successfully!');
                this.isSendingRequest.set(false);
              }
              break;
            case HttpEventType.Sent:
              this.isSendingRequest.set(true);
              break;
          }
        },
        error: (err) => {
          this.messageService.error('Error Processing The Request. Please Try Again...');
          this.isSendingRequest.set(false);
        },
        complete: () => {
          this.triggerRefresh.set(true);
        }
      });
  }

  addNewTable(postData: CreateTable) {
    this.httpClient.post(`${this.baseUrl}/api/Table/create`, postData, { observe: 'events' })
      .pipe()
      .subscribe({
        next: (data) => {
          switch (data.type) {
            case HttpEventType.Response:
              if ((data.status) === 200) {
                this.messageService.success('Table Added Successfully!');
                this.isSendingRequest.set(false);
                this.triggerRefresh.set(true);
              }
              break;
            case HttpEventType.Sent:
              this.isSendingRequest.set(true);
              break;
          }
        },
        error: (err) => {
          this.messageService.error('Error Processing The Request. Please Try Again...');
          this.isSendingRequest.set(false);
        },
        complete: () => {
          this.isSendingRequest.set(false);
        }
      });
  }

  updateTable(id: number, updateData: UpdateTable) {
    this.httpClient.put(`${this.baseUrl}/api/Table/update/${id}`, updateData, { observe: 'events' })
      .pipe()
      .subscribe({
        next: (data) => {
          switch (data.type) {
            case HttpEventType.Response:
              if ((data.status) === 200) {
                this.messageService.success('Table Updated Successfully!');
                this.isSendingRequest.set(false);
                this.triggerRefresh.set(true);
                this.selectedTable.set(null);
                this.isEditMode.set(false);
              }
              break;
            case HttpEventType.Sent:
              this.isSendingRequest.set(true);
              break;
          }
        },
        error: (err) => {
          this.messageService.error('Error updating table');
          this.isSendingRequest.set(false);
        },
        complete: () => {
          this.isSendingRequest.set(false);
        }
      });
  }

  getTableById(id: number) {
    this.httpClient.get<Table>(`${this.baseUrl}/api/Table/get/${id}`, { observe: 'events' })
      .pipe()
      .subscribe({
        next: (data) => {
          switch (data.type) {
            case HttpEventType.Response:
              if ((data.status) === 200) {
                this.selectedTable.set(data.body);
                this.isEditMode.set(true);
                this.showAddModal.set(true);
                this.isSendingRequest.set(false);
              }
              break;
            case HttpEventType.Sent:
              this.isSendingRequest.set(true);
              break;
          }
        },
        error: (err) => {
          this.messageService.error('Error fetching table details');
          this.isSendingRequest.set(false);
        }
      });
  }

  removeEmployeeFromTable(employeeTableId: string) {
    this.httpClient.delete(`${this.baseUrl}/api/EmployeeTable/delete/${employeeTableId}`, { observe: 'events' })
      .pipe()
      .subscribe({
        next: (data) => {
          switch (data.type) {
            case HttpEventType.Response:
              // API delete endpoints return 200 with a body, not 204
              if (data.status >= 200 && data.status < 300) {
                this.messageService.success('Employee Removed Successfully!');
                this.isSendingRequest.set(false);
              }
              break;
            case HttpEventType.Sent:
              this.isSendingRequest.set(true);
              break;
          }
        },
        error: (err) => {
          this.messageService.error('Error Processing The Request. Please Try Again...');
          this.isSendingRequest.set(false);
        },
        complete: () => {
          this.triggerRefresh.set(true);
        }
      });
  }

  loadListOfAvailableEmployees(tableId: string) {
    this.httpClient.get<ResponseAvailableEmployees[]>(`${this.baseUrl}/api/Employee/non-assigned-employees/${tableId}`, { observe: 'events' })
      .pipe()
      .subscribe({
        next: (data) => {
          switch (data.type) {
            case HttpEventType.Response:
              if ((data.status) === 200) {
                this.listOfAvailableEmployees.set(getAvailableEmployee(data.body, this.listOfEmployees()));
              }
              break;
            case HttpEventType.Sent:
              break;
          }
        },
        error: (err) => {
          this.messageService.error('Error Processing The Request. Please Try Again...');
        },
        complete: () => {
        }
      });
  }

  assignEmployeeToTable(selectedEmployees: Employee[], tableId: string) {
    let listOfEmployees: AssignEmployeeToTable[] = [];
    selectedEmployees.forEach(emp => {
      listOfEmployees.push({ employeeId: emp.id, tableId: tableId });
    });

    this.httpClient.post(`${this.baseUrl}/api/EmployeeTable/create-range`, listOfEmployees, { observe: 'events' })
      .pipe()
      .subscribe({
        next: (data) => {
          switch (data.type) {
            case HttpEventType.Response:
              if ((data.status) === 200) {
                this.messageService.success('Employees Assigned Successfully!');
                this.isSendingRequest.set(false);
                this.triggerRefresh.set(true);
              }
              break;
            case HttpEventType.Sent:
              this.isSendingRequest.set(true);
              break;
          }
        },
        error: (err) => {
          this.messageService.error('Error Processing The Request. Please Try Again...');
          this.isSendingRequest.set(false);
        },
        complete: () => {
          this.isSendingRequest.set(false);
        }
      });
  }
}