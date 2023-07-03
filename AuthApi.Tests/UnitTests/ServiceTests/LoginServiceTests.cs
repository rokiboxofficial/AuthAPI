using AuthApi.Configuration;
using AuthApi.Data;
using AuthApi.Entities;
using AuthApi.Exceptions;
using AuthApi.Services;
using AuthApi.Tests.Extensions;
using FluentAssertions;
using Testcontainers.PostgreSql;

namespace AuthApi.Tests.UnitTests.ServiceTests;

public class LoginServiceTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();
    private ApplicationContext _applicationContext = null!;

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        _applicationContext = _postgreSqlContainer.CreateApplicationContext();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync().AsTask();
        await _applicationContext.DisposeAsync().AsTask();
    }

    [Fact]
    public async void WhenLogging_AndLoginDoesNotExist_ThenShouldThrowsUserDoesNotExistsException()
    {
        // Arrange.
        var authTokensConfiguration = new FakeAuthTokensConfiguration();
        var jwtTokensFactoryService = new JwtTokensFactoryService(_applicationContext, authTokensConfiguration);
        var passwordHashingService = new PasswordHashingService();
        var userFinderService = new UserFinderService(_applicationContext);
        var loginService = new LoginService(passwordHashingService, userFinderService, jwtTokensFactoryService);

        var authenticationData = new AuthenticationData();

        // Act.
        var act = () => loginService.LoginAsync(authenticationData);

        // Assert.
        await act.Should().ThrowAsync<UserDoesNotExistsException>();
    }

    [Fact]
    public async void WhenLogging_AndPasswordIsNotCorrect_ThenShouldThrowsWrongCredentialsException()
    {
        // Arrange.
        var saltGeneratorService = new SaltGeneratorService();
        var authTokensConfiguration = new FakeAuthTokensConfiguration();
        var jwtTokensFactoryService = new JwtTokensFactoryService(_applicationContext, authTokensConfiguration);
        var passwordHashingService = new PasswordHashingService();
        var userFinderService = new UserFinderService(_applicationContext);
        var loginService = new LoginService(passwordHashingService, userFinderService, jwtTokensFactoryService);
        
        var login = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();
        var wrongPassword = Guid.NewGuid().ToString();
        var fingerprint = Guid.NewGuid().ToString();

        var salt = saltGeneratorService.GenerateSalt();
        var passwordHash = passwordHashingService.HashPassword(password, salt);
        
        var user = new User(login, passwordHash, salt);
        _applicationContext.Users.Add(user);
        await _applicationContext.SaveChangesAsync();

        var authenticationData = new AuthenticationData()
        {
            Login = login,
            Password = wrongPassword,
            Fingerprint = fingerprint
        };

        // Act.
        var act = () => loginService.LoginAsync(authenticationData);

        // Assert.
        await act.Should().ThrowAsync<WrongCredentialsException>();
    }

    [Fact]
    public async void WhenLogging_AndAuthenticationDataIsCorrect_ThenRefreshSessionShouldBeCorrect()
    {
        // Arrange.
        var saltGeneratorService = new SaltGeneratorService();
        var authTokensConfiguration = new FakeAuthTokensConfiguration();
        var jwtTokensFactoryService = new JwtTokensFactoryService(_applicationContext, authTokensConfiguration);
        var passwordHashingService = new PasswordHashingService();
        var userFinderService = new UserFinderService(_applicationContext);
        var loginService = new LoginService(passwordHashingService, userFinderService, jwtTokensFactoryService);
        
        var login = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();
        var fingerprint = Guid.NewGuid().ToString();

        var salt = saltGeneratorService.GenerateSalt();
        var passwordHash = passwordHashingService.HashPassword(password, salt);
        
        var user = new User(login, passwordHash, salt);
        _applicationContext.Users.Add(user);
        await _applicationContext.SaveChangesAsync();

        var authenticationData = new AuthenticationData()
        {
            Login = login,
            Password = password,
            Fingerprint = fingerprint
        };
    
        // Act.
        var (refreshToken, _) = await loginService.LoginAsync(authenticationData);

        // Assert.
        var refreshSession = refreshToken.GetRefreshSession(authTokensConfiguration, _applicationContext);
        
        var refreshSessionUserId = refreshSession?.UserId;
        var refreshSessionFingerprint = refreshSession?.Fingerprint;

        refreshSessionUserId.Should().Be(user.Id);
        refreshSessionFingerprint.Should().Be(fingerprint);
    }

    [Fact]
    public async void WhenLogging_AndAuthenticationDataIsCorrect_ThenAccessTokenShouldContainsCorrectUserId()
    {
        // Arrange.
        var saltGeneratorService = new SaltGeneratorService();
        var authTokensConfiguration = new FakeAuthTokensConfiguration();
        var jwtTokensFactoryService = new JwtTokensFactoryService(_applicationContext, authTokensConfiguration);
        var passwordHashingService = new PasswordHashingService();
        var userFinderService = new UserFinderService(_applicationContext);
        var loginService = new LoginService(passwordHashingService, userFinderService, jwtTokensFactoryService);
        
        var login = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();
        var fingerprint = Guid.NewGuid().ToString();

        var salt = saltGeneratorService.GenerateSalt();
        var passwordHash = passwordHashingService.HashPassword(password, salt);
        
        var user = new User(login, passwordHash, salt);
        _applicationContext.Users.Add(user);
        await _applicationContext.SaveChangesAsync();

        var authenticationData = new AuthenticationData()
        {
            Login = login,
            Password = password,
            Fingerprint = fingerprint
        };

        // Act.
        var (_, accessToken) = await loginService.LoginAsync(authenticationData);
    
        // Assert.
        var userIdClaimName = authTokensConfiguration.UserIdClaimName;
        var userIdClaim = accessToken.Claims.FirstOrDefault(claim => claim.Type == userIdClaimName);
        var userId = userIdClaim?.Value;

        userId.Should().Be(user.Id.ToString());
    }
}