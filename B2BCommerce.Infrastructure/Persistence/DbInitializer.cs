using B2BCommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerce.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(B2BCommerceDbContext context)
    {
        // Check if database already has tenants
        if (await context.Tenants.AnyAsync())
        {
            return;
        }

        var sampleTenants = new List<Tenant>
        {
            new()
            {
                Name = "Acme Industrial Supplies",
                Code = "ACME",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Apex Global Logistics",
                Code = "APEX",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Tenants.AddRange(sampleTenants);
        await context.SaveChangesAsync();
    }
}
