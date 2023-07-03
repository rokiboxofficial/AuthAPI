using System.Text;
using AuthApi.Configuration.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Configuration;

public sealed class FakeAuthTokensConfiguration : IAuthTokensConfiguration
{
    private string _key = "testtesttesttest";

    public string RefreshSessionIdItemName { get; set; } = "refresh-session-id";
    public string RefreshSessionIdClaimName { get; set; } = "refresh-session-id";
    public string RefreshTokenCookieName { get; set; } = "refresh-token";
    public string UserIdClaimName { get; set; } = "user-id";
    public string SecurityAlgorithm =>
        SecurityAlgorithms.HmacSha256;
    public SymmetricSecurityKey SymmetricSecurityKey
        => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_key));
    public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromDays(30 * 3);
    public TimeSpan AccessTokenLifetime { get; set; } = TimeSpan.FromMinutes(10);
}