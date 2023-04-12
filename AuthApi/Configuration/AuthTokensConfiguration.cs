using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Configuration;

internal sealed class AuthTokensConfiguration
{
    public const string RefreshTokenCookieName = "refresh-token";
    public const string RefreshSessionIdClaimName = "refresh-session-id";
    public const string UserIdClaimName = "user-id";
    private const int DaysInMonth = 30;
    private const string KEY = "testtesttesttest";

    public static string SecuirtyAlgorithm =>
        SecurityAlgorithms.HmacSha256;
    
    public static SymmetricSecurityKey GetSymmetricSecurityKey()
        => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));

    public static TimeSpan GetRefreshTokenLifetime()
        => TimeSpan.FromDays(DaysInMonth * 3);

    public static TimeSpan GetAccessTokenLifetime()
        => TimeSpan.FromMinutes(10);
}