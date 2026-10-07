# Known Footguns & Anti-Patterns (B2B E-Commerce & EF Core)

### 1. Cross-Tenant Data Leakage
- **Trap:** Writing raw SQL or disabling filters (`IgnoreQueryFilters()`) in business queries without explicitly scoping by `TenantId`.
- **Defense:** Never disable query filters outside platform-level SuperAdmin services.

### 2. Negative Stock Concurrency Bug
- **Trap:** Checking `if (stock >= requestedQty)` in memory, then issuing `stock -= requestedQty` and `SaveChanges()`. Under 100 concurrent requests, stock will drop below zero!
- **Defense:** Optimistic Concurrency via `RowVersion` + Database `CHECK (QuantityAvailable >= 0)` constraint + atomic SQL deductions.

### 3. Duplicate Order Creation on Network Retry
- **Trap:** Client sends `POST /api/orders`, server takes 3 seconds, client times out and retries. Two orders are placed and inventory is deducted twice.
- **Defense:** Idempotency Key header checked before transaction execution.

### 4. EF Core Tracking Overhead in Read APIs
- **Trap:** Using default tracking queries on large catalog listings (`GET /api/v1/products`). Memory spikes and performance degrades.
- **Defense:** Always use `.AsNoTracking()` for read queries.

### 5. Circular JSON Reference on Navigation Properties
- **Trap:** `Order` points to `Customer`, and `Customer` has `List<Order>`. Serializing to JSON causes a runtime cyclical error or massive response explosion.
- **Defense:** Never return EF Core entities directly from Controllers. Always project to DTOs.
