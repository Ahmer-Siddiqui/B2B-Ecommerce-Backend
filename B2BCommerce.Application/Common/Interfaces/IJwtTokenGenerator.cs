using B2BCommerce.Domain.Entities;

namespace B2BCommerce.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
