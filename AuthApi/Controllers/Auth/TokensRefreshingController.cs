using AuthApi.Configuration;
using AuthApi.Extensions;
using AuthApi.Filters;
using AuthApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers.Auth;

[ApiController]
[Route("/auth/refresh-tokens")]
[ExceptionsHandler]
public sealed class TokensRefreshingController : Controller
{
    private readonly TokensRefreshingService _tokensRefreshingService;
    private readonly AuthTokensConfiguration _authTokensConfiguration;

    public TokensRefreshingController(TokensRefreshingService tokensRefreshingService, AuthTokensConfiguration authTokensConfiguration)
    {
        _tokensRefreshingService = tokensRefreshingService;
        _authTokensConfiguration = authTokensConfiguration;
    }

    [HttpPost]
    [TypeFilter(typeof(RefreshTokenHandlerAttribute))]
    public async Task RefreshTokens([FromBody] string fingerprint)
    {
        var refreshSessionIdItemName = _authTokensConfiguration.RefreshSessionIdItemName;
        var refreshSessionId = long.Parse((string) HttpContext.Items[refreshSessionIdItemName]!);
        var (refreshToken, accessToken) = await _tokensRefreshingService.RefreshTokensAsync(refreshSessionId, fingerprint);

        var response = HttpContext.Response;
        response.SetRefreshTokenCookie(_authTokensConfiguration.RefreshTokenCookieName, refreshToken);
        await response.SetAccessTokenInBodyAsync(accessToken);
    }
}