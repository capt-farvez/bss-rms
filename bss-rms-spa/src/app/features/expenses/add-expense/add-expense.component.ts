import { Component, inject, effect, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import {
  FormsModule,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Subject } from 'rxjs';
import { ExpenseService, CreateExpense, UpdateExpense } from '../../../core/services/expense.service';

@Component({
  selector: 'app-add-expense',
  standalone: true,
  imports: [
    CommonModule,
    NzModalModule,
    NzButtonModule,
    NzFormModule,
    ReactiveFormsModule,
    NzGridModule,
    NzInputModule,
    FormsModule,
    NzSelectModule,
    NzDatePickerModule,
    NzIconModule,
    NzInputNumberModule,
  ],
  templateUrl: './add-expense.component.html',
  styleUrl: './add-expense.component.scss'
})
export class AddExpenseComponent implements OnDestroy {
  backendService: ExpenseService = inject(ExpenseService);
  responsive = inject(BreakpointObserver);
  modalWidth = "80vw";
  private destroy$ = new Subject<void>();

  fb = inject(NonNullableFormBuilder);
  validateForm = this.fb.group({
    title: this.fb.control('', [Validators.required]),
    category: this.fb.control('', [Validators.required]),
    amount: this.fb.control<number | null>(null, [Validators.required, Validators.min(0.01)]),
    expenseDate: this.fb.control<Date | null>(null, [Validators.required]),
    notes: this.fb.control(''),
  });

  categories = [
    'Ingredients',
    'Utilities',
    'Supplies',
    'Maintenance',
    'Salary',
    'Rent',
    'Other'
  ];

  constructor() {
    effect(() => {
      const selectedExpense = this.backendService.selectedExpense();
      if (selectedExpense && this.backendService.isEditMode()) {
        this.populateForm(selectedExpense);
      }
    }, { allowSignalWrites: true });

    this.responsive.observe([Breakpoints.Large, Breakpoints.XLarge])
      .subscribe(result => {
        this.modalWidth = result.matches ? "60vw" : "100vw";
      });
  }

  populateForm(expense: any) {
    this.validateForm.patchValue({
      title: expense.title,
      category: expense.category,
      amount: expense.amount,
      expenseDate: new Date(expense.expenseDate),
      notes: expense.notes || '',
    });
  }

  handleOk(): void {
    if (this.validateForm.status === "INVALID") {
      for (const i in this.validateForm.controls) {
        this.validateForm.controls[i as keyof typeof this.validateForm.controls].markAsDirty();
        this.validateForm.controls[i as keyof typeof this.validateForm.controls].updateValueAndValidity();
      }
      return;
    }

    const formValue = this.validateForm.getRawValue();
    const expenseData = {
      title: formValue.title,
      category: formValue.category,
      amount: formValue.amount!,
      expenseDate: formValue.expenseDate!.toISOString(),
      notes: formValue.notes,
    };

    if (this.backendService.isEditMode()) {
      const expenseId = this.backendService.selectedExpense()?.id;
      if (expenseId) {
        this.backendService.updateExpense(expenseId, expenseData as UpdateExpense);
      }
    } else {
      this.backendService.addNewExpense(expenseData as CreateExpense);
    }
    this.validateForm.reset();
  }

  handleCancel() {
    this.validateForm.reset();
    this.backendService.showAddModal.set(false);
    this.backendService.selectedExpense.set(null);
    this.backendService.isEditMode.set(false);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
