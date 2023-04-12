using System.Reflection;

namespace AuthApi.Extensions;

public static class IServiceCollectionExtensions
{
    public const string ServicesNamespacePrefix = "AuthApi.Services";
    public const string ServicesNamePostfix = "Service";

    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        var servicesClasses = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => type.IsClass &&
                type.Namespace != null &&
                type.Namespace.StartsWith(ServicesNamespacePrefix) &&
                type.Name.EndsWith(ServicesNamePostfix)).ToList();

        foreach(var serviceClass in servicesClasses)
            serviceCollection.AddTransient(serviceClass);

        return serviceCollection;
    }
}