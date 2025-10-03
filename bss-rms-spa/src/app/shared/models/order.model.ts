import { Table } from './table.model';
import { User } from './auth.model';
import { FoodItem, FoodPackage } from './food.model';

export interface Order {
  id: string;
  orderNumber: string;
  amount: number;
  orderStatus: OrderStatus;
  orderTime: string;
  table: Table;
  orderedBy: User;
  orderTakenBy: User;
  orderItems: OrderItem[];
}

export enum OrderStatus {
  PENDING = 'pending',
  PROCESSING = 'processing',
  COMPLETED = 'completed',
  CANCELLED = 'cancelled',
  DELIVERED = 'delivered'
}

export interface OrderItem {
  id: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  food?: FoodItem;
  foodPackage?: FoodPackage;
}

export interface CreateOrderRequest {
  tableId: number;
  customerId?: string;
  orderItems: CreateOrderItemRequest[];
  notes?: string;
}

export interface CreateOrderItemRequest {
  foodId?: number;
  packageId?: number;
  quantity: number;
  unitPrice: number;
}

export interface UpdateOrderStatusRequest {
  orderId: string;
  status: OrderStatus;
}

export interface CartItem {
  food?: FoodItem;
  foodPackage?: FoodPackage;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}