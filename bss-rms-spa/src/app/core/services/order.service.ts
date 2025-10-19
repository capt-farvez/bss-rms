import { Injectable, signal, inject } from '@angular/core';
import { HttpClient, HttpEventType, HttpParams } from '@angular/common/http';
import { NzMessageService } from 'ng-zorro-antd/message';
import { OrderData, ResponseOrderList, UpdateOrder } from '../../shared/models/order.model';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private baseUrl = 'https://restaurantapi.bssoln.com';
  private http = inject(HttpClient);
  private message = inject(NzMessageService);

  // Signals for state management
  listOfOrder = signal<OrderData[]>([]);
  totalOrder = signal(0);
  isSendingRequest = signal(false);
  triggerRefresh = signal(false);

  // Edit order signals
  selectedOrder = signal<OrderData | null>(null);
  showEditModal = signal(false);

  getOrders(sortBy: string = '', page: number = 1, per_page: number = 10, search: string = '') {
    let params = new HttpParams()
      .append('Sort', sortBy)
      .append('Page', page)
      .append('Per_Page', per_page)
      .append('Search', search);

    this.http.get<ResponseOrderList>(`${this.baseUrl}/api/Order/datatable`, { 
      observe: 'events', 
      params: params 
    }).subscribe({
      next: (data) => {
        switch (data.type) {
          case HttpEventType.Response:
            if (data.status === 200) {
              this.listOfOrder.set(data.body!.data);
              this.totalOrder.set(data.body!.totalRecords);
              this.isSendingRequest.set(false);
            }
            break;
          case HttpEventType.Sent:
            this.isSendingRequest.set(true);
            break;
        }
      },
      error: (err) => {
        this.message.error('Failed to load orders. Please try again.');
        this.isSendingRequest.set(false);
      },
      complete: () => {
        this.isSendingRequest.set(false);
      }
    });
  }

  updateOrderStatus(id: string, status: string) {
    let params = {
      status: status,
    };
    
    this.http.put(`${this.baseUrl}/api/Order/update-status/${id}`, params, { 
      observe: 'events' 
    }).subscribe({
      next: (data) => {
        switch (data.type) {
          case HttpEventType.Response:
            if (data.status === 200) {
              this.message.success('Order status updated successfully.');
              this.triggerRefresh.set(true);
            }
            break;
          case HttpEventType.Sent:
            this.isSendingRequest.set(true);
            break;
        }
      },
      error: (err) => {
        this.message.error('Order status was not updated. Try again later.');
        this.isSendingRequest.set(false);
      },
      complete: () => {
        this.isSendingRequest.set(false);
      }
    });
  }

  deleteOrder(id: string) {
    this.http.delete(`${this.baseUrl}/api/Order/delete/${id}`, { 
      observe: 'events' 
    }).subscribe({
      next: (data) => {
        switch (data.type) {
          case HttpEventType.Response:
            if (data.status === 204) {
              this.isSendingRequest.set(false);
              this.message.success("Order deleted successfully.");
            }
            break;
          case HttpEventType.Sent:
            this.isSendingRequest.set(true);
            break;
        }
      },
      error: (err) => {
        this.message.error("Order could not be deleted. Try again later.");
        this.isSendingRequest.set(false);
      },
      complete: () => {
        this.triggerRefresh.set(true);
        this.isSendingRequest.set(false);
      }
    });
  }

  getFoodImage(image: string): string {
    return `${this.baseUrl}/images/food/${image}`;
  }

  updateOrder(id: string, orderData: UpdateOrder) {
    this.http.put(`${this.baseUrl}/api/Order/update/${id}`, orderData, {
      observe: 'events'
    }).subscribe({
      next: (data) => {
        switch (data.type) {
          case HttpEventType.Response:
            if (data.status === 200) {
              this.message.success('Order updated successfully.');
              this.triggerRefresh.set(true);
              this.showEditModal.set(false);
              this.selectedOrder.set(null);
            }
            break;
          case HttpEventType.Sent:
            this.isSendingRequest.set(true);
            break;
        }
      },
      error: (err) => {
        console.error('Update order error:', err);
        console.error('Error response:', err.error);
        this.message.error('Failed to update order. Please try again.');
        this.isSendingRequest.set(false);
      },
      complete: () => {
        this.isSendingRequest.set(false);
      }
    });
  }

  getOrderById(id: string) {
    this.http.get<OrderData>(`${this.baseUrl}/api/Order/get/${id}`, {
      observe: 'events'
    }).subscribe({
      next: (data) => {
        switch (data.type) {
          case HttpEventType.Response:
            if (data.status === 200) {
              console.log('API Response for order:', data.body);
              this.selectedOrder.set(data.body);
              this.showEditModal.set(true);
              this.isSendingRequest.set(false);
            }
            break;
          case HttpEventType.Sent:
            this.isSendingRequest.set(true);
            break;
        }
      },
      error: (err) => {
        this.message.error('Failed to load order details. Please try again.');
        this.isSendingRequest.set(false);
      },
      complete: () => {
        this.isSendingRequest.set(false);
      }
    });
  }
}
