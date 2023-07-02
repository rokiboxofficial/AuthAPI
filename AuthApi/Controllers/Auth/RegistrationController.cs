using AuthApi.Configuration;
using AuthApi.Entities;
using AuthApi.Exceptions;
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
    private readonly AuthTokensConfiguration _authTokensConfiguration;
    private readonly UserFinderService _userFinderService;
    private readonly RegistrationService _registrationService;

    public RegistrationController(AuthTokensConfiguration authTokensConfiguration, UserFinderService userFinderService, RegistrationService registrationService)
    {
        _authTokensConfiguration = authTokensConfiguration;
        _userFinderService = userFinderService;
        _registrationService = registrationService;
    }

    [HttpPost]
    public async Task Register([FromBody] AuthenticationData authenticationData)
    {
        if(await _userFinderService.FindAsync(authenticationData.Login) != null)
            throw new UserAlreadyExistsException();

        var (refreshToken, accessToken) = await _registrationService.RegisterAsync(authenticationData);
        
        var response = HttpContext.Response;
        response.SetRefreshTokenCookie(_authTokensConfiguration.RefreshTokenCookieName, refreshToken);
        await response.SetAccessTokenInBodyAsync(accessToken);
    }
}