using B2BCommerce.Application.Authentication.Models;
using B2BCommerce.Application.Common.Interfaces;
using B2BCommerce.Domain.Entities;
using B2BCommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace B2BCommerce.Infrastructure.Security;

public class AuthService : IAuthService
{
    private readonly B2BCommerceDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ICurrentTenantService _currentTenantService;

    public AuthService(
        B2BCommerceDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        ICurrentTenantService currentTenantService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _currentTenantService = currentTenantService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var tenantId = _currentTenantService.TenantId 
            ?? throw new InvalidOperationException("Tenant context is required for registration.");

        // Check if user already exists within this tenant
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (emailExists)
        {
            throw new InvalidOperationException("A user with this email already exists in this organization.");
        }

        var user = new User
        {
            TenantId = tenantId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            Role = request.Role,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return await GenerateAuthResponseAsync(user, tenantId, cancellationToken);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var tenantId = _currentTenantService.TenantId 
            ?? throw new InvalidOperationException("Tenant context is required for login.");

        // Global query filter automatically filters by tenantId
        var user = await _context.Users
            .SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Your account is deactivated. Contact administrator.");
        }

        return await GenerateAuthResponseAsync(user, tenantId, cancellationToken);
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var existingToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .SingleOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (existingToken == null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        // Fraud / Replay Attack Detection: If token is already revoked, invalidate all user sessions
        if (existingToken.IsRevoked)
        {
            var userTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == existingToken.UserId && !rt.IsRevoked)
                .ToListAsync(cancellationToken);

            foreach (var token in userTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedAccessException("Replay attack detected. All sessions revoked for security.");
        }

        if (existingToken.IsExpired)
        {
            throw new UnauthorizedAccessException("Refresh token has expired. Please log in again.");
        }

        // Rotate token
        var newRefreshTokenString = GenerateSecureToken();
        existingToken.IsRevoked = true;
        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.ReplacedByToken = newRefreshTokenString;

        var newRefreshToken = new RefreshToken
        {
            TenantId = existingToken.TenantId,
            UserId = existingToken.UserId,
            Token = newRefreshTokenString,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        var accessToken = _jwtTokenGenerator.GenerateToken(existingToken.User);

        return new AuthResponse(
            AccessToken: accessToken,
            RefreshToken: newRefreshToken.Token,
            RefreshTokenExpiresAt: newRefreshToken.ExpiresAt,
            UserId: existingToken.User.Id,
            Email: existingToken.User.Email,
            FullName: $"{existingToken.User.FirstName} {existingToken.User.LastName}".Trim(),
            Role: existingToken.User.Role.ToString()
        );
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(User user, int tenantId, CancellationToken cancellationToken)
    {
        var accessToken = _jwtTokenGenerator.GenerateToken(user);
        var refreshTokenString = GenerateSecureToken();

        var refreshToken = new RefreshToken
        {
            TenantId = tenantId,
            UserId = user.Id,
            Token = refreshTokenString,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken.Token,
            RefreshTokenExpiresAt: refreshToken.ExpiresAt,
            UserId: user.Id,
            Email: user.Email,
            FullName: $"{user.FirstName} {user.LastName}".Trim(),
            Role: user.Role.ToString()
        );
    }

    private static string GenerateSecureToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
