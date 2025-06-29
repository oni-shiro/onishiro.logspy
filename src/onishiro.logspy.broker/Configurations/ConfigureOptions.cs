
using Onishiro.LogSpy.Common.Models;

namespace Onishiro.LogSpy.Broker.Configurations;

public static class ConfigureOptions
{
    public static IServiceCollection ConfigureBrokerLogOptions(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.Configure<BrokerLogOptions>(
                    configuration.GetSection(BrokerLogOptions.SectionName));
        return services;
    }
}