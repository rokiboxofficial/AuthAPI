using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using AuthApi.Configuration;
using AuthApi.Data;
using AuthApi.Entities;
using AuthApi.Exceptions;
using AuthApi.Services;
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
    public async void WhenLogging_AndLoginDoesNotExist_ThenShouldThrowedException()
    {
        // Arrange.
        var authTokensConfiguration = new AuthTokensConfiguration();
        var jwtTokensFactoryService = new JwtTokensFactoryService(_applicationContext, authTokensConfiguration);
        var passwordHashingService = new PasswordHashingService();
        var userFinderService = new UserFinderService(_applicationContext);
        var loginService = new LoginService(_applicationContext, passwordHashingService, userFinderService, jwtTokensFactoryService);

        var authenticationData = new AuthenticationData();

        // Act.
        var act = () => loginService.LoginAsync(authenticationData);

        // Assert.
        await act.Should().ThrowAsync<UserDoesNotExistsException>();
    }

    [Fact]
    public async void WhenLogging_AndPasswordIsNotCorrect_ThenShouldThrowedException()
    {
        // Arrange.
        var authTokensConfiguration = new AuthTokensConfiguration();
        var jwtTokensFactoryService = new JwtTokensFactoryService(_applicationContext, authTokensConfiguration);
        var passwordHashingService = new PasswordHashingService();
        var userFinderService = new UserFinderService(_applicationContext);
        var loginService = new LoginService(_applicationContext, passwordHashingService, userFinderService, jwtTokensFactoryService);
        
        var login = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();
        var wrongPassword = Guid.NewGuid().ToString();
        var fingerprint = Guid.NewGuid().ToString();

        var saltBytes = RandomNumberGenerator.GetBytes(64);
        var salt = Convert.ToBase64String(saltBytes);
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
}