# 05 - Idempotency & Order State Machine

## 1. Idempotency Standard
- Critical mutation endpoints (e.g. `POST /api/v1/orders`) must accept an `Idempotency-Key` HTTP header.
- **Protocol:**
  1. Check Redis / Database for existing `Idempotency-Key`.
  2. If found with status `Completed`, immediately return the cached response (prevent duplicate order creation).
  3. If found with status `Processing`, return `409 Conflict` (request in-flight).
  4. If not found, save key with status `Processing`, execute business transaction, update key status to `Completed` with response payload, and set TTL (e.g., 24 hours).

## 2. Order Lifecycle State Machine
- Orders must strictly follow defined state transitions:
  - `Draft` → `PendingPayment` → `Confirmed` → `Processing` → `Shipped` → `Delivered`
  - Cancellation allowed ONLY from `PendingPayment` or `Confirmed`. Once `Shipped`, cancellation is forbidden (requires Return flow).
- State transitions must be validated within Domain methods (e.g., `order.Cancel()`), not scattered across controller code.
