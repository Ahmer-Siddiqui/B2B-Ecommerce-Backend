using B2BCommerce.Domain.Enums;

namespace B2BCommerce.Application.Authentication.Models;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    UserRole Role
);
