using B2BCommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace B2BCommerce.Domain.Entities
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
