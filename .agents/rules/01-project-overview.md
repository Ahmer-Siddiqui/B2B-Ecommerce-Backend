# 01 - Project Overview & Business Domain

## Domain: Multi-Tenant B2B E-Commerce & Order Management SaaS

### Business Model
Multiple businesses (Tenants) run their B2B operations on a single unified platform. Each company has its own isolated catalog, customers, inventory levels, order workflows, and staff users.

### Actors & Roles
1. **SuperAdmin:** Platform-level administrator. Manages tenant creation, subscription status, system health.
2. **TenantAdmin:** Company administrator. Manages company staff, warehouse settings, categories, and business rules.
3. **Manager:** Oversees inventory, approves special order discounts, views analytics.
4. **SalesEmployee:** Places orders on behalf of corporate customers, manages customer accounts, tracks shipments.
5. **Customer:** Corporate buyer. Views company-specific catalog/pricing, places orders, views invoice history.

### The 10-Phase Roadmap
- **Phase 1:** Solution Structure, Clean Architecture Boundaries, Domain Base Entities, Multi-Tenancy Core.
- **Phase 2:** Authentication (JWT, Refresh Tokens) & Authorization (RBAC, Claims).
- **Phase 3:** Tenant Context & Global Query Filters (Zero-leak data isolation).
- **Phase 4:** Product Catalog & Categories (Search, Dynamic Filtering, Pagination).
- **Phase 5:** Inventory Management (Concurrency, Race Condition Prevention, Stock Reservations).
- **Phase 6:** Orders Lifecycle & State Machine (Pending → Confirmed → Shipped → Delivered, Cancellation rules).
- **Phase 7:** Idempotency & Payments Abstraction (Prevent duplicate charges/orders on network retry).
- **Phase 8:** Audit Logging & Interceptors (ChangeTracker old/new diffs in JSON).
- **Phase 9:** Redis Caching & Query Optimization (Cache-aside, index tuning, AsNoTracking).
- **Phase 10:** Testing (xUnit, Integration Tests), Dockerization, and Documentation.
