using AuthApi.Entities;
using AuthApi.Exceptions;
using AuthApi.Extensions;
using AuthApi.Filters;
using AuthApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[ApiController]
[Route("/auth/register")]
[ExceptionsHandlerAttribute]
public sealed class RegistrationController : Controller
{
    private readonly UserFinderService _userFinderService;
    private readonly RegistrationService _registrationService;

    public RegistrationController(UserFinderService userFinderService, RegistrationService registrationService)
    {
        _userFinderService = userFinderService;
        _registrationService = registrationService;
    }

    [HttpPost]
    public async void Register([FromForm] AuthenticationData authenticationData)
    {
        if(await _userFinderService.FindAsync(authenticationData.Login) != null)
            throw new UserAlreadyExistsException();

        var (refreshToken, accessToken) = await _registrationService.RegisterAsync(authenticationData);
        
        var response = HttpContext.Response;
        response.SetRefreshTokenCookie(refreshToken);
        await response.SetAccessTokenInBodyAsync(accessToken);
    }
}