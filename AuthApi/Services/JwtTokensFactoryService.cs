using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthApi.Configuration.Abstractions;
using AuthApi.Data;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Services;

public sealed class JwtTokensFactoryService
{
    private readonly ApplicationContext _applicationContext;
    private readonly IAuthTokensConfiguration _authTokensConfiguration;

    public JwtTokensFactoryService(ApplicationContext applicationContext, IAuthTokensConfiguration authTokensConfiguration)
    {
        _applicationContext = applicationContext;
        _authTokensConfiguration = authTokensConfiguration;
    }

    public async Task<JwtSecurityToken> CreateRefreshTokenAsync(int userId, string fingerprint)
    {
        var expireDate = GetExpireDate(_authTokensConfiguration.RefreshTokenLifetime);
        var refreshSession = new RefreshSession(fingerprint, expireDate, userId);

        _applicationContext.RefreshSessions.Add(refreshSession);
        await _applicationContext.SaveChangesAsync();

        var refreshSessionIdClaim = new Claim(_authTokensConfiguration.RefreshSessionIdClaimName, refreshSession.Id.ToString());
        var jwt = CreateJwtSecurityToken(expireDate, new List<Claim> { refreshSessionIdClaim });
        return jwt;
    }

    public JwtSecurityToken CreateAccessToken(int userId)
    {
        var expireDate = GetExpireDate(_authTokensConfiguration.AccessTokenLifetime);
        var userIdClaim = new Claim(_authTokensConfiguration.UserIdClaimName, userId.ToString());
        
        var jwt = CreateJwtSecurityToken(expireDate, new List<Claim>{ userIdClaim });
        return jwt;
    }

    private JwtSecurityToken CreateJwtSecurityToken(DateTime expireDate, IEnumerable<Claim> claims)
    {
        return new JwtSecurityToken(
                claims: claims,
                expires: expireDate,
                signingCredentials: new SigningCredentials(
                    _authTokensConfiguration.SymmetricSecurityKey,
                    _authTokensConfiguration.SecurityAlgorithm));
    }

    private DateTime GetExpireDate(TimeSpan tokenLifeTime)
        => DateTime.UtcNow + tokenLifeTime;
}