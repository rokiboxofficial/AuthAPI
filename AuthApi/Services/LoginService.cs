using System.IdentityModel.Tokens.Jwt;
using AuthApi.Data;
using AuthApi.Entities;
using AuthApi.Exceptions;

namespace AuthApi.Services;

public sealed class LoginService
{
    private readonly ApplicationContext _applicationContext;
    private readonly PasswordHashingService _passwordHashingService;
    private readonly UserFinderService _userFinderService;
    private readonly  JwtTokensFactoryService _jwtTokensFactoryService;

    public LoginService(
        ApplicationContext applicationContext,
        PasswordHashingService passwordHashingService,
        UserFinderService userFinderService,
        JwtTokensFactoryService jwtTokensFactoryService)
    {
        _applicationContext = applicationContext;
        _passwordHashingService = passwordHashingService;
        _userFinderService = userFinderService;
        _jwtTokensFactoryService = jwtTokensFactoryService;
    }

    public async Task<(JwtSecurityToken refreshToken, JwtSecurityToken)> LoginAsync(AuthenticationData authenticationData)
    {
        var (login, fingerprint, password) = ParseAuthenticationData(authenticationData);
        
        var user = await _userFinderService.FindAsync(login);
        if(user == null)
            throw new UserDoesNotExistsException();

        if(!IsPasswordValid(user, password))
            throw new WrongCredentialsException();

        var refreshToken = await _jwtTokensFactoryService.CreateRefreshTokenAsync(user.Id, fingerprint);
        var accessToken = _jwtTokensFactoryService.CreateAccessToken(user.Id);

        return (refreshToken, accessToken);
    }

    private bool IsPasswordValid(User user, string password)
    {
        var hashedPassword = user.PasswordHash;
        var salt = user.Salt;
        var isPasswordValid = _passwordHashingService.ValidateHashedPassword(password, hashedPassword, salt);

        return isPasswordValid;
    }

    private (string login, string fingerprint, string password) ParseAuthenticationData(AuthenticationData authenticationData)
        => (authenticationData.Login, authenticationData.Fingerprint, authenticationData.Password);
}