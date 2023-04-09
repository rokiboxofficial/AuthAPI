using System.IdentityModel.Tokens.Jwt;
using System.Security;
using AuthApi.Data;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Services;

public sealed class TokensRefreshingService
{
    private readonly ApplicationContext _applicationContext;
    private readonly JwtTokensFactoryService _jwtTokensFactoryService;

    public TokensRefreshingService(ApplicationContext applicationContext, JwtTokensFactoryService jwtTokensFactoryService)
    {
        _applicationContext = applicationContext;
        _jwtTokensFactoryService = jwtTokensFactoryService;
    }

    public async Task<(JwtSecurityToken refreshToken, JwtSecurityToken accessToken)> RefreshTokensAsync(long refreshSessionId, string fingerprint)
    {
        var refreshSession = await RemoveAndGetRefreshSessionAsync(refreshSessionId);

        ValidateExpiration(refreshSession.ExpireDate);
        ValidateFingerprint(fingerprint, refreshSession.Fingerprint);

        var refreshToken = await _jwtTokensFactoryService.CreateRefreshTokenAsync(refreshSession.UserId, fingerprint);
        var accessToken = _jwtTokensFactoryService.CreateAccessToken(refreshSession.UserId);

        return (refreshToken, accessToken);
    }

    private async Task<RefreshSession> RemoveAndGetRefreshSessionAsync(long refreshSessionId)
    {
        var refreshSession = await _applicationContext.RefreshSessions.FindAsync(refreshSessionId);
        if (refreshSession is null)
            throw new SecurityTokenException();

        _applicationContext.RefreshSessions.Remove(refreshSession);
        await _applicationContext.SaveChangesAsync();

        return refreshSession;
    }

    private void ValidateExpiration(DateTime expireDate)
    {
        if(expireDate < DateTime.UtcNow)
            throw new SecurityTokenExpiredException("Refresh token is expired");
    }

    private void ValidateFingerprint(string clientFingerprint, string serverFingerprint)
    {
        if(clientFingerprint != serverFingerprint)
            throw new SecurityException("Fingerprints are not same");
    }
}