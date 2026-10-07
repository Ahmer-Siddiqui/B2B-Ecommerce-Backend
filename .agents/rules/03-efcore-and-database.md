# 03 - EF Core & Database Engineering Standards

## 1. Multi-Tenancy via Global Query Filters
- Every tenant-scoped entity (`ITenantScopedEntity`) must have an EF Core Global Query Filter:
  ```csharp
  modelBuilder.Entity<TEntity>()
      .HasQueryFilter(e => e.TenantId == _currentTenantId && !e.IsDeleted);
  ```
- This ensures database-level zero-leakage without relying on manual `.Where(x => x.TenantId == ...)` in application queries.
- Disabling filters via `.IgnoreQueryFilters()` is strictly restricted to platform SuperAdmin administrative queries.

## 2. ChangeTracker & Audit Interceptors
- Use `SaveChangesInterceptor` to intercept entity state mutations before committing to the database.
- Automatically populate:
  - `CreatedAt` and `CreatedBy` on `EntityState.Added`.
  - `UpdatedAt` and `UpdatedBy` on `EntityState.Modified`.
  - `TenantId` stamp on new tenant-scoped entities.
- Generate `AuditLog` records containing `OldValues` and `NewValues` in serialized JSON for tracked business entities.

## 3. Query Optimization & Performance
- **Read Queries:** Always apply `.AsNoTracking()` to read-only queries to eliminate ChangeTracker memory overhead.
- **Selective Projection:** Use `.Select(x => new Dto { ... })` instead of fetching full entity graphs when only a subset of fields is needed.
- **N+1 Prevention:** Never execute database queries inside `foreach` loops. Use explicit `.Include()` / `.ThenInclude()` or projection joins.
- **Composite Indexes:** Always index `(TenantId, Id)` or `(TenantId, SKU)` or `(TenantId, Status)` for fast multi-tenant lookup.
