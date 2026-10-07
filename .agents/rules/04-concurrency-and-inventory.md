# 04 - Concurrency & Inventory Control Standards

## The High-Stakes Concurrency Problem
When multiple users attempt to purchase the same product concurrently:
- If stock is `1`, and Request A and Request B read stock as `1` simultaneously, both may deduct `1`, resulting in stock `-1` (Overselling / Data Corruption).

## Architectural Standards for Inventory:
1. **Optimistic Concurrency Control:**
   - Use `[Timestamp]` / `RowVersion` (e.g. `byte[] RowVersion { get; set; }`) on `InventoryItem`.
   - When a concurrency conflict occurs (`DbUpdateConcurrencyException`), retry or return a clean conflict response (`409 Conflict: Stock changed during checkout`).
2. **Database CHECK Constraints:**
   - In SQL Server DDL / EF Core Fluent API:
     ```csharp
     builder.ToTable(t => t.HasCheckConstraint("CK_Inventory_QuantityAvailable", "QuantityAvailable >= 0"));
     ```
   - Even if application code has a bug, the database engine physically rejects negative stock.
3. **Atomic Stock Decrement:**
   - Where justified, use atomic update statements:
     ```sql
     UPDATE InventoryItems 
     SET QuantityAvailable = QuantityAvailable - @Quantity 
     WHERE Id = @Id AND QuantityAvailable >= @Quantity
     ```
4. **Transactions:**
   - Order creation and inventory deduction must execute within a single database transaction (`using var transaction = await dbContext.Database.BeginTransactionAsync()`). If anything fails, rollback completely.
