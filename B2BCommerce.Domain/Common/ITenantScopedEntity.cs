using System;
using System.Collections.Generic;
using System.Text;

namespace B2BCommerce.Domain.Common
{
    public interface ITenantScopedEntity
    {
        int TenantId { get; set; }
    }
}
