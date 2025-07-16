using SmartAiChat.Domain.Entities;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Domain.Interfaces;

public interface IJwtTokenService
{
    Task<AuthTokens> GenerateTokensAsync(User user, CancellationToken cancellationToken = default);
    Task<AuthTokens> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
} 