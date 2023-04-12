using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AuthApi.Configuration;

namespace AuthApi.Extensions;

public static class HttpResponseExtensions
{
    public static void SetRefreshTokenCookie(this HttpResponse httpResponse, JwtSecurityToken refreshToken)
    {
        var serializedRefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshToken);
        var refreshTokenCookieOptions = new CookieOptions()
        {
            HttpOnly = true,
            Expires = new DateTimeOffset(refreshToken.ValidTo),
            Secure = true,
            Path = "/"
        };
        httpResponse.Cookies.Append(AuthTokensConfiguration.RefreshTokenCookieName, serializedRefreshToken, refreshTokenCookieOptions);
    }

    public static async Task SetAccessTokenInBodyAsync(this HttpResponse httpResponse, JwtSecurityToken accessToken)
    {
        var serializedAccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken);
        var bytes = Encoding.UTF8.GetBytes(serializedAccessToken);
    
        await httpResponse.Body.WriteAsync(bytes, 0, bytes.Length);
    }
}