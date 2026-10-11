using B2BCommerce.Application.Common.Interfaces;

namespace B2BCommerce.Api.Services;

public class CurrentTenantService : ICurrentTenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

        public int? TenantId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return null;

            // 1. Priority: Check JWT Claims if user is authenticated (Spoofing prevention)
            var tenantClaim = httpContext.User?.FindFirst("tenant_id")?.Value;
            if (!string.IsNullOrEmpty(tenantClaim) && int.TryParse(tenantClaim, out var claimTenantId))
            {
                return claimTenantId;
            }

            // 2. Fallback: Check custom header for unauthenticated endpoints (e.g. X-Tenant-Id: 101)
            if (httpContext.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader) &&
                int.TryParse(tenantHeader, out var headerTenantId))
            {
                return headerTenantId;
            }

            return null;
        }
    }

}
