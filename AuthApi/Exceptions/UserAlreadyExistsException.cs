using System.Security.Authentication;

namespace AuthApi.Exceptions;

public class UserAlreadyExistsException : AuthenticationException
{
    public UserAlreadyExistsException()
    {
    }

    public UserAlreadyExistsException(string message)
        : base(message)
    {
    }

    public UserAlreadyExistsException(string message, Exception inner)
        : base(message, inner)
    {
    }
}