namespace B2BCommerce.Application.Authentication.Models;

public record LoginRequest(
    string Email,
    string Password
);
