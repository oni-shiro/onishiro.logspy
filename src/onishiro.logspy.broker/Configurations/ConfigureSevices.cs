using Onishiro.LogSpy.Broker.Services.Impl;
using Onishiro.LogSpy.Broker.Services.Interfaces;
using Onishiro.LogSpy.Broker.SignalR.Interfaces;

public static class ConfigureServices
{
    public static IServiceCollection AddBrokerServices(this IServiceCollection services)
    {
        services.AddSingleton<IFileManager, FileManager>();
        services.AddSingleton<IFileReader, FileReader>();
        services.AddSingleton<IBrokerService, BrokerService>();

        services.AddHostedService<FileWatcherService>();

        return services;
    }
}