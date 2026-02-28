import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpParams, HttpEventType } from '@angular/common/http';
import { NzMessageService } from 'ng-zorro-antd/message';
import { API_BASE_URL } from '../../app.config';

export interface ExpenseItem {
  id: number;
  title: string;
  category: string;
  amount: number;
  expenseDate: string;
  notes: string | null;
}

export interface ResponseExpenseList {
  data: ExpenseItem[];
  current_page: number;
  per_page: number;
  total: number;
  last_page: number;
}

export interface CreateExpense {
  title: string;
  category: string;
  amount: number;
  expenseDate: string;
  notes: string;
}

export interface UpdateExpense {
  title: string;
  category: string;
  amount: number;
  expenseDate: string;
  notes: string;
}

@Injectable({
  providedIn: 'root'
})
export class ExpenseService {
  private http = inject(HttpClient);
  private message = inject(NzMessageService);
  private baseUrl = inject(API_BASE_URL);

  isSendingRequest = signal(false);
  triggerRefresh = signal(false);
  listOfExpenses = signal<ExpenseItem[]>([]);
  totalExpenses = signal(10);
  showAddModal = signal(false);
  selectedExpense = signal<ExpenseItem | null>(null);
  isEditMode = signal(false);

  getListOfExpenses(sortBy: string, page: string, per_page: string, search: string = '') {
    let params = new HttpParams()
      .append('Sort', sortBy)
      .append('Page', page)
      .append('Per_Page', per_page);

    if (search) {
      params = params.append('Search', search);
    }

    this.http.get<ResponseExpenseList>(`${this.baseUrl}/api/Expense/datatable/`, {
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
              this.listOfExpenses.set(data.body?.data || []);
              this.totalExpenses.set(data.body?.total || 10);
              this.isSendingRequest.set(false);
            }
            break;
        }
      },
      error: () => {
        this.isSendingRequest.set(false);
        this.message.create('error', 'Error Processing The Request. Please Try Again...');
      }
    });
  }

  addNewExpense(postData: CreateExpense) {
    this.http.post(`${this.baseUrl}/api/Expense/create`, postData, {
      observe: 'events'
    }).subscribe({
      next: (data) => {
        switch (data.type) {
          case HttpEventType.Sent:
            this.isSendingRequest.set(true);
            break;
          case HttpEventType.Response:
            if (data.status === 200) {
              this.message.create('success', 'Expense Added Successfully!');
              this.isSendingRequest.set(false);
              this.showAddModal.set(false);
              this.selectedExpense.set(null);
              this.isEditMode.set(false);
              this.triggerRefresh.set(true);
            } else {
              this.isSendingRequest.set(false);
            }
            break;
        }
      },
      error: () => {
        this.isSendingRequest.set(false);
        this.message.create('error', 'Error Processing The Request. Please Try Again...');
      }
    });
  }

  deleteExpense(id: number) {
    this.isSendingRequest.set(true);
    this.http.delete(`${this.baseUrl}/api/Expense/delete/${id}`, {
      observe: 'events'
    }).subscribe({
      next: (data) => {
        switch (data.type) {
          case HttpEventType.Response:
            if (data.status === 200) {
              this.message.create('success', 'Expense Removed Successfully!');
              this.triggerRefresh.set(true);
            }
            this.isSendingRequest.set(false);
            break;
        }
      },
      error: () => {
        this.isSendingRequest.set(false);
        this.message.create('error', 'Error Processing The Request. Please Try Again...');
      }
    });
  }

  getExpenseById(id: number) {
    this.http.get<ExpenseItem>(`${this.baseUrl}/api/Expense/get/${id}`, {
      observe: 'events'
    }).subscribe({
      next: (data) => {
        switch (data.type) {
          case HttpEventType.Sent:
            this.isSendingRequest.set(true);
            break;
          case HttpEventType.Response:
            if (data.status === 200) {
              this.selectedExpense.set(data.body);
              this.isEditMode.set(true);
              this.showAddModal.set(true);
              this.isSendingRequest.set(false);
            }
            break;
        }
      },
      error: () => {
        this.message.create('error', 'Error fetching expense details');
        this.isSendingRequest.set(false);
      }
    });
  }

  updateExpense(id: number, updateData: UpdateExpense) {
    this.http.put(`${this.baseUrl}/api/Expense/update/${id}`, updateData, {
      observe: 'events'
    }).subscribe({
      next: (data) => {
        switch (data.type) {
          case HttpEventType.Sent:
            this.isSendingRequest.set(true);
            break;
          case HttpEventType.Response:
            if (data.status === 200) {
              this.message.create('success', 'Expense Updated Successfully!');
              this.isSendingRequest.set(false);
              this.triggerRefresh.set(true);
              this.selectedExpense.set(null);
              this.isEditMode.set(false);
            }
            break;
        }
      },
      error: () => {
        this.message.create('error', 'Error updating expense');
        this.isSendingRequest.set(false);
      },
      complete: () => {
        this.isSendingRequest.set(false);
      }
    });
  }
}
