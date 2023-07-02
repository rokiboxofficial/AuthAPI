using System.Security;
using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Filters;

public sealed class ExceptionsHandlerAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        if(exception is AuthenticationException or SecurityException or SecurityTokenException)
            context.Result = new UnauthorizedResult();
    }
}