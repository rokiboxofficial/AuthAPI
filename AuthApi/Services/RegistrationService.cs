using System.IdentityModel.Tokens.Jwt;
using AuthApi.Data;
using AuthApi.Entities;

namespace AuthApi.Services;

public sealed class RegistrationService
{
    private readonly ApplicationContext _applicationContext;
    private readonly JwtTokensFactoryService _jwtTokensFactoryService;
    private readonly PasswordHashingService _passwordHashingService;
    private readonly SaltGeneratorService _saltGeneratorService;

    public RegistrationService(
        ApplicationContext applicationContext,
        JwtTokensFactoryService jwtTokensFactoryService,
        PasswordHashingService passwordHashingService,
        SaltGeneratorService saltGeneratorService)
    {
        _applicationContext = applicationContext;
        _jwtTokensFactoryService = jwtTokensFactoryService;
        _passwordHashingService = passwordHashingService;
        _saltGeneratorService = saltGeneratorService;
    }

    public async Task<(JwtSecurityToken refreshToken, JwtSecurityToken accessToken)> RegisterAsync(AuthenticationData authenticationData)
    {
        var fingerprint = authenticationData.Fingerprint;
        var password = authenticationData.Password;

        var salt = _saltGeneratorService.GenerateSalt();
        var passwordHash = _passwordHashingService.HashPassword(password, salt);
        var user = await CreateAndSaveUser(authenticationData, passwordHash, salt);

        var refreshToken = await _jwtTokensFactoryService.CreateRefreshTokenAsync(user.Id, fingerprint);
        var accessToken = _jwtTokensFactoryService.CreateAccessToken(user.Id);

        return (refreshToken, accessToken);
    }

    private async Task<User> CreateAndSaveUser(AuthenticationData authenticationData, string passwordHash, string salt)
    {
        var user = new User(authenticationData.Login, passwordHash, salt);
        _applicationContext.Users.Add(user);
        await _applicationContext.SaveChangesAsync();
        return user;
    }
}