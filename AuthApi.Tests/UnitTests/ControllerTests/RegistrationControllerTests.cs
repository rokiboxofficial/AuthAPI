using AuthApi.Configuration;
using AuthApi.Controllers;
using AuthApi.Data;
using AuthApi.Entities;
using AuthApi.Services;
using AuthApi.Tests.Extensions;
using AuthApi.Tests.UnitTests.ControllerTests.Mocks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Testcontainers.PostgreSql;

namespace AuthApi.Tests.UnitTests.ControllerTests;

public class RegistrationControllerTests : IAsyncLifetime
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
    public async void WhenRegistering_AndAuthenticationDataIsCorrect_ThenResponseShouldContainCorrectRefreshTokenCookie()
    {
        // Arrange.
        var authTokensConfiguration = new FakeAuthTokensConfiguration();
        var passwordHashingService = new PasswordHashingService();
        var userFinderService = new UserFinderService(_applicationContext);
        var jwtTokensFactoryService = new JwtTokensFactoryService(_applicationContext, authTokensConfiguration);
        var saltGeneratorService = new SaltGeneratorService();
        var registrationService = new RegistrationService(_applicationContext, jwtTokensFactoryService, passwordHashingService, saltGeneratorService);
        var registrationController = new RegistrationController(authTokensConfiguration, registrationService);

        var httpContext = new Mock<HttpContext>();
        var refreshTokenCookie = new MockResponseCookieCollection();
        httpContext.Setup(hc => hc.Response.Cookies.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>()))
        .Callback(new Action<string, string, CookieOptions>((key, value, options) => refreshTokenCookie.Append(key, value, options)));
        httpContext.Setup(hc => hc.Response.Body)
        .Returns(new Mock<Stream>().Object);

        registrationController.ControllerContext = new ControllerContext(){HttpContext = httpContext.Object};
    
        var authenticationData = new AuthenticationData()
        {
            Login = Guid.NewGuid().ToString(),
            Password = Guid.NewGuid().ToString(),
            Fingerprint = Guid.NewGuid().ToString()
        };
        
        // Act.
        await registrationController.Register(authenticationData);

        // Assert.
        refreshTokenCookie?.Key.Should().Be(authTokensConfiguration.RefreshTokenCookieName);
        refreshTokenCookie?.Value.Should().NotBeNull();
        refreshTokenCookie?.Options.Should().NotBeNull();
    }

    [Fact]
    public async void WhenRegistering_AndAuthenticationDataIsCorrect_ThenRefreshSessionShouldBeCorrect()
    {
        // Arrange.
        var authTokensConfiguration = new FakeAuthTokensConfiguration();
        var passwordHashingService = new PasswordHashingService();
        var userFinderService = new UserFinderService(_applicationContext);
        var jwtTokensFactoryService = new JwtTokensFactoryService(_applicationContext, authTokensConfiguration);
        var saltGeneratorService = new SaltGeneratorService();
        var registrationService = new RegistrationService(_applicationContext, jwtTokensFactoryService, passwordHashingService, saltGeneratorService);
        var registrationController = new RegistrationController(authTokensConfiguration, registrationService);

        var httpContext = new Mock<HttpContext>();
        var refreshTokenCookie = new MockResponseCookieCollection();
        httpContext.Setup(hc => hc.Response.Cookies.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>()))
        .Callback(new Action<string, string, CookieOptions>((key, value, options) => refreshTokenCookie.Append(key, value, options)));
        httpContext.Setup(hc => hc.Response.Body)
        .Returns(new Mock<Stream>().Object);

        registrationController.ControllerContext = new ControllerContext(){HttpContext = httpContext.Object};

        var login = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();
        var fingerprint = Guid.NewGuid().ToString();

        var authenticationData = new AuthenticationData()
        {
            Login = login,
            Password = password,
            Fingerprint = fingerprint
        };
        
        // Act.
        await registrationController.Register(authenticationData);
    
        // Assert.
        var refreshTokenCookieValue = refreshTokenCookie.Value;
        var refreshToken = refreshTokenCookieValue!.ParseJwtSecurityToken();
        var refreshSession = refreshToken.GetRefreshSession(authTokensConfiguration, _applicationContext);

        var refreshSessionUserId = refreshSession?.UserId;
        var refreshSessionFingerprint = refreshSession?.Fingerprint;

        refreshSessionUserId.Should().NotBe(0);
        refreshSessionFingerprint.Should().Be(fingerprint);
    }

    [Fact]
    public async void WhenRegistering_AndAuthenticationDataIsCorrect_ThenResponseBodyShouldContainCorrectAccessToken()
    {
        // Arrange.
        var authTokensConfiguration = new FakeAuthTokensConfiguration();
        var passwordHashingService = new PasswordHashingService();
        var userFinderService = new UserFinderService(_applicationContext);
        var jwtTokensFactoryService = new JwtTokensFactoryService(_applicationContext, authTokensConfiguration);
        var saltGeneratorService = new SaltGeneratorService();
        var registrationService = new RegistrationService(_applicationContext, jwtTokensFactoryService, passwordHashingService, saltGeneratorService);
        var registrationController = new RegistrationController(authTokensConfiguration, registrationService);

        var httpContext = new Mock<HttpContext>();
        var refreshTokenCookie = new MockResponseCookieCollection();
        httpContext.Setup(hc => hc.Response.Cookies.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>()))
        .Callback(new Action<string, string, CookieOptions>((key, value, options) => refreshTokenCookie.Append(key, value, options)));
        
        var mockStream = new MockStream();
        httpContext.Setup(hc => hc.Response.Body)
        .Returns(mockStream);

        registrationController.ControllerContext = new ControllerContext(){HttpContext = httpContext.Object};

        var login = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();
        var fingerprint = Guid.NewGuid().ToString();

        var authenticationData = new AuthenticationData()
        {
            Login = login,
            Password = password,
            Fingerprint = fingerprint
        };

        // Act.
        await registrationController.Register(authenticationData);
        
        // Assert.
        var accessToken = mockStream.Body.ParseJwtSecurityToken();
        var accessTokenUserIdClaim = accessToken.GetClaimByType(authTokensConfiguration.UserIdClaimName);
        var accessTokenUserId = int.Parse(accessTokenUserIdClaim!.Value);
        
        accessTokenUserId.Should().NotBe(0);
    }
}