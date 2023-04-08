using System.Security.Claims;

namespace AuthApi.Extensions;

public static class ClaimsPrincipalEnumerableExtensions
{
    public static Claim? FirstOrDefaultClaimByType(this ClaimsPrincipal claimsPrincipal, string type)
        => claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == type);
}