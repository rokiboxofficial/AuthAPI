using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthApi.Configuration;
using AuthApi.Data;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Services;

public sealed class JwtTokensFactoryService
{
    private readonly ApplicationContext _applicationContext;

    public JwtTokensFactoryService(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<JwtSecurityToken> CreateRefreshTokenAsync(int userId, string fingerprint)
    {
        var expireDate = GetExpireDate(AuthTokensConfiguration.GetRefreshTokenLifetime());
        var refreshSession = new RefreshSession(fingerprint, expireDate, userId);

        _applicationContext.RefreshSessions.Add(refreshSession);
        await _applicationContext.SaveChangesAsync();

        var refreshSessionIdClaim = new Claim(AuthTokensConfiguration.RefreshSessionIdClaimName, refreshSession.Id.ToString());
        var jwt = CreateJwtSecurityToken(expireDate, new List<Claim> { refreshSessionIdClaim });
        return jwt;
    }

    public JwtSecurityToken CreateAccessToken(int userId)
    {
        var expireDate = GetExpireDate(AuthTokensConfiguration.GetAccessTokenLifetime());
        var userIdClaim = new Claim(AuthTokensConfiguration.UserIdClaimName, userId.ToString());
        
        var jwt = CreateJwtSecurityToken(expireDate, new List<Claim>{ userIdClaim });
        return jwt;
    }

    private JwtSecurityToken CreateJwtSecurityToken(DateTime expireDate, IEnumerable<Claim> claims)
    {
        return new JwtSecurityToken(
                claims: claims,
                expires: expireDate,
                signingCredentials: new SigningCredentials(
                    AuthTokensConfiguration.GetSymmetricSecurityKey(),
                    AuthTokensConfiguration.SecuirtyAlgorithm));
    }

    private DateTime GetExpireDate(TimeSpan tokenLifeTime)
        => DateTime.UtcNow + tokenLifeTime;
}