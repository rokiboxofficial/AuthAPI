using System.Security.Authentication;

namespace AuthApi.Exceptions;

public class WrongCredentialsException : AuthenticationException
{
    public WrongCredentialsException()
    {
    }

    public WrongCredentialsException(string message)
        : base(message)
    {
    }

    public WrongCredentialsException(string message, Exception inner)
        : base(message, inner)
    {
    }
}