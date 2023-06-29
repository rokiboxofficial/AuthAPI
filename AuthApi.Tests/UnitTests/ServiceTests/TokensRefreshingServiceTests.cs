using System.Security;
using AuthApi.Configuration;
using AuthApi.Data;
using AuthApi.Services;
using AuthApi.Tests.Extensions;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Testcontainers.PostgreSql;

namespace AuthApi.Tests.UnitTests.ServiceTests;

public class TokensRefreshingServiceTests : IAsyncLifetime
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
    public async void WhenRefreshingTokens_AndRefreshSessionIdIsWrong_ThenShouldThrowSecurityTokenException()
    {
        // Arrange.
        var authTokensConfiguration = new AuthTokensConfiguration();
        var jwtTokensFactoryService = new JwtTokensFactoryService(_applicationContext, authTokensConfiguration);
        var tokensRefreshingService = new TokensRefreshingService(_applicationContext, jwtTokensFactoryService);

        // Act.
        var act = async () => await tokensRefreshingService.RefreshTokensAsync(0, Guid.NewGuid().ToString());

        // Assert.
        await act.Should().ThrowAsync<SecurityTokenException>();
    }

    [Fact]
    public async void WhenRefreshingTokens_AndRefreshSessionIsExpired_ThenShouldThrowSecurityTokenExpiredException()
    {
        // Arrange.
        var authTokensConfiguration = new AuthTokensConfiguration();
        var jwtTokensFactoryService = new JwtTokensFactoryService(_applicationContext, authTokensConfiguration);
        var tokensRefreshingService = new TokensRefreshingService(_applicationContext, jwtTokensFactoryService);

        authTokensConfiguration.RefreshTokenLifetime = TimeSpan.FromMilliseconds(1);
        var fingerprint = Guid.NewGuid().ToString();
        var refreshToken = await jwtTokensFactoryService.CreateRefreshTokenAsync(1, fingerprint);
        var refreshSessionIdClaim = refreshToken.GetClaimByType(authTokensConfiguration.RefreshSessionIdClaimName);
        var refreshSessionId = long.Parse(refreshSessionIdClaim!.Value);
        await Task.Delay(2);

        // Act.
        var act = async () => await tokensRefreshingService.RefreshTokensAsync(refreshSessionId, fingerprint);

        // Assert.
        await act.Should().ThrowAsync<SecurityTokenExpiredException>();
    }

    [Fact]
    public async void WhenRefreshingTokens_AndFingerprintIsWrong_ThenShouldThrowSecurityException()
    {
        // Arrange.
        var authTokensConfiguration = new AuthTokensConfiguration();
        var jwtTokensFactoryService = new JwtTokensFactoryService(_applicationContext, authTokensConfiguration);
        var tokensRefreshingService = new TokensRefreshingService(_applicationContext, jwtTokensFactoryService);

        var fingerprint = Guid.NewGuid().ToString();
        var wrongFingerprint = Guid.NewGuid().ToString();
        var refreshToken = await jwtTokensFactoryService.CreateRefreshTokenAsync(1, fingerprint);
        var refreshSessionIdClaim = refreshToken.GetClaimByType(authTokensConfiguration.RefreshSessionIdClaimName);
        var refreshSessionId = long.Parse(refreshSessionIdClaim!.Value);

        // Act.
        var act = async () => await tokensRefreshingService.RefreshTokensAsync(refreshSessionId, wrongFingerprint);

        // Assert.
        await act.Should().ThrowAsync<SecurityException>();
    }

    [Fact]
    public async void WhenRefreshingTokens_AndFingeprintAndRefreshSessionIdAreCorrect_ThenAccessTokenUserIdShouldBeCorrect()
    {
        // Arrange.
        var authTokensConfiguration = new AuthTokensConfiguration();
        var jwtTokensFactoryService = new JwtTokensFactoryService(_applicationContext, authTokensConfiguration);
        var tokensRefreshingService = new TokensRefreshingService(_applicationContext, jwtTokensFactoryService);

        var fingerprint = Guid.NewGuid().ToString();
        var refreshToken = await jwtTokensFactoryService.CreateRefreshTokenAsync(1, fingerprint);
        var refreshSessionIdClaim = refreshToken.GetClaimByType(authTokensConfiguration.RefreshSessionIdClaimName);
        var refreshSessionId = long.Parse(refreshSessionIdClaim!.Value);

        // Act.
        var (refresh, access) = await tokensRefreshingService.RefreshTokensAsync(refreshSessionId, fingerprint);

        // Assert.
        var refreshSession = refresh.GetRefreshSession(authTokensConfiguration, _applicationContext);
        var userId = refreshSession!.UserId;

        var accessTokenUserIdClaim = access.GetClaimByType(authTokensConfiguration.UserIdClaimName);
        var accessTokenUserId = int.Parse(accessTokenUserIdClaim!.Value);

        accessTokenUserId.Should().Be(userId);
    }

    [Fact]
    public async void WhenRefreshingTokens_AndFingeprintAndRefreshSessionIdAreCorrect_ThenRefreshSessionShouldBeCorrect()
    {
        // Arrange.
        var authTokensConfiguration = new AuthTokensConfiguration();
        var jwtTokensFactoryService = new JwtTokensFactoryService(_applicationContext, authTokensConfiguration);
        var tokensRefreshingService = new TokensRefreshingService(_applicationContext, jwtTokensFactoryService);

        var userId = 1;
        var fingerprint = Guid.NewGuid().ToString();
        
        var refreshToken = await jwtTokensFactoryService.CreateRefreshTokenAsync(userId, fingerprint);
        var refreshSessionIdClaim = refreshToken.GetClaimByType(authTokensConfiguration.RefreshSessionIdClaimName);
        var refreshSessionId = long.Parse(refreshSessionIdClaim!.Value);

        // Act.
        var (refresh, access) = await tokensRefreshingService.RefreshTokensAsync(refreshSessionId, fingerprint);

        // Assert.
        var refreshSession = refresh.GetRefreshSession(authTokensConfiguration, _applicationContext);
        var refreshSessionUserId = refreshSession?.UserId;
        var refreshSessionFingerprint = refreshSession?.Fingerprint;

        refreshSessionUserId.Should().Be(userId);
        refreshSessionFingerprint.Should().Be(fingerprint);
    }
}