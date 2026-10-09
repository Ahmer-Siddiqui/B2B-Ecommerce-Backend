using System;
using System.Collections.Generic;
using System.Text;

namespace B2BCommerce.Application.Common.Interfaces
{
    public interface ICurrentTenantService
    {
        int? TenantId { get; }
    }
}
