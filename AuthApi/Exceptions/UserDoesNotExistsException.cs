using System.Security.Authentication;

namespace AuthApi.Exceptions;

public class UserDoesNotExistsException : AuthenticationException
{
    public UserDoesNotExistsException()
    {
    }

    public UserDoesNotExistsException(string message)
        : base(message)
    {
    }

    public UserDoesNotExistsException(string message, Exception inner)
        : base(message, inner)
    {
    }
}