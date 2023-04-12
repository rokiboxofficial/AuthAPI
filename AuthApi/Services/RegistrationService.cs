using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using AuthApi.Data;
using AuthApi.Entities;

namespace AuthApi.Services;

public sealed class RegistrationService
{
    private readonly ApplicationContext _applicationContext;
    private readonly JwtTokensFactoryService _jwtTokensFactoryService;
    private readonly PasswordHashingService _passwordHashingService;

    public RegistrationService(
        ApplicationContext applicationContext,
        JwtTokensFactoryService jwtTokensFactoryService,
        PasswordHashingService passwordHashingService)
    {
        _applicationContext = applicationContext;
        _jwtTokensFactoryService = jwtTokensFactoryService;
        _passwordHashingService = passwordHashingService;
    }

    public async Task<(JwtSecurityToken refreshToken, JwtSecurityToken accessToken)> RegisterAsync(AuthenticationData authenticationData)
    {
        var fingeprint = authenticationData.Fingerprint;
        var password = authenticationData.Password;

        var salt = GenerateSalt();
        var passwordHash = _passwordHashingService.HashPassword(password, salt);
        var user = await CreateAndSaveUser(authenticationData, salt, passwordHash);

        var refreshToken = await _jwtTokensFactoryService.CreateRefreshTokenAsync(user.Id, fingeprint);
        var accessToken = _jwtTokensFactoryService.CreateAccessToken(user.Id);

        return (refreshToken, accessToken);
    }

    private string GenerateSalt()
    {
        var saltBytes = RandomNumberGenerator.GetBytes(64);
        var salt = Convert.ToBase64String(saltBytes);

        return salt;
    }

    private async Task<User> CreateAndSaveUser(AuthenticationData authenticationData, string passwordHash, string salt)
    {
        var user = new User(authenticationData.Login, passwordHash, salt);
        _applicationContext.Users.Add(user);
        await _applicationContext.SaveChangesAsync();
        return user;
    }
}