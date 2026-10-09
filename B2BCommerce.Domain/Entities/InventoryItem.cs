using B2BCommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace B2BCommerce.Domain.Entities
{
    public class InventoryItem : BaseEntity, ITenantScopedEntity
    {
        public int TenantId { get; set; }
        public int ProductId { get; set; }
        public int QuantityAvailable { get; set; }
        public int QuantityReserved { get; set; }
        public byte[] RowVersion { get; set; } = [];
        public virtual Product Product { get; set; } = null!;


    }
}
