using AuthApi.Configuration.Abstractions;
using AuthApi.Entities;
using AuthApi.Extensions;
using AuthApi.Filters;
using AuthApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[ApiController]
[Route("/auth/register")]
[ExceptionsHandler]
public sealed class RegistrationController : Controller
{
    private readonly IAuthTokensConfiguration _authTokensConfiguration;
    private readonly RegistrationService _registrationService;

    public RegistrationController(IAuthTokensConfiguration authTokensConfiguration, RegistrationService registrationService)
    {
        _authTokensConfiguration = authTokensConfiguration;
        _registrationService = registrationService;
    }

    [HttpPost]
    public async Task Register([FromBody] AuthenticationData authenticationData)
    {
        var (refreshToken, accessToken) = await _registrationService.RegisterAsync(authenticationData);

        await HttpContext.Response.SetAccesAndRefreshTokens(_authTokensConfiguration.RefreshTokenCookieName, refreshToken, accessToken);
    }
}