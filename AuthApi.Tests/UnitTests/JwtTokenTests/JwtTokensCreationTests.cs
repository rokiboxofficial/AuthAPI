using Testcontainers.PostgreSql;
using AuthApi.Services;
using AuthApi.Configuration;
using AuthApi.Data;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace AuthApi.Tests.UnitTests.JwtTokenTests;

public class JwtTokensCreationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();
    private ApplicationContext _applicationContext = null!;

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
        var connectionString = _postgreSqlContainer.GetConnectionString();
        optionsBuilder.UseNpgsql(connectionString);
        _applicationContext = new ApplicationContext(optionsBuilder.Options);
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync().AsTask();
        await _applicationContext.DisposeAsync().AsTask();
    }

    [Fact]
    public void WhenCreatingAccessToken_ThenUserIdClaimShouldNotBeNull()
    {
        // Arrange.
        var authTokensConfiguration = new AuthTokensConfiguration();
        var jwtTokensFactoryService = CreateJwtTokensFactoryService(authTokensConfiguration);
        var userId = 0;
        
        // Act.
        var accessToken = jwtTokensFactoryService.CreateAccessToken(userId); 

        // Assert.
        var claims = accessToken.Claims;
        var userIdClaimName = authTokensConfiguration.UserIdClaimName;
        var userIdClaim = claims.FirstOrDefault(claim => claim.Type == userIdClaimName);

        userIdClaim.Should().NotBeNull();
    }

    [Fact]
    public void WhenCreatingAccessToken_AndUserIdIs0_ThenUserIdClaimValueShouldBe0()
    {
        // Arrange.
        var authTokensConfiguration = new AuthTokensConfiguration();
        var jwtTokensFactoryService = CreateJwtTokensFactoryService(authTokensConfiguration);
        var userId = 0;

        // Act.
        var accessToken = jwtTokensFactoryService.CreateAccessToken(userId); 

        // Assert.
        var claims = accessToken.Claims;
        var userIdClaimName = authTokensConfiguration.UserIdClaimName;
        var userIdClaim = claims.FirstOrDefault(claim => claim.Type == userIdClaimName);

        userIdClaim?.Value.Should().Be(userId.ToString());
    }

    [Fact]
    public async Task WhenCreatingRefreshToken_ThenRefreshSessionIdClaimShouldNotBeNull()
    {
        // Arrange.
        var authTokensConfiguration = new AuthTokensConfiguration();
        var jwtTokensFactoryService = CreateJwtTokensFactoryService(authTokensConfiguration);
        var userId = 0;
        var fingerprint = Guid.NewGuid().ToString();

        // Act.
        var refreshToken = await jwtTokensFactoryService.CreateRefreshTokenAsync(userId, fingerprint);

        // Assert.
        var claims = refreshToken.Claims;
        var refreshSessionIdClaimName = authTokensConfiguration.RefreshSessionIdClaimName;
        var refreshSessionIdClaim = claims.FirstOrDefault(claim => claim.Type == refreshSessionIdClaimName);

        refreshSessionIdClaim.Should().NotBeNull();
    }

    [Fact]
    public async Task WhenCreatingRefreshToken_ThenRefreshSessionShouldNotBeNull()
    {
        // Arrange.
        var authTokensConfiguration = new AuthTokensConfiguration();
        var jwtTokensFactoryService = CreateJwtTokensFactoryService(authTokensConfiguration);
        var userId = 0;
        var fingerprint = Guid.NewGuid().ToString();
    
        // Act.
        var refreshToken = await jwtTokensFactoryService.CreateRefreshTokenAsync(userId, fingerprint);

        // Assert.
        var claims = refreshToken.Claims;
        var refreshSessionIdClaimName = authTokensConfiguration.RefreshSessionIdClaimName;
        var refreshSessionIdClaim = refreshToken.Claims.First(claim => claim.Type == refreshSessionIdClaimName);
        var refreshSessionId = int.Parse(refreshSessionIdClaim.Value);
        var refreshSession = _applicationContext.RefreshSessions.FirstOrDefault(refreshSession => refreshSession.Id == refreshSessionId);

        refreshSession.Should().NotBeNull();
    }

    [Fact]
    public async Task WhenCreatingRefreshToken_ThenRefreshSessionShouldContainCorrectData()
    {
        // Arrange.
        var authTokensConfiguration = new AuthTokensConfiguration();
        var jwtTokensFactoryService = CreateJwtTokensFactoryService(authTokensConfiguration);
        var userId = 0;
        var fingerprint = Guid.NewGuid().ToString();

        // Act.
        var refreshToken = await jwtTokensFactoryService.CreateRefreshTokenAsync(userId, fingerprint);

        // Assert.
        var claims = refreshToken.Claims;
        var refreshSessionIdClaimName = authTokensConfiguration.RefreshSessionIdClaimName;
        var refreshSessionIdClaim = refreshToken.Claims.First(claim => claim.Type == refreshSessionIdClaimName);
        var refreshSessionId = int.Parse(refreshSessionIdClaim.Value);
        var refreshSession = _applicationContext.RefreshSessions.FirstOrDefault(refreshSession => refreshSession.Id == refreshSessionId);

        var refreshSessionUserId = refreshSession?.UserId;
        var refreshSessionFingerprint = refreshSession?.Fingerprint; 
        refreshSessionUserId.Should().Be(userId);
        refreshSessionFingerprint.Should().Be(fingerprint);
    }

    private JwtTokensFactoryService CreateJwtTokensFactoryService(AuthTokensConfiguration authTokensConfiguration)
        => new JwtTokensFactoryService(_applicationContext, authTokensConfiguration);
}