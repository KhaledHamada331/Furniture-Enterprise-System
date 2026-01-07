# Furniture Enterprise System - Entity Relationship Diagram

## Entities and Relationships

### User
- UserId (PK)
- Username
- Password (Hashed)
- FullName
- Email
- Role
- CreatedAt
- IsActive

### Product
- ProductId (PK)
- Name
- Description
- Category
- PurchasePrice
- SellingPrice
- StockQuantity
- MinimumStock
- CreatedAt
- LastModified

### Supplier
- SupplierId (PK)
- Name
- ContactPerson
- Email
- Phone
- Address
- CreatedAt
- IsActive

### Purchase
- PurchaseId (PK)
- SupplierId (FK)
- PurchaseDate
- TotalAmount
- Status
- PaymentMethod
- Notes
- CreatedAt
- Relationships:
  - Has many PurchaseItems
  - Belongs to one Supplier

### PurchaseItem
- PurchaseItemId (PK)
- PurchaseId (FK)
- ProductId (FK)
- Quantity
- UnitPrice
- TotalPrice
- Relationships:
  - Belongs to one Purchase
  - Belongs to one Product

### Sale
- SaleId (PK)
- UserId (FK)
- SaleDate
- TotalAmount
- Status
- PaymentMethod
- CustomerName
- CustomerPhone
- Notes
- CreatedAt
- Relationships:
  - Has many SaleItems
  - Belongs to one User (who processed the sale)

### SaleItem
- SaleItemId (PK)
- SaleId (FK)
- ProductId (FK)
- Quantity
- UnitPrice
- TotalPrice
- Relationships:
  - Belongs to one Sale
  - Belongs to one Product

### Expense
- ExpenseId (PK)
- Category
- Description
- Amount
- Date
- PaymentMethod
- Notes
- CreatedAt
- CreatedBy (FK to User)

### Payroll
- PayrollId (PK)
- UserId (FK)
- PayPeriodStart
- PayPeriodEnd
- BaseSalary
- OvertimePay
- Bonuses
- Deductions
- TotalPay
- PaymentStatus
- PaymentDate
- Notes
- CreatedAt
- Relationships:
  - Belongs to one User

## Relationships Overview

1. User -> Sales: One-to-Many
   - A user can process many sales
   - Each sale is processed by one user

2. User -> Payroll: One-to-Many
   - A user can have many payroll records
   - Each payroll record belongs to one user

3. Supplier -> Purchases: One-to-Many
   - A supplier can have many purchases
   - Each purchase is from one supplier

4. Product -> PurchaseItems: One-to-Many
   - A product can be in many purchase items
   - Each purchase item refers to one product

5. Product -> SaleItems: One-to-Many
   - A product can be in many sale items
   - Each sale item refers to one product

6. Purchase -> PurchaseItems: One-to-Many
   - A purchase has many purchase items
   - Each purchase item belongs to one purchase

7. Sale -> SaleItems: One-to-Many
   - A sale has many sale items
   - Each sale item belongs to one sale

## Database Constraints

1. Product Inventory
   - Stock quantity cannot be negative
   - Purchase price and selling price must be positive

2. Transactions
   - Sale and purchase amounts must be positive
   - Sale quantity cannot exceed available stock
   - Purchase status transitions: Pending -> Completed/Cancelled
   - Sale status transitions: Pending -> Completed/Cancelled

3. Financial
   - Expense amounts must be positive
   - Payroll amounts must be non-negative
   - Payment methods must be from predefined list

4. Users
   - Email must be unique
   - Username must be unique
   - Password must meet security requirements 