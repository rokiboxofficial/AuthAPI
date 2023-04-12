using AuthApi.Configuration;
using AuthApi.Extensions;
using AuthApi.Filters;
using AuthApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers.Auth;

[ApiController]
[Route("/auth/refresh-tokens")]
public sealed class TokensRefreshingController : Controller
{
    private const string RefreshSessionIdItemName = "refresh-session-id";
    private const string FingerprintItemName = "refresh-token-id";
    private readonly TokensRefreshingService _tokensRefreshingService;

    public TokensRefreshingController(TokensRefreshingService tokensRefreshingService)
    {
        _tokensRefreshingService = tokensRefreshingService;
    }

    [HttpPost]
    [RefreshTokenHandlerAttribute(RefreshSessionIdItemName, AuthTokensConfiguration.RefreshTokenCookieName)]
    public async Task RefreshTokens([FromBody] string fingerprint)
    {
        var refreshSessionId = long.Parse((string) HttpContext.Items[RefreshSessionIdItemName]!);
        var (refreshToken, accessToken) = await _tokensRefreshingService.RefreshTokensAsync(refreshSessionId, fingerprint);

        var response = HttpContext.Response;
        response.SetRefreshTokenCookie(refreshToken);
        await response.SetAccessTokenInBodyAsync(accessToken);
    }
}