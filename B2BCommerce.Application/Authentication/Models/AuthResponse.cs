namespace B2BCommerce.Application.Authentication.Models;

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt,
    int UserId,
    string Email,
    string FullName,
    string Role
);
