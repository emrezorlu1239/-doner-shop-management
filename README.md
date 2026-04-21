# Doner Shop Management

A Windows desktop application for döner (shawarma) shop management with inventory tracking, order management, employee management, and sales reporting.

## Features

- **Table Management** - Visual table grid with status tracking (available, occupied, cleaning)
- **Order Management** - Product menu, real-time order editing, cash/card payments
- **Stock Tracking** - Ingredient inventory with automatic stock deduction on sale, minimum stock alerts
- **Product Management** - Menu items with recipe (product-ingredient) relationships
- **Employee Management** - Role-based access (manager, cashier, waiter, kitchen)
- **Supplier Management** - Supplier contacts and ordering
- **Reports & Analytics** - Daily sales, revenue by payment method, top-selling products
- **Settings** - Password change for all roles

## Tech Stack

- **.NET 10** (Windows Forms)
- **Entity Framework Core 10** (SQLite)
- **C#**

## Built with Claude

This project was designed and implemented with Claude AI assistance.

## Getting Started

### Prerequisites

- Windows 10/11
- .NET 10 SDK

### Running the App

```bash
cd DonerApp
dotnet run
```

Default login (created on first run):
- **Username:** Mehmet Yılmaz
- **Password:** 1234

### Project Structure

```
DonerApp/
├── Models/           # Entity models
├── Migrations/      # EF Core migrations
├── Program.cs        # Entry point & seed data
├── LoginForm.cs     # Authentication
├── MainForm.cs      # Dashboard
├── OrderForm.cs     # Order management + stock deduction
├── StockForm.cs     # Inventory tracking
├── ProductForm.cs   # Product management
├── EmployeeForm.cs # Employee management
├── TableForm.cs    # Table management
├── ReportForm.cs   # Reports & analytics
├── SupplierForm.cs # Supplier management
├── SettingsForm.cs# Settings
├── Permission.cs   # Role-based access
├── Session.cs      # User session
└── AppDbContext.cs # Database context
```

## Database

SQLite database (`doner.db`) is auto-created on first run with sample data:
- 6 employees (manager, cashier, waiters, kitchen)
- 5 suppliers (meat, vegetables, beverages, bread, oil)
- 13 ingredients with stock levels
- 10 products with recipes
- 25 restaurant tables

## License

MIT License