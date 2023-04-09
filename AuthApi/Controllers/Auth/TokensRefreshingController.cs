using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AuthApi.Filters;
using AuthApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers.Auth;

[ApiController]
[Route("/auth/refresh-tokens")]
public sealed class TokensRefreshingController : Controller
{
    private const string RefreshTokenCookieName = "refresh-token";
    private const string RefreshSessionIdItemName = "refresh-session-id";
    private const string FingerprintItemName = "refresh-token-id";
    private readonly TokensRefreshingService _tokensRefreshingService;

    public TokensRefreshingController(TokensRefreshingService tokensRefreshingService)
    {
        _tokensRefreshingService = tokensRefreshingService;
    }

    [HttpPost]
    [RefreshTokenHandlerAttribute(RefreshSessionIdItemName, RefreshTokenCookieName)]
    public async Task RefreshTokens([FromBody] string fingerprint)
    {
        var refreshSessionId = long.Parse((string) HttpContext.Items[RefreshSessionIdItemName]!);
        var (refreshToken, accessToken) = await _tokensRefreshingService.RefreshTokensAsync(refreshSessionId, fingerprint);

        SetRefreshTokenCookie(HttpContext.Response.Cookies, refreshToken);
        await SetAccessTokenInBodyAsync(HttpContext.Response.Body, accessToken);
    }

    private void SetRefreshTokenCookie(IResponseCookies responseCookies, JwtSecurityToken refreshToken)
    {
        var serializedRefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshToken);
        var refreshTokenCookieOptions = new CookieOptions()
        {
            HttpOnly = true,
            Expires = new DateTimeOffset(refreshToken.ValidTo),
            Secure = true,
            Path = "/"
        };
        responseCookies.Append(RefreshTokenCookieName, serializedRefreshToken, refreshTokenCookieOptions);
    }

    private async Task SetAccessTokenInBodyAsync(Stream responseBody, JwtSecurityToken accessToken)
    {
        var serializedAccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken);
        var bytes = Encoding.UTF8.GetBytes(serializedAccessToken);
    
        await responseBody.WriteAsync(bytes, 0, bytes.Length);
    }
}