using AuthApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers.Auth;

[ApiController]
[Route("/refresh-tokens")]
public sealed class TokensRefreshingController : Controller
{
    private const string RefreshTokenIdItem = "refresh-token-id";

    [HttpPost]
    [RefreshTokenHandlerAttribute(RefreshTokenIdItem)]
    public void RefreshTokens()
    {
    }
}