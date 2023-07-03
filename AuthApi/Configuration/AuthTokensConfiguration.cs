using System.Text;
using AuthApi.Configuration.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Configuration;

internal sealed class AuthTokensConfiguration : IAuthTokensConfiguration
{
    private string _secretKey = "testtesttesttest";

    private const string SecretKeyConfigurationKey = "Key";
    private const string RefreshSessionIdItemNameConfigurationKey = "RefreshSessionIdItemName";
    private const string RefreshSessionIdClaimNameConfigurationKey = "RefreshSessionIdClaimName";
    private const string RefreshTokenCookieNameConfigurationKey = "RefreshTokenCookieName";
    private const string UserIdClaimNameConfigurationKey = "UserIdClaimName";
    private const string RefreshTokenLifetimeInSecondsConfigurationKey = "RefreshTokenLifetimeInSeconds";
    private const string AccessTokenLifetimeInSecondsConfigurationKey = "AccessTokenLifetimeInSeconds";

    public AuthTokensConfiguration(IConfiguration configuration)
    {
        _secretKey = configuration[SecretKeyConfigurationKey]!;
        RefreshSessionIdItemName = configuration[RefreshSessionIdItemNameConfigurationKey]!;
        RefreshSessionIdClaimName = configuration[RefreshSessionIdClaimNameConfigurationKey]!;
        RefreshTokenCookieName = configuration[RefreshTokenCookieNameConfigurationKey]!;
        UserIdClaimName = configuration[UserIdClaimNameConfigurationKey]!;
        var refreshTokenLifetimeInSeconds = int.Parse(configuration[RefreshTokenLifetimeInSecondsConfigurationKey]!);
        var accessTokenLifetimeInSeconds = int.Parse(configuration[AccessTokenLifetimeInSecondsConfigurationKey]!);

        RefreshTokenLifetime = TimeSpan.FromSeconds(refreshTokenLifetimeInSeconds);
        AccessTokenLifetime = TimeSpan.FromSeconds(accessTokenLifetimeInSeconds);
    }

    public string RefreshSessionIdItemName { get; set; }
    public string RefreshSessionIdClaimName { get; set; }
    public string RefreshTokenCookieName { get; set; }
    public string UserIdClaimName { get; set; }
    public string SecurityAlgorithm =>
        SecurityAlgorithms.HmacSha256;
    public SymmetricSecurityKey SymmetricSecurityKey
        => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secretKey));
    public TimeSpan RefreshTokenLifetime { get; set; }
    public TimeSpan AccessTokenLifetime { get; set; }
}