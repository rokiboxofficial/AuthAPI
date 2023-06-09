using AuthApi.Configuration.Abstractions;
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
    private readonly IAuthTokensConfiguration _authTokensConfiguration;
    private readonly LoginService _loginService;

    public LoginController(IAuthTokensConfiguration authTokensConfiguration, LoginService loginService)
    {
        _authTokensConfiguration = authTokensConfiguration;
        _loginService = loginService;
    }

    [HttpPost]
    public async Task Login([FromBody] AuthenticationData authenticationData)
    {
        var (refreshToken, accessToken) = await _loginService.LoginAsync(authenticationData);

        await HttpContext.Response.SetAccesAndRefreshTokens(_authTokensConfiguration.RefreshTokenCookieName, refreshToken, accessToken);
    }
}