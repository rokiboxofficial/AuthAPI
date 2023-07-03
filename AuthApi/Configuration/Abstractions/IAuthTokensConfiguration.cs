using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Configuration.Abstractions;

public interface IAuthTokensConfiguration
{
    public string RefreshSessionIdItemName { get; }
    public string RefreshSessionIdClaimName { get; }
    public string RefreshTokenCookieName { get; }
    public string UserIdClaimName { get; }
    public string SecurityAlgorithm { get; }
    public SymmetricSecurityKey SymmetricSecurityKey { get; }
    public TimeSpan RefreshTokenLifetime { get; }
    public TimeSpan AccessTokenLifetime { get; }
}