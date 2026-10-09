using B2BCommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace B2BCommerce.Domain.Entities
{
    public class Category : BaseEntity, ITenantScopedEntity
    {
        public int TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    }
}
