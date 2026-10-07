# 02 - Clean Architecture & Design Principles

## Core Invariants

1. **Dependency Inversion Principle (DIP):**
   - High-level modules must not depend on low-level modules. Both must depend on abstractions.
   - `Domain` is at the center with ZERO dependencies.
   - `Application` depends ONLY on `Domain`.
   - `Infrastructure` implements abstractions from `Application` and uses `Domain` entities.
   - `Api` configures DI and exposes HTTP endpoints.

2. **No Business Logic in Controllers:**
   - Controllers only:
     - Receive HTTP requests.
     - Delegate execution to Application handlers / services.
     - Translate application responses (`Result<T>`) into HTTP status codes (200, 201, 400, 404, 409).

3. **Result Pattern (`Result<T>`):**
   - Expected domain and business failures (e.g. "Insufficient Stock", "Invalid Discount Code") must not throw expensive C# exceptions.
   - Use explicit `Result<T>` types representing `IsSuccess`, `Value`, `Error`.
   - Only truly unhandled runtime exceptions trigger 500 Internal Server Error.

4. **Dependency Injection (DI) Lifecycles:**
   - `Scoped`: DbContext, TenantContext, CurrentUserService (lives for the duration of a single HTTP request).
   - `Transient`: Lightweight stateless services, validators, calculation strategies.
   - `Singleton`: Caching engines, Token validators, Background workers.
