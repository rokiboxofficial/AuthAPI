using System.IdentityModel.Tokens.Jwt;

namespace AuthApi.Tests.Extensions;

public static class StringExtensions
{
    public static JwtSecurityToken ParseJwtSecurityToken(this string s)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(s);

        return jwtSecurityToken;
    }
}