using B2BCommerce.Domain.Common;
using B2BCommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace B2BCommerce.Domain.Entities
{
    public class User : BaseEntity, ITenantScopedEntity
    {
        public int TenantId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual Tenant Tenant { get; set; } = null!;

    }
}
