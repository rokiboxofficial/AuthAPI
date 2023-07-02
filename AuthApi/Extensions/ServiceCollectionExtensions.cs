using AuthApi.Services;

namespace AuthApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
                        .AddTransient<JwtTokensFactoryService>()
                        .AddTransient<LoginService>()
                        .AddTransient<PasswordHashingService>()
                        .AddTransient<RegistrationService>()
                        .AddTransient<SaltGeneratorService>()
                        .AddTransient<TokensRefreshingService>()
                        .AddTransient<UserFinderService>();
    }
}