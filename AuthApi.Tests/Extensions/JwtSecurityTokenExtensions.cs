using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthApi.Configuration;
using AuthApi.Data;

namespace AuthApi.Tests.Extensions;

public static class JwtSecurityTokenExtensions
{
    public static Claim? GetClaimByType(this JwtSecurityToken jwtSecurityToken, string type)
        => jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == type);

    public static RefreshSession? GetRefreshSession(this JwtSecurityToken jwtSecurityToken, AuthTokensConfiguration authTokensConfiguration, ApplicationContext applicationContext)
    {
        var refreshSessionIdClaimName = authTokensConfiguration.RefreshSessionIdClaimName;
        var refreshSessionIdClaim = jwtSecurityToken.GetClaimByType(refreshSessionIdClaimName);
        
        if(refreshSessionIdClaim?.Value is null)
            return null;

        var refreshSessionId = long.Parse(refreshSessionIdClaim.Value);
        var refreshSession = applicationContext.RefreshSessions.FirstOrDefault(session => session.Id == refreshSessionId);
        
        return refreshSession;
    }
}