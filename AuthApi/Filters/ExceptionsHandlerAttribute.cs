using System.Security;
using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthApi.Filters;

public sealed class ExceptionsHandlerAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        if(exception is AuthenticationException or SecurityException)
            context.Result = new UnauthorizedResult();
    }
}