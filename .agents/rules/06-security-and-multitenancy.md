# 06 - Security & Multi-Tenancy Invariants

## 1. Authentication & Token Standards
- **Access Tokens:** Short-lived JWTs (15 minutes). Contains `sub` (UserId), `tenant_id`, `role`, `email`.
- **Refresh Tokens:** Long-lived cryptographically secure random tokens stored in database with `ExpiryDate`, `IsRevoked`, and `ReplacedByToken` (rotation tracking).
- When a refresh token is used, it is immediately revoked and a new pair is issued (Refresh Token Rotation). If an already revoked token is submitted, revoke all tokens for that user (compromise detection).

## 2. Multi-Tenancy Resolution
- Incoming requests resolve tenant context via `TenantResolutionMiddleware`:
  - From authenticated JWT `tenant_id` claim, OR
  - From `X-Tenant-Id` header (for public/registration endpoints only).
- Verified tenant is scoped into `ICurrentTenantService`.

## 3. IDOR Prevention (Insecure Direct Object References)
- Never rely solely on entity `Id`. Always query through the tenant context.
- Even if a customer knows `OrderId = 452`, if `OrderId = 452` belongs to another customer or tenant, return `404 Not Found` (never `403 Forbidden`, to avoid leaking record existence).
