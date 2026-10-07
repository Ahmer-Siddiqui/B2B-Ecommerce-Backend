# 07 - Verification & Quality Gates (Adversarial Self-Audit)

Before considering any feature or PR complete, run through these 5 mandatory quality gates:

1. **Clean Architecture Boundary Audit:**
   - Did any EF Core or external package leak into `B2BCommerce.Domain`?
   - Is there any business logic residing in `Controllers`?
2. **Tenant Isolation Verification:**
   - Does every new entity inherit `ITenantScopedEntity`?
   - Is the Global Query Filter configured in `DbContext`?
3. **Concurrency & Mutation Safety:**
   - Are stock updates or balance adjustments protected against race conditions?
   - Are mutative operations wrapped in database transactions?
4. **Boundary & Null Safety:**
   - Are empty collections, zero values, negative quantities, and nulls validated before hitting domain logic?
5. **Zero Speculative Footprint:**
   - Only code required by the current approved feature specification should be touched.
