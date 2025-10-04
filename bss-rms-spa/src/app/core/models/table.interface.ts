export interface ResponseTableList {
  pageNumber: number;
  current_page: number;
  per_page: number;
  pageSize: number;
  firstPage: string;
  lastPage: string;
  last_page: number;
  totalPages: number;
  totalRecords: number;
  total: number;
  from: number;
  to: number;
  next_page_url: any;
  prev_page_url: any;
  data: Table[];
}

export interface Table {
  id: number;
  tableNumber: string;
  numberOfSeats: number;
  isOccupied: boolean;
  image: string;
  employees: TableEmployee[];
}

export interface TableEmployee {
  employeeTableId: number;
  employeeId: string;
  name: string;
}

export interface CreateTable {
  tableNumber: string;
  numberOfSeats: string;
  image: string;
  base64: string;
}

export interface UpdateTable {
  id: number;
  tableNumber: string;
  numberOfSeats: string;
  image?: string;
  base64?: string;
}

export interface ResponseAvailableEmployees {
  employeeId: string;
  name: string;
}

export interface AssignEmployeeToTable {
  employeeId: string;
  tableId: string;
}