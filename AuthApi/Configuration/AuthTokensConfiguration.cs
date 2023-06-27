using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Configuration;

public sealed class AuthTokensConfiguration
{
    private string _key = "testtesttesttest";
    private int _daysInMonth = 30;

    public AuthTokensConfiguration()
    {
    }

    public string RefreshSessionIdItemName { get; set; } = "refresh-session-id";
    public string RefreshSessionIdClaimName { get; set; } = "refresh-session-id";
    public string RefreshTokenCookieName { get; set; } = "refresh-token";
    public string UserIdClaimName { get; set; } = "user-id";
    public string SecurityAlgorithm =>
        SecurityAlgorithms.HmacSha256;
    public SymmetricSecurityKey SymmetricSecurityKey
        => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_key));
    public TimeSpan RefreshTokenLifetime
        => TimeSpan.FromDays(_daysInMonth * 3);
    public TimeSpan AccessTokenLifetime
        => TimeSpan.FromMinutes(10);
}