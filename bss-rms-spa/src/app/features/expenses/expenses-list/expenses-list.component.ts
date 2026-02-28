import { Component, OnInit, inject, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NzTableModule, NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzModalService } from 'ng-zorro-antd/modal';
import { ExpenseService } from '../../../core/services/expense.service';
import { AddExpenseComponent } from '../add-expense/add-expense.component';

@Component({
  selector: 'app-expenses-list',
  standalone: true,
  providers: [NzModalService],
  imports: [
    CommonModule,
    NzTableModule,
    NzIconModule,
    NzToolTipModule,
    NzButtonModule,
    AddExpenseComponent
  ],
  templateUrl: './expenses-list.component.html',
  styleUrl: './expenses-list.component.scss'
})
export class ExpensesListComponent implements OnInit {
  expenseService: ExpenseService = inject(ExpenseService);
  private modal = inject(NzModalService);

  pageSize = 10;
  pageIndex = 1;

  listOfExpenses = this.expenseService.listOfExpenses;

  constructor() {
    effect(() => {
      if (this.expenseService.triggerRefresh()) {
        this.loadDataFromServer(this.pageIndex, this.pageSize);
        this.expenseService.triggerRefresh.set(false);
      }
    }, { allowSignalWrites: true });
  }

  ngOnInit() {
    // Initial load is handled by nz-table's (nzQueryParams) event
  }

  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex, sort } = params;
    const currentSort = sort.find(item => item.value !== null);
    const sortBy = currentSort?.key || '';
    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.loadDataFromServer(pageIndex, pageSize, sortBy);
  }

  loadDataFromServer(pageIndex: number, pageSize: number, sortBy: string = ''): void {
    this.expenseService.getListOfExpenses(sortBy, pageIndex.toString(), pageSize.toString());
  }

  editExpense(id: number): void {
    this.expenseService.getExpenseById(id);
  }

  deleteExpense(id: number): void {
    this.modal.confirm({
      nzTitle: 'Are you sure you want to delete this expense?',
      nzContent: 'This action cannot be undone.',
      nzOkText: 'Yes, Delete',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Cancel',
      nzOnOk: () => {
        this.expenseService.deleteExpense(id);
        if (this.expenseService.listOfExpenses().length <= 1 && this.pageIndex > 1) {
          this.pageIndex--;
          this.loadDataFromServer(this.pageIndex, this.pageSize);
        }
      }
    });
  }

  formatDate(dateStr: string): string {
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
  }
}
