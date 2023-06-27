using System.IdentityModel.Tokens.Jwt;
using AuthApi.Configuration;
using AuthApi.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Filters;

public sealed class RefreshTokenHandlerAttribute : ActionFilterAttribute
{
    private readonly AuthTokensConfiguration _authTokensConfiguration;
    private readonly string _refreshSessionIdItemName;
    private readonly string _refreshTokenCookieName;
    private TokenValidationParameters _tokenValidationParameters;
    private JwtSecurityTokenHandler _jwtSecurityTokenHandler = new ();

    public RefreshTokenHandlerAttribute(AuthTokensConfiguration authTokensConfiguration,string refreshSessionIdItemName, string refreshTokenCookieName)
    {
        _tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey = authTokensConfiguration.SymmetricSecurityKey,
            ValidateIssuerSigningKey = true,
        };
        _authTokensConfiguration = authTokensConfiguration;
        _refreshSessionIdItemName = refreshSessionIdItemName;
        _refreshTokenCookieName = refreshTokenCookieName;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if(!TryGetRefreshTokenFromCookie(context, out var refreshToken))
            throw new SecurityTokenException("Refresh token cookie is not setted");
        
        var claimsPrincipal = ValidateToken(refreshToken!);
        var refreshSessionIdClaim = claimsPrincipal.FirstOrDefaultClaimByType(_authTokensConfiguration.RefreshSessionIdClaimName);
        if(refreshSessionIdClaim == null)
            throw new SecurityTokenException("Refresh session id claim is not setted");

        var refreshSessionId = refreshSessionIdClaim!.Value;
        context.HttpContext.Items[_refreshSessionIdItemName] = refreshSessionId;
    }

    private bool TryGetRefreshTokenFromCookie(ActionExecutingContext context, out string? refreshToken)
        => context.HttpContext.Request.Cookies.TryGetValue(_refreshTokenCookieName, out refreshToken);

    private System.Security.Claims.ClaimsPrincipal ValidateToken(string refreshToken)
        => _jwtSecurityTokenHandler.ValidateToken(refreshToken, _tokenValidationParameters, out var result);
}