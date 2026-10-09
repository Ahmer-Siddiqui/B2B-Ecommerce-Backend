using B2BCommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace B2BCommerce.Infrastructure.Persistence.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.Property(i => i.RowVersion)
            .IsRowVersion();

        // Database level check constraint
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_InventoryItems_QuantityAvailable",
            "QuantityAvailable >= 0"));
    }
}
