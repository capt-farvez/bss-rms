import { Component, effect, inject, Injector, signal } from '@angular/core';
import { NzButtonComponent } from 'ng-zorro-antd/button';
import { NzColDirective, NzRowDirective } from 'ng-zorro-antd/grid';
import { NzFormControlComponent, NzFormDirective, NzFormItemComponent, NzFormLabelComponent } from 'ng-zorro-antd/form';
import { NzIconDirective } from 'ng-zorro-antd/icon';
import { NzInputDirective } from 'ng-zorro-antd/input';
import { NzModalComponent, NzModalContentDirective, NzModalFooterDirective } from 'ng-zorro-antd/modal';
import { NzOptionComponent, NzSelectComponent } from 'ng-zorro-antd/select';
import { NzUploadChangeParam, NzUploadComponent, NzUploadFile } from 'ng-zorro-antd/upload';
import {
  AbstractControl,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators
} from '@angular/forms';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Observable, Observer, Subject } from 'rxjs';
import { toObservable } from '@angular/core/rxjs-interop';
import { TableService } from '../../../core/services/table.service';
import { CreateTable, UpdateTable } from '../../../core/models/table.interface';
import { Table } from '../../../core/models/table.interface';

@Component({
  selector: 'app-add-table',
  standalone: true,
  imports: [
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
    NzUploadComponent,
    ReactiveFormsModule,
    NzModalContentDirective,
    NzModalFooterDirective
  ],
  templateUrl: './add-table.component.html',
  styleUrl: './add-table.component.scss'
})
export class AddTableComponent {
  tableService = inject(TableService);
  responsive = inject(BreakpointObserver);
  modalWidth = "50vw";
  discType = signal("");

  handleOk(): void {
    if (this.validateForm.status === "INVALID") {
      for (const i in this.validateForm.controls) {
        this.validateForm.controls[i as keyof typeof this.validateForm.controls].markAsDirty();
        this.validateForm.controls[i as keyof typeof this.validateForm.controls].updateValueAndValidity();
      }
    }
    if (this.validateForm.status === "VALID") {
      if (this.tableService.isEditMode()) {
        // Update existing table
        const updateData: UpdateTable = {
          id: this.tableService.selectedTable()!.id,
          tableNumber: this.validateForm.controls.tableName.value,
          numberOfSeats: this.validateForm.controls.numberofseats.value,
        };

        // Only include image if a new one was uploaded
        if (this.image && this.imageB64) {
          updateData.image = this.image;
          updateData.base64 = this.imageB64;
        }

        this.tableService.updateTable(updateData.id, updateData);
      } else {
        // Create new table
        const POST_VALUES: CreateTable = {
          tableNumber: this.validateForm.controls.tableName.value,
          numberOfSeats: this.validateForm.controls.numberofseats.value,
          image: this.image,
          base64: this.imageB64,
        };

        this.tableService.addNewTable(POST_VALUES);
      }

      setTimeout(() => {
        this.tableService.triggerRefresh.set(false);
        this.handleCancel();
        this.tableService.triggerRefresh.set(true);
      }, 1000);
    }
  }

  handleCancel() {
    this.fileList = [];
    this.image = '';
    this.imageB64 = '';
    this.validateForm.reset();
    this.tableService.showAddModal.set(false);
    this.tableService.selectedTable.set(null);
    this.tableService.isEditMode.set(false);
  }

  // FORM SECTION
  fb = inject(NonNullableFormBuilder);
  private destroy$ = new Subject<void>();
  private injector = inject(Injector);
  validateForm = this.fb.group({
    tableName: this.fb.control('', [Validators.required], [this.nameValidator]),
    numberofseats: this.fb.control('', [Validators.required], [this.isNumberValidator]),
  });

  constructor() {
    // Watch for edit mode changes using toObservable
    toObservable(this.tableService.selectedTable, {
      injector: this.injector
    }).subscribe((table: Table | null) => {
      if (table && this.tableService.isEditMode()) {
        this.validateForm.patchValue({
          tableName: table.tableNumber,
          numberofseats: table.numberOfSeats.toString()
        });
        // Don't load existing image, let user upload new one if needed
      }
    });
  }

  ngOnInit() {
    this.responsive.observe([Breakpoints.Large, Breakpoints.XLarge])
      .subscribe(result => {
        this.modalWidth = "100vw";
        if (result.matches) {
          this.modalWidth = "50vw";
        } else {
          this.modalWidth = "100vw";
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // Validators
  isNumberValidator(control: AbstractControl): Observable<ValidationErrors | null> {
    return new Observable((observer: Observer<ValidationErrors | null>) => {
      setTimeout(() => {
        if (control.value.length > 0 && !/^\d*$/.test(control.value)) {
          observer.next({ error: true, isNotNum: true });
        } else {
          observer.next(null);
        }
        observer.complete();
      }, 500);
    });
  }

  nameValidator(control: AbstractControl): Observable<ValidationErrors | null> {
    return new Observable((observer: Observer<ValidationErrors | null>) => {
      setTimeout(() => {
        if (!/^[a-z0-9]+$/i.test(control.value)) {
          observer.next({ error: true, notAplhaNum: true });
        } else {
          observer.next(null);
        }
        observer.complete();
      }, 500);
    });
  }

  submitForm() {
    // Not used - handleOk is used instead
  }

  // File Upload Section
  image = '';
  imageB64 = '';
  fileList: NzUploadFile[] = [];
  previewImage: string | undefined = '';
  previewVisible = false;

  handlePreview = async (file: NzUploadFile): Promise<void> => {
    if (!file.url && !file['preview']) {
      file['preview'] = await this.getBase64(file.originFileObj!);
    }
    this.previewImage = file.url || file['preview'];
    this.previewVisible = true;
  };

  getBase64 = (file: File): Promise<string> =>
    new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => resolve(reader.result as string);
      reader.onerror = error => reject(error);
    });

  beforeUpload(file: NzUploadFile, fileList: NzUploadFile[]): boolean {
    return true;
  }

  onChange(event: NzUploadChangeParam) {
    const reader = new FileReader();
    if (event.file.originFileObj) {
      reader.onloadend = () => {
        this.imageB64 = reader.result as string;
        this.image = event.file.uid + event.file.name;
      };
      reader.readAsDataURL(event.file.originFileObj);
    }

    if (event.type === 'removed') {
      this.image = "";
      this.imageB64 = "";
    }

    if (event.type === "error") {
      event.file.error.statusText = "Table Image";
      event.file.error.status = "200";
    }
  }
}
