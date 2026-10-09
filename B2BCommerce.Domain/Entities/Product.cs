using B2BCommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace B2BCommerce.Domain.Entities
{
    public class Product : BaseEntity, ITenantScopedEntity
    {
        public int TenantId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual Category Category { get; set; } = null!;

    }
}
