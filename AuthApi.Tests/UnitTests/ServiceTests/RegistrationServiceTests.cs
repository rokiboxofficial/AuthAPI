using AuthApi.Configuration;
using AuthApi.Data;
using AuthApi.Entities;
using AuthApi.Services;
using AuthApi.Tests.Extensions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace AuthApi.Tests.UnitTests.ServiceTests;

public class RegistrationServiceTests : IAsyncLifetime
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
    public async void WhenRegistering_AndUserWithSameLoginAlreadyExists_ThenShouldThrowDbUpdateException()
    {
        // Arrange.
        var authTokensConfiguration = new AuthTokensConfiguration();
        var jwtTokensFactoryService = new JwtTokensFactoryService(_applicationContext, authTokensConfiguration);
        var passwordHashingService = new PasswordHashingService();
        var saltGeneratorService = new SaltGeneratorService();
        var registrationService = new RegistrationService(_applicationContext, jwtTokensFactoryService, passwordHashingService, saltGeneratorService);

        var authenticationData = new AuthenticationData()
        {
            Login = Guid.NewGuid().ToString(),
            Password = Guid.NewGuid().ToString(),
            Fingerprint = Guid.NewGuid().ToString()
        };
    
        // Act.
        await registrationService.RegisterAsync(authenticationData);
        var act = async () => await registrationService.RegisterAsync(authenticationData);
    
        // Assert.
        await act.Should()
                 .ThrowAsync<DbUpdateException>()
                 .WithInnerException<DbUpdateException, PostgresException>()
                 .WithMessage("*duplicate key value violates unique constraint*");
    }

    [Fact]
    public async void WhenRegistering_AndUserIsNotAlreadyExists_ThenRefreshSessionFingerprintShouldBeCorrect()
    {
        // Arrange.
        var authTokensConfiguration = new AuthTokensConfiguration();
        var jwtTokensFactoryService = new JwtTokensFactoryService(_applicationContext, authTokensConfiguration);
        var passwordHashingService = new PasswordHashingService();
        var saltGeneratorService = new SaltGeneratorService();
        var registrationService = new RegistrationService(_applicationContext, jwtTokensFactoryService, passwordHashingService, saltGeneratorService);

        var authenticationData = new AuthenticationData()
        {
            Login = Guid.NewGuid().ToString(),
            Password = Guid.NewGuid().ToString(),
            Fingerprint = Guid.NewGuid().ToString()
        };

        // Act.
        var (refreshToken, _) = await registrationService.RegisterAsync(authenticationData);

        // Assert.
        var refreshSession = refreshToken.GetRefreshSession(authTokensConfiguration, _applicationContext);
        var refreshSessionFingerprint = refreshSession?.Fingerprint;

        refreshSessionFingerprint.Should().Be(authenticationData.Fingerprint);
    }

    [Fact]
    public async void WhenRegistering_AndUserIsNotAlreadyExists_ThenAccessTokenAndRefreshSessionShouldHaveSameUserId()
    {
        // Arrange.
        var authTokensConfiguration = new AuthTokensConfiguration();
        var jwtTokensFactoryService = new JwtTokensFactoryService(_applicationContext, authTokensConfiguration);
        var passwordHashingService = new PasswordHashingService();
        var saltGeneratorService = new SaltGeneratorService();
        var registrationService = new RegistrationService(_applicationContext, jwtTokensFactoryService, passwordHashingService, saltGeneratorService);

        var authenticationData = new AuthenticationData()
        {
            Login = Guid.NewGuid().ToString(),
            Password = Guid.NewGuid().ToString(),
            Fingerprint = Guid.NewGuid().ToString()
        };

        // Act.
        var (refreshToken, accessToken) = await registrationService.RegisterAsync(authenticationData);

        // Assert.
        var accessTokenUserIdClaim = accessToken.GetClaimByType(authTokensConfiguration.UserIdClaimName);
        var accessTokenUserId = int.Parse(accessTokenUserIdClaim!.Value);

        var refreshSession = refreshToken.GetRefreshSession(authTokensConfiguration, _applicationContext);
        var refreshSessionUserId = refreshSession?.UserId;

        refreshSessionUserId.Should().Be(accessTokenUserId);
    }

    [Fact]
    public async void WhenRegistering_AndUserIsNotAlreadyExists_ThenUserShouldBeCorrect()
    {
        // Arrange.
        var authTokensConfiguration = new AuthTokensConfiguration();
        var jwtTokensFactoryService = new JwtTokensFactoryService(_applicationContext, authTokensConfiguration);
        var passwordHashingService = new PasswordHashingService();
        var saltGeneratorService = new SaltGeneratorService();
        var registrationService = new RegistrationService(_applicationContext, jwtTokensFactoryService, passwordHashingService, saltGeneratorService);

        var authenticationData = new AuthenticationData()
        {
            Login = Guid.NewGuid().ToString(),
            Password = Guid.NewGuid().ToString(),
            Fingerprint = Guid.NewGuid().ToString()
        };

        // Act.
        var (_, accessToken) = await registrationService.RegisterAsync(authenticationData);
    
        // Assert.
        var userIdClaim = accessToken.GetClaimByType(authTokensConfiguration.UserIdClaimName);
        var userId = int.Parse(userIdClaim!.Value);

        var user = _applicationContext.Users.FirstOrDefault(user => user.Id == userId);
        var userLogin = user?.Login;
        var userPasswordHash = user?.PasswordHash;
        var userSalt = user?.Salt ?? string.Empty;

        var expectedPasswordHash = passwordHashingService.HashPassword(authenticationData.Password, userSalt);

        userLogin.Should().Be(authenticationData.Login);
        userPasswordHash.Should().Be(expectedPasswordHash);
    }
}