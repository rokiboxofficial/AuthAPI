using System.IdentityModel.Tokens.Jwt;
using AuthApi.Configuration;
using AuthApi.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Filters;

public sealed class RefreshTokenHandlerAttribute : ActionFilterAttribute
{
    private const string RefreshTokenCookieName = "refresh-token";
    private const string RefreshTokenIdClaimName = "refresh-token-id";
    private TokenValidationParameters _tokenValidationParameters;
    private JwtSecurityTokenHandler _jwtSecurityTokenHandler = new ();
    private readonly string _refreshTokenIdItemName;

    public RefreshTokenHandlerAttribute(string refreshTokenIdItemName)
    {
        _tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            IssuerSigningKey = AuthTokensConfiguration.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };
        _refreshTokenIdItemName = refreshTokenIdItemName;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if(!TryGetRefreshTokenFromCookie(context, out var refreshToken))
            Throw("Refresh token cookie is not setted");
        
        var claimsPrincipal = ValidateToken(refreshToken!);
        var refreshTokenIdClaim = claimsPrincipal.FirstOrDefaultClaimByType(RefreshTokenIdClaimName);
        ThrowIfNull(refreshTokenIdClaim, "RefreshTokenIdClaim is not setted");

        var refreshTokenId = refreshTokenIdClaim!.Value;
        context.HttpContext.Items[_refreshTokenIdItemName] = refreshTokenId;
    }

    private bool TryGetRefreshTokenFromCookie(ActionExecutingContext context, out string? refreshToken)
        => context.HttpContext.Request.Cookies.TryGetValue(RefreshTokenCookieName, out refreshToken);

    private System.Security.Claims.ClaimsPrincipal ValidateToken(string refreshToken)
        => _jwtSecurityTokenHandler.ValidateToken(refreshToken, _tokenValidationParameters, out var result);
    
    private void ThrowIfNull(object? argument, string? message = null)
    {
        if(argument == null)
            Throw(message);
    }

    private void Throw(string? message)
        => throw new SecurityTokenException(message);
}