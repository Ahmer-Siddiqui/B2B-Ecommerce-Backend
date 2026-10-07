# B2B E-Commerce & Order Management SaaS — Agent Instructions & Operating Rules

Welcome to the **B2B E-Commerce Backend** codebase. This document defines the architectural rules, coding standards, and operational workflows for AI coding assistants.

---

## 1. Project Overview & Business Domain
- **Project Name:** B2B E-Commerce & Order Management SaaS (`B2BCommerceBackend`)
- **Domain:** Multi-Tenant B2B E-Commerce platform enabling multiple businesses to operate their own product catalog, inventory, order lifecycle, invoicing, and customer accounts under strict data isolation.
- **Tenant Isolation Guarantee:** Every tenant-owned entity (`Product`, `Order`, `InventoryItem`, `Customer`, `Invoice`, etc.) is tagged with a `TenantId`. Data leakage across tenants is treated as a Critical Severity P0 defect.
- **Core Modules:**
  - **Auth & Identity:** JWT Access Tokens, Refresh Token rotation, Password hashing, Current User context.
  - **Authorization:** Role-Based (SuperAdmin, TenantAdmin, Manager, SalesEmployee, Customer) & Permission-based access control.
  - **Multi-Tenancy:** Automated isolation via EF Core Global Query Filters and Tenant Resolution Middleware.
  - **Catalog:** Categories, Products (SKU, pricing, cost, specifications).
  - **Inventory & Concurrency:** Concurrency control, stock reservations, race-condition prevention during checkout.
  - **Orders & State Machine:** Orders, OrderItems, totals, discounts, status flow (`Pending` → `Confirmed` → `Processing` → `Shipped` → `Delivered`), cancellation rules.
  - **Idempotency:** Idempotency keys for safe retries on order creation & payments.
  - **Audit Logging:** Automatic audit capture via EF Core ChangeTracker (`OldValues` vs `NewValues` in JSON).

---

## 2. Tech Stack & Dependencies
- **Runtime & Language:** .NET 10.0 (C# 13)
- **Framework:** ASP.NET Core Web API (Controllers)
- **ORM & Data Access:** Entity Framework Core, SQL Server
- **Caching & Key-Value:** Redis (distributed caching, idempotency cache)
- **Testing:** xUnit, FluentAssertions, Moq/NSubstitute, WebApplicationFactory
- **Containerization:** Docker & Docker Compose (API + SQL Server + Redis)

---

## 3. Clean Architecture Layering & Invariants

The solution follows a **strict Clean Architecture** structure:

```
[ HTTP Client / Swagger / Postman ]
                │
                ▼
┌─────────────────────────────────────────────────────────────┐
│ 1. Presentation Layer (B2BCommerce.Api)                     │
│    - Controllers, Middleware, Filters, Program.cs           │
└─────────────────────────────────────────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────────────────────────┐
│ 2. Application Layer (B2BCommerce.Application)             │
│    - Use Cases, DTOs, Interfaces, Validators, Business Logic│
└─────────────────────────────────────────────────────────────┘
         ▲                               ▲
         │                               │
┌───────────────────────────┐   ┌─────────────────────────────┐
│ 3. Infrastructure Layer   │   │ 4. Domain Layer             │
│    (B2BCommerce.          │   │    (B2BCommerce.Domain)     │
│     Infrastructure)       │──►│    - Pure C# Entities,      │
│    - EF Core, SQL Server, │   │      Enums, Exceptions,     │
│      JWT, Redis, Emails   │   │      Domain Rules (ZERO     │
└───────────────────────────┘   │      external dependencies) │
                                └─────────────────────────────┘
```

### ⚠️ Strict Layering Rules:
1. **Domain has ZERO external dependencies.** Never install EF Core, ASP.NET Core, or third-party packages in `B2BCommerce.Domain`.
2. **Controllers have ZERO business logic.** Controllers only accept HTTP requests, call the Application layer, and return typed HTTP results.
3. **Application Layer depends ONLY on Domain.** It defines interfaces (`IApplicationDbContext`, `ICurrentTenantService`, `IEmailService`); it never directly references Infrastructure.
4. **Infrastructure implements Application interfaces.** Concrete database queries, SQL connections, and third-party APIs live here.
5. **No Blind Generic Repositories.** EF Core's `DbContext` and `DbSet<T>` already provide Unit of Work & Repository capabilities. Avoid wrapping every query in unnecessary repository abstractions unless decoupling for specific testing requirements.

---

## 4. Coding Standards & Conventions

### A. Socratic Mentorship & Implementation Rule
- **CRITICAL MENTORSHIP CONTRACT:** The AI assistant must **NEVER** write unsolicited implementation code unless the developer explicitly asks ("Code likho" or "Implementation do").
- Always explain the **Requirement** and **The WHY (Architectural reasoning & trade-offs)** first.
- Give tasks to the developer to implement, review their code, highlight bugs/concurrency flaws, and let them fix it.

### B. Error Handling & API Responses
- Use custom Domain/Application exceptions or the **Result Pattern (`Result<T>`)**.
- Global Exception Middleware translates unhandled errors to standard RFC 7807 `ProblemDetails`.

### C. Multi-Tenancy & Audit Logging
- All tenant-owned entities must implement `ITenantScopedEntity`.
- Never execute raw SQL without verifying tenant filtering.
- Soft-delete only (`IsDeleted = true`). Physical `DELETE` is prohibited on business entities.
