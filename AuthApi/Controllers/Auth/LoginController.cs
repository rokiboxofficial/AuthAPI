using AuthApi.Configuration;
using AuthApi.Entities;
using AuthApi.Extensions;
using AuthApi.Filters;
using AuthApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[ApiController]
[Route("/auth/login")]
[ExceptionsHandler]
public sealed class LoginController : Controller
{
    private readonly AuthTokensConfiguration _authTokensConfiguration;
    private readonly LoginService _loginService;

    public LoginController(AuthTokensConfiguration authTokensConfiguration, LoginService loginService)
    {
        _authTokensConfiguration = authTokensConfiguration;
        _loginService = loginService;
    }

    [HttpPost]
    public async Task Login([FromBody] AuthenticationData authenticationData)
    {
        var (refreshToken, accessToken) = await _loginService.LoginAsync(authenticationData);

        var response = HttpContext.Response;
        response.SetRefreshTokenCookie(_authTokensConfiguration.RefreshTokenCookieName, refreshToken);
        await response.SetAccessTokenInBodyAsync(accessToken);
    }
}