### Entity Relationship Diagram (ERD)
[Diagram Link](https://miro.com/app/board/uXjVJyHExn0=/?share_link_id=315564782637)


### User
- uid (PK) ----> UUID
- userName (Unique) ---> String (Nullable)
- email (Unique)  ---> String
- phoneNumber (Unique) ---> String
- password (Hashed) ---> String (Nullable)
- firstName () ---> String
- middleName () ---> String
- lastName () ---> String
- fatherName () ---> String
- motherName () ---> String
- spouseName () ---> String
- nid (Unique)
- dob () ---> Date
- genderId () ---> int
- image () ---> String
- imageBase64 () ---> String
- refreshToken () ---> String
- refreshTokenExpiryTime () ---> DateTime
- createdAt () ---> DateTime
- updatedAt () ---> DateTime

### Employee
- employeeId (PK) ---> UUID
- userId (FK) ---> User Table
- designation () ---> string
- joinDate() ---> date
- AmountSold () ---> decimal
- createdAt () ---> DateTime
- updatedAt () ---> DateTime

### Table
- tableId (PK) ---> (int)
- tableNumber () ---> (string)
- numberOfSeats () ---> (int)
- image() ---> (string)
- imageBase64() ---> (string)
- createdAt () ---> DateTime
- updatedAt () ---> DateTime

### EmployeeTable
- employeeTableId (PK) ---> (int)
- employeeId (FK) ---> Employee Table
- tableId (FK) ---> Table
- createdAt () ---> DateTime
- updatedAt () ---> DateTime

### Food
- foodId (PK) ---> (int)
- name () ---> (string)
- description () ---> (string)
- price () ---> (decimal)
- discountType () ---> (int {[0:None, 1:Percentage, 2:Flat]})
- discount () ---> (decimal)
- image() ---> (string)
- imageBase64() ---> (string)
- createdAt () ---> DateTime
- updatedAt () ---> DateTime

### FoodPackage
- foodPackageId (PK) ---> (int)
- foodId (FK) ---> Food Table

### Order
- orderId (PK) ---> (int)
- tableId (FK) ---> Table
- orderNumber () ---> (string)
- orderDate () ---> (date)
- amount () ---> (decimal)
- phoneNumber () ---> (string)
- status () ---> (int {[ 0:Pending, 1:Confirmed, 2:Preparing, 3:PreparedToServer, 4:Served, 5:Paid, 6:Cancelled ]})
- createdAt () ---> DateTime
- updatedAt () ---> DateTime

### OrderItem
- orderItemId (PK) ---> (int)
- orderId (FK) ---> Order Table
- foodPackageId (FK) ---> FoodPackage Table
- quantity () ---> (int)
- unitPrice () ---> (decimal)
- totalPrice () ---> (decimal)