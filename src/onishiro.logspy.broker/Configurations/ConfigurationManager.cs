namespace Onishiro.LogSpy.Broker.Configurations;
public static class ConfigurationManager
{
    public static IServiceCollection ConfigureDependencies(this IServiceCollection services, IConfigurationManager configuration)
    {
        // Add all configuration extensions here
        services.ConfigureBrokerLogOptions(configuration);
        services.AddBrokerServices();
        return services;
    }
}