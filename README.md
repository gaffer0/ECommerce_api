# EC_V2 — E-Commerce REST API

> A production-ready ASP.NET Core Web API for managing an e-commerce platform with phone-based OTP authentication, role-based authorization, product management, orders, coupons, and vendor/customer profiles.

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Architecture](#2-architecture)
3. [Tech Stack](#3-tech-stack)
4. [Installation & Setup](#4-installation--setup)
5. [Project Structure](#5-project-structure)
6. [Core Features](#6-core-features)
7. [API Documentation](#7-api-documentation)
8. [Database Schema](#8-database-schema)
9. [Configuration](#9-configuration)
10. [Usage Examples](#10-usage-examples)
11. [Deployment Guide](#11-deployment-guide)
12. [Future Improvements](#12-future-improvements)
13. [Troubleshooting](#13-troubleshooting)

---

## 1. Project Overview

### What the Application Does

EC_V2 is a fully-featured e-commerce backend API built with ASP.NET Core 8. It powers a marketplace platform where vendors can list products and customers can browse, filter, and place orders — all secured behind phone-based OTP authentication and JWT tokens.

### Problem It Solves

Traditional e-commerce backends require email/password authentication and complex session management. EC_V2 solves this by:

- Using **phone number + OTP** instead of passwords — no forgotten credentials
- Supporting **Egypt (+20) and Saudi Arabia (+966)** phone numbers natively
- Providing a **clean REST API** that any frontend (web, mobile) can consume
- Handling **concurrent stock deduction** safely during checkout

### Key Features

- 📱 Phone + OTP authentication (no passwords)
- 🔐 JWT access tokens (15 min) + refresh token rotation (7 days)
- 🚫 Token blacklist middleware for instant logout
- 👤 Role-based authorization (Admin, Vendor, Customer)
- 🛍️ Product management with category hierarchy
- 🔍 Cursor-based pagination with filtering, search, and sorting
- 🧾 Order management with tax (14% VAT), discounts, and coupons
- 💰 Coupon system (percentage & fixed discounts)
- 🏪 Vendor profiles with store management
- 👥 Customer profiles with shipping addresses
- 📦 Stock management with automatic deduction on order
- 📝 Structured logging with Serilog

### Target Users

| Role | Description |
|------|-------------|
| **Admin** | Full system access — manage all orders, users, coupons |
| **Vendor** | Create/manage products, view orders for their products |
| **Customer** | Browse products, place orders, manage their profile |

---

## 2. Architecture

### High-Level Architecture

```
Client (Mobile/Web)
        │
        ▼
   ASP.NET Core API
        │
   ┌────┴────────────────────┐
   │  Middleware Pipeline     │
   │  1. HTTPS Redirect       │
   │  2. Authentication (JWT) │
   │  3. Token Blacklist      │
   │  4. Authorization        │
   └────┬────────────────────┘
        │
   ┌────▼────────────────────┐
   │      Controllers         │
   │  Auth / Product /        │
   │  Profile / Order /       │
   │  Category                │
   └────┬────────────────────┘
        │
   ┌────▼────────────────────┐
   │      Services            │
   │  AuthService             │
   │  ProfileService          │
   │  OrderService            │
   │  CartService (memory)    │
   │  TokenBlacklistService   │
   └────┬────────────────────┘
        │
   ┌────▼────────────────────┐
   │   Unit of Work           │
   │   + Repositories         │
   └────┬────────────────────┘
        │
   ┌────▼────────────────────┐
   │   SQL Server (EF Core)   │
   └─────────────────────────┘
```

### Design Patterns Used

- **Repository Pattern** — abstracts database access from business logic
- **Unit of Work** — coordinates multiple repositories in a single transaction
- **Service Layer** — encapsulates all business logic
- **DTO Pattern** — decouples API responses from database models
- **Options Pattern** — type-safe configuration via `IOptions<T>`

### Data Flow

```
Request → Middleware → Controller → Service → Repository → Database
Response ← Controller ← Service ← Repository ← Database
```

---

## 3. Tech Stack

### Backend

| Technology | Version | Purpose |
|-----------|---------|---------|
| ASP.NET Core | 8.0 | Web API framework |
| Entity Framework Core | 8.0 | ORM & database access |
| ASP.NET Core Identity | 8.0 | User management & roles |
| JWT Bearer | Latest | Token-based authentication |
| AutoMapper | Latest | Object-to-object mapping |
| Serilog | Latest | Structured logging |
| libphonenumber-csharp | Latest | Phone number validation |

### Database

| Technology | Purpose |
|-----------|---------|
| SQL Server (LocalDB) | Primary database |
| EF Core Migrations | Schema management |

### Frontend

> EC_V2 is a pure backend API. Frontend is handled by the client application (mobile app, web app) that consumes this API.

---

## 4. Installation & Setup

### Requirements

- .NET 8 SDK
- SQL Server or SQL Server LocalDB
- Visual Studio 2022 / VS Code / Rider

### Environment Variables / Configuration

Create or update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EC_V2;Trusted_Connection=True;"
  },
  "JwtSettings": {
    "Secret": "your-secret-key-minimum-32-characters-long",
    "Issuer": "EC_V2",
    "Audience": "EC_V2",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  }
}
```

### Installation Steps

```bash
# 1. Clone the repository
git clone https://github.com/your-username/EC_V2.git
cd EC_V2

# 2. Restore packages
dotnet restore

# 3. Apply database migrations
dotnet ef database update

# 4. Run the application
dotnet run
```

### Running Locally

```bash
dotnet run --project EC_V2
```

Swagger UI will be available at: `https://localhost:7208/swagger`

### Building for Production

```bash
dotnet publish -c Release -o ./publish
```

---

## 5. Project Structure

```
EC_V2/
├── Controllers/
│   ├── AuthController.cs         # Registration, login, OTP, tokens
│   ├── ProfileController.cs      # Vendor/Customer profile CRUD
│   ├── ProductController.cs      # Product CRUD + pagination/filtering
│   ├── CategoriesController.cs   # Category CRUD
│   └── OrderController.cs        # Order creation, status, cancellation
│
├── Models/
│   ├── AppUser.cs                # Extended Identity user
│   ├── Product.cs                # Product entity
│   ├── Category.cs               # Category with self-referencing hierarchy
│   ├── Order.cs                  # Order with financial breakdown
│   ├── OrderItem.cs              # Order line item (price snapshot)
│   ├── Coupon.cs                 # Discount coupon
│   ├── RefreshToken.cs           # Hashed refresh tokens
│   ├── OtpCode.cs                # Hashed OTP codes
│   ├── VendorProfile.cs          # Vendor store details
│   ├── CustomerProfile.cs        # Customer shipping info
│   └── Enums/
│       ├── OrderStatus.cs        # Pending, Confirmed, Shipped, etc.
│       └── DiscountType.cs       # Percentage, Fixed
│
├── DTOs/
│   ├── Auth/                     # RegisterDto, LoginDto, AuthResponseDto, etc.
│   ├── Product/                  # ProductDto, AddProductDto, ProductQueryDto
│   ├── Category/                 # CategoryDto, AddCategoryDto
│   ├── Profile/                  # VendorProfileDto, CustomerProfileDto, etc.
│   ├── Order/                    # OrderDto, CreateOrderDto, OrderItemDto
│   ├── Cart/                     # CartDto, CartItem, AddToCartDto
│   ├── PagedResult.cs            # Generic cursor pagination wrapper
│   └── ServiceResult.cs          # Generic service response wrapper
│
├── Repositories/
│   ├── Interfaces/
│   │   ├── IGenericRepository.cs
│   │   ├── IUnitOfWork.cs
│   │   ├── IProductRepository.cs
│   │   ├── ICategoryRepository.cs
│   │   ├── IOrderRepository.cs
│   │   ├── ICouponRepository.cs
│   │   ├── IVendorProfileRepository.cs
│   │   └── ICustomerProfileRepository.cs
│   └── Implementations/
│       ├── GenericRepository.cs
│       ├── UnitOfWork.cs
│       ├── ProductRepository.cs  # Includes cursor pagination
│       ├── CategoryRepository.cs
│       ├── OrderRepository.cs
│       ├── CouponRepository.cs
│       ├── VendorProfileRepository.cs
│       └── CustomerProfileRepository.cs
│
├── Services/
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── IProfileService.cs
│   │   ├── IOrderService.cs
│   │   ├── IProductServices.cs
│   │   ├── ICartService.cs
│   │   └── ITokenBlacklistService.cs
│   └── Implementations/
│       ├── AuthServices.cs       # OTP, JWT, refresh tokens
│       ├── ProfileService.cs     # Profile creation + role assignment
│       ├── OrderService.cs       # Order creation, tax, coupons
│       ├── ProductServices.cs    # Paged product queries
│       ├── CartService.cs        # In-memory cart (ConcurrentDictionary)
│       └── TokenBlacklistService.cs  # Singleton JTI blacklist
│
├── Middlewares/
│   └── TokenBlacklistMiddleware.cs   # Checks blacklisted JTIs on every request
│
├── Mapping/
│   └── MappingProfile.cs         # AutoMapper configuration
│
├── Settings/
│   └── JWTSettings.cs            # Typed JWT configuration
│
├── Data/
│   ├── AppDbContext.cs            # EF Core DbContext (extends IdentityDbContext)
│   └── DbSeeder.cs                # Seeds roles and admin user on startup
│
├── Migrations/                    # EF Core migration files
├── Logs/                          # Serilog log files
├── appsettings.json
└── Program.cs                     # Service registration & middleware pipeline
```

---

## 6. Core Features

### 6.1 Phone + OTP Authentication

**How it works:**

1. User registers with phone number (Egypt/Saudi only)
2. Server generates a 4-digit OTP, hashes it with bcrypt, stores with 5-min expiry
3. OTP returned in response (dev mode) — in production, sent via SMS
4. User verifies OTP → account activated
5. Login follows same OTP flow → returns JWT access token + refresh token

**Key files:** `AuthServices.cs`, `AuthController.cs`, `OtpCode.cs`

**Security:**
- OTPs hashed with `PasswordHasher` (bcrypt + random salt)
- Refresh tokens hashed with SHA256 (deterministic for DB lookup)
- Phone validation via `libphonenumber-csharp`

---

### 6.2 JWT + Token Blacklist

**How it works:**

- Access token expires in **15 minutes**, signed with HMAC-SHA256
- Refresh token expires in **7 days**, stored hashed in DB
- On logout: access token JTI added to in-memory blacklist, refresh token revoked in DB
- `TokenBlacklistMiddleware` checks every request's JTI against the blacklist

**Token rotation:** Every refresh generates a new refresh token and invalidates the old one.

**Key files:** `AuthServices.cs`, `TokenBlacklistService.cs`, `TokenBlacklistMiddleware.cs`

---

### 6.3 Role-Based Authorization

Three roles seeded automatically on startup:

| Role | Permissions |
|------|------------|
| `Admin` | All orders, all users, system management |
| `Vendor` | Create products, view orders for their products, manage coupons |
| `Customer` | Place orders, view own orders, manage own profile |

Roles are assigned automatically when profiles are created:
- `POST /api/profile/vendor` → assigns `Vendor` role
- `POST /api/profile/customer` → assigns `Customer` role

---

### 6.4 Cursor-Based Pagination

**How it works:**

1. Client requests products with optional `cursor`, `pageSize`, `search`, `categoryId`, `minPrice`, `maxPrice`, `sortBy`, `sortOrder`
2. Server fetches `pageSize + 1` items
3. If `count > pageSize` → `hasMore = true`, last item removed, cursor encoded as Base64 JSON
4. Client uses `nextCursor` in next request to get next page

**Cursor encoding:**
```
{"id": 50} → Base64 → eyJpZCI6NTB9
```

**Key files:** `ProductRepository.cs`, `ProductQueryDto.cs`, `PagedResult.cs`

---

### 6.5 Order System

**How it works:**

1. Frontend manages cart locally (localStorage)
2. On checkout, frontend sends `CreateOrderDto` with items array
3. Server validates all products exist and have sufficient stock
4. Calculates: `SubTotal → Tax (14%) → Coupon Discount → GrandTotal`
5. Deducts stock from each product
6. Saves order + order items (with price snapshot)
7. Returns full `OrderDto`

**Coupon validation checks:**
- IsActive
- Not expired
- SubTotal >= MinOrderAmount
- UsedCount < MaxUses

**Key files:** `OrderService.cs`, `OrderController.cs`, `Order.cs`, `OrderItem.cs`

---

### 6.6 Coupon System

Two discount types:
- `Percentage` — e.g. 10% off subtotal
- `Fixed` — e.g. 50 EGP off subtotal

Coupons track `UsedCount` and support `MaxUses` (null = unlimited).

---

## 7. API Documentation

### Auth Endpoints

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/auth/register` | ❌ | Register with phone number |
| POST | `/api/auth/verify-otp` | ❌ | Verify registration OTP |
| POST | `/api/auth/login` | ❌ | Request login OTP |
| POST | `/api/auth/verify-login` | ❌ | Verify login OTP → get tokens |
| POST | `/api/auth/refresh` | ❌ | Rotate refresh token |
| POST | `/api/auth/logout` | ✅ | Invalidate tokens |

#### Example: Register
```http
POST /api/auth/register
Content-Type: application/json

{
  "phone": "+201012345678",
  "firstName": "Ahmed",
  "lastName": "Mohamed"
}
```

```json
{
  "message": "OTP sent to your phone",
  "otpCode": "5678"
}
```

#### Example: Verify Login → Get Tokens
```http
POST /api/auth/verify-login
Content-Type: application/json

{
  "phone": "+201012345678",
  "otp": "5678"
}
```

```json
{
  "accessToken": "eyJhbGci...",
  "refreshToken": "De7xjcDH...",
  "accessTokenExpiry": "2026-02-23T11:47:01Z"
}
```

---

### Product Endpoints

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/api/product` | ✅ | Get products (paginated, filtered) |
| GET | `/api/product/{id}` | ✅ | Get product by ID |
| POST | `/api/product` | ✅ Vendor | Create product |
| PUT | `/api/product/{id}` | ✅ Vendor | Update product |
| DELETE | `/api/product/{id}` | ✅ Vendor | Delete product |

#### Example: Get Products with Filtering
```http
GET /api/product?search=iphone&minPrice=10000&maxPrice=50000&sortBy=price&sortOrder=asc&pageSize=5
Authorization: Bearer {token}
```

```json
{
  "items": [
    {
      "name": "iPhone 13",
      "price": 30000,
      "stock": 5,
      "categories": [...]
    }
  ],
  "nextCursor": "eyJpZCI6MTB9",
  "hasMore": true
}
```

---

### Profile Endpoints

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/profile/vendor` | ✅ | Create vendor profile |
| POST | `/api/profile/customer` | ✅ | Create customer profile |
| GET | `/api/profile/vendor` | ✅ | Get my vendor profile |
| GET | `/api/profile/customer` | ✅ | Get my customer profile |
| PUT | `/api/profile/vendor` | ✅ | Update vendor profile |
| PUT | `/api/profile/customer` | ✅ | Update customer profile |

---

### Order Endpoints

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/order` | ✅ Customer | Create order |
| GET | `/api/order` | ✅ Customer | Get my orders |
| GET | `/api/order/vendor` | ✅ Vendor | Get orders for my products |
| GET | `/api/order/all` | ✅ Admin | Get all orders |
| GET | `/api/order/{id}` | ✅ Any | Get order by ID |
| PUT | `/api/order/{id}/status` | ✅ Vendor/Admin | Update order status |
| PUT | `/api/order/{id}/cancel` | ✅ Customer | Cancel order |

#### Example: Create Order
```http
POST /api/order
Authorization: Bearer {customer_token}
Content-Type: application/json

{
  "shippingAddress": "123 Cairo Street, Cairo",
  "couponCode": "SUMMER20",
  "items": [
    { "productId": 1, "quantity": 2 },
    { "productId": 3, "quantity": 1 }
  ]
}
```

```json
{
  "id": 1,
  "status": "Pending",
  "subTotal": 65000,
  "taxAmount": 9100,
  "discountAmount": 13000,
  "grandTotal": 61100,
  "items": [...]
}
```

---

## 8. Database Schema

### Core Tables

#### AspNetUsers (AppUser)
| Column | Type | Description |
|--------|------|-------------|
| Id | string (GUID) | Primary key |
| PhoneNumber | nvarchar | Used as username |
| FirstName | nvarchar | User first name |
| LastName | nvarchar | User last name |
| CreatedAt | datetime2 | Account creation date |

#### Products
| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| Name | nvarchar | Product name |
| Description | nvarchar | Product description |
| Price | decimal(18,2) | Product price |
| Stock | int | Available quantity |
| ImageUrl | nvarchar (null) | Optional image URL |
| VendorId | nvarchar (FK) | References AspNetUsers |

#### Orders
| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| CustomerId | nvarchar (FK) | References AspNetUsers |
| Status | int | Enum: Pending/Confirmed/etc. |
| SubTotal | decimal(18,2) | Before tax and discount |
| TaxAmount | decimal(18,2) | 14% VAT |
| DiscountAmount | decimal(18,2) | Coupon discount |
| GrandTotal | decimal(18,2) | Final amount |
| CouponId | int (FK, null) | References Coupons |
| ShippingAddress | nvarchar | Delivery address |

#### OrderItems
| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| OrderId | int (FK) | References Orders |
| ProductId | int (FK) | References Products |
| ProductName | nvarchar | Snapshot at purchase time |
| UnitPrice | decimal(18,2) | Snapshot at purchase time |
| Quantity | int | Quantity ordered |

#### Coupons
| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| Code | nvarchar | Unique coupon code |
| Type | int | Percentage or Fixed |
| Value | decimal(18,2) | Discount amount/percent |
| MinOrderAmount | decimal(18,2) | Minimum cart value |
| ExpiryDate | datetime2 | Expiry date |
| MaxUses | int (null) | null = unlimited |
| UsedCount | int | Times used so far |

#### RefreshTokens
| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| TokenHash | nvarchar | SHA256 hashed token |
| UserId | nvarchar (FK) | References AspNetUsers |
| CreatedAt | datetime2 | Issue date |
| ExpiresAt | datetime2 | Expiry date |
| RevokedAt | datetime2 (null) | Revocation date |
| IsUsed | bit | Whether token was used |

### Relationships

```
AppUser ──< RefreshTokens
AppUser ──< OtpCodes
AppUser ──1 VendorProfile
AppUser ──1 CustomerProfile
AppUser ──< Orders (as Customer)
AppUser ──< Products (as Vendor)
Product >──< Categories (many-to-many via ProductCategories)
Category >── Category (self-referencing ParentId)
Order ──< OrderItems
Order >── Coupon (optional)
OrderItem >── Product
```

---

## 9. Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EC_V2;Trusted_Connection=True;"
  },
  "JwtSettings": {
    "Secret": "0acd72a3c67b8158fbd874c9063cd6c9ee4168ca",
    "Issuer": "EC_V2",
    "Audience": "EC_V2",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "Serilog": {
    "MinimumLevel": "Information"
  }
}
```

### Service Lifetimes

| Service | Lifetime | Reason |
|---------|----------|--------|
| `AppDbContext` | Scoped | One per request |
| `IUnitOfWork` | Scoped | Wraps DbContext |
| `IAuthService` | Scoped | Uses DbContext |
| `IProfileService` | Scoped | Uses DbContext |
| `IOrderService` | Scoped | Uses DbContext |
| `ITokenBlacklistService` | **Singleton** | In-memory dictionary must persist across requests |
| `ICartService` | **Singleton** | In-memory cart must persist across requests |

---

## 10. Usage Examples

### Full Registration Flow

```bash
# Step 1: Register
curl -X POST https://localhost:7208/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"phone": "+201012345678", "firstName": "Ahmed", "lastName": "Mohamed"}'

# Step 2: Verify OTP
curl -X POST https://localhost:7208/api/auth/verify-otp \
  -H "Content-Type: application/json" \
  -d '{"phone": "+201012345678", "otp": "5678", "purpose": "Registration"}'

# Step 3: Login
curl -X POST https://localhost:7208/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"phone": "+201012345678"}'

# Step 4: Verify Login → get tokens
curl -X POST https://localhost:7208/api/auth/verify-login \
  -H "Content-Type: application/json" \
  -d '{"phone": "+201012345678", "otp": "1234"}'
```

### Full Order Flow

```bash
# Step 1: Create customer profile
curl -X POST https://localhost:7208/api/profile/customer \
  -H "Authorization: Bearer {token}" \
  -d '{"shippingAddress": "123 Cairo St", "phoneNumber": "01012345678"}'

# Step 2: Browse products
curl -X GET "https://localhost:7208/api/product?search=iphone&maxPrice=40000" \
  -H "Authorization: Bearer {token}"

# Step 3: Place order
curl -X POST https://localhost:7208/api/order \
  -H "Authorization: Bearer {token}" \
  -d '{
    "shippingAddress": "123 Cairo St",
    "couponCode": "SAVE10",
    "items": [{"productId": 1, "quantity": 1}]
  }'
```

---

*EC_V2 API — Built with ASP.NET Core 8 | February 2026*
