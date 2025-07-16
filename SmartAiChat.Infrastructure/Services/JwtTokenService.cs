using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared.Constants;
using SmartAiChat.Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SmartAiChat.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<JwtTokenService> _logger;
    private readonly string _jwtKey;
    private readonly int _accessTokenExpirationMinutes;
    private readonly int _refreshTokenExpirationDays;

    public JwtTokenService(
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        ILogger<JwtTokenService> logger)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtKey = configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!123456789";
        _accessTokenExpirationMinutes = int.Parse(configuration["Jwt:ExpirationInMinutes"] ?? "60");
        _refreshTokenExpirationDays = 30; // Default 30 days for refresh tokens
    }

    public async Task<AuthTokens> GenerateTokensAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            var accessTokenExpires = DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes);
            var refreshTokenExpires = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);

            // Generate access token
            var accessToken = GenerateAccessToken(user, accessTokenExpires);

            // Generate refresh token
            var refreshToken = GenerateRefreshToken();

            // Update user with refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiresAt = refreshTokenExpires;
            user.LastLoginAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthTokens
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpires = accessTokenExpires,
                RefreshTokenExpires = refreshTokenExpires,
                TokenType = "Bearer"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating tokens for user {UserId}", user.Id);
            throw;
        }
    }

    public async Task<AuthTokens> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(
                u => u.RefreshToken == refreshToken && 
                     u.RefreshTokenExpiresAt > DateTime.UtcNow &&
                     u.IsActive,
                cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token");
            }

            return await GenerateTokensAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            throw;
        }
    }

    public async Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(
                u => u.RefreshToken == refreshToken,
                cancellationToken);

            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiresAt = null;
                await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking token");
            throw;
        }
    }

    public Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    private string GenerateAccessToken(User user, DateTime expires)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtKey);

        var claims = new List<Claim>
        {
            new(SystemConstants.ClaimTypes.UserId, user.Id.ToString()),
            new(SystemConstants.ClaimTypes.TenantId, user.TenantId.ToString()),
            new(SystemConstants.ClaimTypes.Role, user.Role.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
        var randomBytes = new byte[64];
        rngCryptoServiceProvider.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
} 