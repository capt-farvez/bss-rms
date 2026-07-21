import { Component, effect, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzImageModule } from 'ng-zorro-antd/image';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzMessageService } from 'ng-zorro-antd/message';
import { OrderService } from '../../../core/services/order.service';
import { TableService } from '../../../core/services/table.service';
import { FoodService } from '../../../core/services/food.service';
import { UpdateOrder, UpdateOrderItem, OrderItem } from '../../../shared/models/order.model';

interface EditOrderItem {
  uniqueId: string;
  foodId: number;
  foodName: string;
  foodImage: string;
  foodPackageId: number;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

@Component({
  selector: 'app-edit-order',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    NzModalModule,
    NzButtonModule,
    NzIconModule,
    NzInputModule,
    NzInputNumberModule,
    NzImageModule,
    NzSelectModule
  ],
  templateUrl: './edit-order.component.html',
  styleUrl: './edit-order.component.scss'
})
export class EditOrderComponent {
  orderService = inject(OrderService);
  tableService = inject(TableService);
  foodService = inject(FoodService);
  message = inject(NzMessageService);

  editItems = signal<EditOrderItem[]>([]);
  phoneNumber = signal<string>('');
  totalAmount = signal<number>(0);
  selectedFoodId = signal<number | null>(null);
  private lastLoadedOrderId: string | null = null;

  fallbackImage = 'data:image/png;base64,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';

  constructor() {
    effect(() => {
      const order = this.orderService.selectedOrder();
      // Only populate if this is a different order than the last one we loaded
      if (order && order.id !== this.lastLoadedOrderId) {
        this.lastLoadedOrderId = order.id;
        this.populateOrderData(order);
      }
    }, { allowSignalWrites: true });
  }

  ngOnInit() {
    this.tableService.getListOfTables('', '1', '1000', '');
    this.foodService.getListOfFood('', '1', '1000', '');
  }

  populateOrderData(order: any) {
    this.phoneNumber.set(order.orderedBy?.phoneNumber || '');

    const items: EditOrderItem[] = order.orderItems.map((item: OrderItem, index: number) => ({
      uniqueId: `${item.food.id}-${index}-${Date.now()}`,
      foodId: item.food.id,
      foodName: item.food.name,
      foodImage: item.food.image,
      foodPackageId: item.foodPackage?.id || 0,
      quantity: item.quantity,
      unitPrice: item.unitPrice,
      totalPrice: item.totalPrice
    }));

    this.editItems.set(items);
    this.calculateTotalAmount();
  }

  handleOk() {
    const order = this.orderService.selectedOrder();
    if (!order) {
      console.error('No order selected');
      return;
    }

    console.log('Full order object:', order);
    console.log('Order table:', order.table);
    console.log('Order orderNumber:', order.orderNumber);

    // Extract tableId - handle different API response structures
    let tableId: number | undefined;
    if (order.table && typeof order.table === 'object') {
      // Try table.id first (standard structure), then table.tableId (alternative structure)
      tableId = order.table.id || order.table.tableId;
    } else if ((order as any).tableId) {
      // Fallback: check if tableId is directly on the order
      tableId = (order as any).tableId;
    }

    if (!tableId) {
      console.error('Could not extract table ID from order');
      this.message.error('Cannot update order: missing table information');
      return;
    }

    const updateData: UpdateOrder = {
      tableId: tableId,
      orderNumber: order.orderNumber,
      amount: Number(this.totalAmount()),
      phoneNumber: this.phoneNumber() || '',
      items: this.editItems().map(item => ({
        foodId: Number(item.foodId),
        foodPackageId: Number(item.foodPackageId) || 0,
        quantity: Number(item.quantity),
        unitPrice: Number(item.unitPrice),
        totalPrice: Number(item.totalPrice)
      }))
    };

    console.log('Updating order:', order.id, updateData);
    this.orderService.updateOrder(order.id, updateData);
  }

  handleCancel() {
    this.orderService.showEditModal.set(false);
    this.orderService.selectedOrder.set(null);
    this.editItems.set([]);
    this.lastLoadedOrderId = null;
  }

  onQuantityChange(item: EditOrderItem, index: number, newQuantity: number) {
    const items = [...this.editItems()];
    items[index] = {
      ...item,
      uniqueId: item.uniqueId,
      quantity: newQuantity,
      totalPrice: newQuantity * item.unitPrice
    };
    this.editItems.set(items);
    this.calculateTotalAmount();
  }

  removeItem(index: number) {
    console.log('removeItem called with index:', index);
    console.log('Current items:', this.editItems());

    // Use update to ensure reactivity
    this.editItems.update(items => items.filter((_, i) => i !== index));

    console.log('Updated editItems signal:', this.editItems());
    this.calculateTotalAmount();
  }

  calculateTotalAmount() {
    const total = this.editItems().reduce((sum, item) => sum + item.totalPrice, 0);
    this.totalAmount.set(Math.round(total * 100) / 100);
  }

  getFoodImage(image: string): string {
    return this.orderService.getFoodImage(image);
  }

  addFoodToOrder() {
    const foodId = this.selectedFoodId();
    if (!foodId) {
      this.message.warning('Please select a food item to add');
      return;
    }

    const selectedFood = this.foodService.listOfFood().find(f => f.id === foodId);
    if (!selectedFood) {
      this.message.error('Food item not found');
      return;
    }

    // Check if food already exists in the order
    const existingItemIndex = this.editItems().findIndex(item => item.foodId === foodId);

    if (existingItemIndex >= 0) {
      // If exists, increase quantity
      const items = [...this.editItems()];
      const existingItem = items[existingItemIndex];
      items[existingItemIndex] = {
        ...existingItem,
        quantity: existingItem.quantity + 1,
        totalPrice: (existingItem.quantity + 1) * existingItem.unitPrice
      };
      this.editItems.set(items);
    } else {
      // Add new item
      const price = selectedFood.discountPrice > 0 ? selectedFood.discountPrice : selectedFood.price;
      const newItem: EditOrderItem = {
        uniqueId: `${foodId}-${Date.now()}`,
        foodId: foodId,
        foodName: selectedFood.name,
        foodImage: selectedFood.image,
        foodPackageId: 0,
        quantity: 1,
        unitPrice: price,
        totalPrice: price
      };
      this.editItems.update(items => [...items, newItem]);
    }

    this.calculateTotalAmount();
    this.selectedFoodId.set(null);
    this.message.success(`Added ${selectedFood.name} to order`);
  }
}
