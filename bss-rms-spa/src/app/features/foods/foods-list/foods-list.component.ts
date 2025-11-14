import { Component, OnInit, inject, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NzTableModule, NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzModalService } from 'ng-zorro-antd/modal';
import { FoodService } from '../../../core/services/food.service';
import { AddFoodComponent } from '../add-food/add-food.component';
import { API_BASE_URL } from '../../../app.config';

@Component({
  selector: 'app-foods-list',
  standalone: true,
  providers: [NzModalService],
  imports: [
    CommonModule,
    NzTableModule,
    NzAvatarModule,
    NzIconModule,
    NzToolTipModule,
    NzButtonModule,
    AddFoodComponent
  ],
  templateUrl: './foods-list.component.html',
  styleUrl: './foods-list.component.scss'
})
export class FoodsListComponent implements OnInit {
  foodService: FoodService = inject(FoodService);
  private modal = inject(NzModalService);

  pageSize = 10;
  pageIndex = 1;
  imageBaseUrl = inject(API_BASE_URL) + '/images/food/';

  // Expose signals for template
  listOfFood = this.foodService.listOfFood;

  constructor() {
    // Setup refresh trigger effect
    effect(() => {
      if (this.foodService.triggerRefresh()) {
        this.ngOnInit();
        this.foodService.triggerRefresh.set(false);
      }
    }, { allowSignalWrites: true });
  }

  ngOnInit() {
    this.loadDataFromServer(this.pageIndex, this.pageSize);
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
    this.foodService.getListOfFood(sortBy, pageIndex.toString(), pageSize.toString());
  }

  getImageUrl(image: string): string {
    return this.imageBaseUrl + image;
  }

  editFood(id: number): void {
    this.foodService.getFoodById(id);
  }

  deleteFood(id: number): void {
    this.modal.confirm({
      nzTitle: 'Are you sure you want to delete this food item?',
      nzContent: 'This action cannot be undone.',
      nzOkText: 'Yes, Delete',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Cancel',
      nzOnOk: () => {
        this.foodService.deleteFood(id);
        // Reset to previous page if deleting last item on current page
        if (this.foodService.listOfFood().length <= 1 && this.pageIndex > 1) {
          this.pageIndex--;
          this.loadDataFromServer(this.pageIndex, this.pageSize);
        }
      }
    });
  }

  getDiscountDisplay(data: any): string {
    if (data.discountType === 'None') {
      return '-';
    } else if (data.discountType === 'Percentage') {
      return data.discount + ' %';
    } else {
      return  data.discount;
    }
  }
}