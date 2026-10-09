using B2BCommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace B2BCommerce.Domain.Entities
{
    public class OrderItem : BaseEntity, ITenantScopedEntity
    {
        public int TenantId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
