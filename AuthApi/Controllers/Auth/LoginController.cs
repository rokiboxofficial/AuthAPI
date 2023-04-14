using AuthApi.Entities;
using AuthApi.Extensions;
using AuthApi.Filters;
using AuthApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[ApiController]
[Route("/auth/login")]
[ExceptionsHandlerAttribute]
public sealed class LoginController : Controller
{
    private readonly LoginService _loginService;

    public LoginController(LoginService loginService)
    {
        _loginService = loginService;
    }

    [HttpPost]
    public async Task Login(AuthenticationData authenticationData)
    {
        var (refreshToken, accessToken) = await _loginService.LoginAsync(authenticationData);
        
        var response = HttpContext.Response;
        response.SetRefreshTokenCookie(refreshToken);
        await response.SetAccessTokenInBodyAsync(accessToken);
    }
}