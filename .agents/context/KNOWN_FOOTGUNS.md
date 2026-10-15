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

### 6. Multi-Tenant Refresh Token Resolution (`WHERE 0 = 1`)
- **Trap:** `RefreshToken` implements `ITenantScopedEntity`. When client sends an unauthenticated `POST /api/auth/refresh` request without `X-Tenant-Id` or JWT, `CurrentTenantService.TenantId` is `null`. EF Core evaluates non-nullable `TenantId == null` as statically false, generating `WHERE 0 = 1` in SQL and failing valid refresh tokens!
- **Defense:** Apply `.IgnoreQueryFilters()` in `AuthService.RefreshTokenAsync()` on `RefreshTokens`. The high-entropy refresh token itself securely identifies both the user and their tenant.

