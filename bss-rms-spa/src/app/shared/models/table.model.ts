export interface Table {
  id: number;
  tableId?: number; // Alternative field name used in some API responses
  tableNumber: string;
  numberOfSeats: number;
  isOccupied: boolean;
  image?: string;
  employees: TableEmployee[];
}

export interface TableEmployee {
  employeeTableId: number;
  employeeId: string;
  name: string;
}

export interface CreateTableRequest {
  tableNumber: string;
  numberOfSeats: string;
  image?: string;
  base64?: string;
}

export interface UpdateTableRequest extends CreateTableRequest {
  id: number;
}

export interface AvailableEmployee {
  employeeId: string;
  name: string;
}

export interface AssignEmployeeToTableRequest {
  tableId: number;
  employeeId: string;
}