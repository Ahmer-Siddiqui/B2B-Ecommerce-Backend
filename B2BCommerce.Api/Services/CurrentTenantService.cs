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

            // 1. Check custom header (e.g. X-Tenant-Id: 101)
            if (httpContext.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader) &&
                int.TryParse(tenantHeader, out var tenantId))
            {
                return tenantId;
            }

            // 2. Later we will also read from JWT Claims here
            return null;
        }
    }
}
