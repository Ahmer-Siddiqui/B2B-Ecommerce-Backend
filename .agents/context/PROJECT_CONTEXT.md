# Project Context: Multi-Tenant B2B E-Commerce & Order Management SaaS

## 1. Executive Summary
Enterprise-grade Multi-Tenant B2B E-Commerce & Order Management SaaS backend built with ASP.NET Core Web API, EF Core, SQL Server, Redis, and Docker.

## 2. Solution Structure
- `src/B2BCommerce.Domain/`: Enterprise business models, enums, interfaces, exceptions.
- `src/B2BCommerce.Application/`: Use cases, CQRS, DTOs, validations, interfaces.
- `src/B2BCommerce.Infrastructure/`: EF Core `ApplicationDbContext`, Migrations, JWT token generator, Redis client.
- `src/B2BCommerce.Api/`: REST Controllers, Middlewares, Program.cs, Swagger configuration.
- `tests/B2BCommerce.UnitTests/`: Domain and business rule tests.
- `tests/B2BCommerce.IntegrationTests/`: WebApplicationFactory end-to-end API tests.

## 3. Database & Multi-Tenancy Strategy
- **Strategy:** Shared Database, Shared Schema with Discriminator (`TenantId`).
- **Enforcement:** EF Core Global Query Filters (`e => e.TenantId == _currentTenantId && !e.IsDeleted`).
- **Audit:** Automatic `AuditLogs` captured via EF Core `SaveChangesInterceptor`.
