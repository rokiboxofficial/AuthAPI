using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Configuration;

internal sealed class AuthTokensConfiguration
{
    private const string KEY = "testtesttesttest";
    
    public static SymmetricSecurityKey GetSymmetricSecurityKey()
        => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
}