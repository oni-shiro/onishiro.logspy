using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using onishiro.logspy.client.ViewModels;
using onishiro.logspy.client.Views;
using Onishiro.LogSpy.Client.Core.SignalR;
using Avalonia.Threading;
using System;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace onishiro.logspy.client;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Load from appsettings.json
            var basePath = AppContext.BaseDirectory; // Use AppContext.BaseDirectory directly
            var config = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(basePath, "appsettings.json"), optional: false, reloadOnChange: true) // Combine base path with file name
                .Build();

            var brokerUrl = config["Broker:Url"] ?? throw new ArgumentNullException("Broker url is not found");
            // Broker setup
            var brokerConnection = new BrokerConnection(brokerUrl);
            var logViewModel = new LogViewModel();
            brokerConnection.OnLogReceived += logPackaet =>
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    logViewModel.ShowLogPackets(logPackaet);
                });
            };

            _ = brokerConnection.StartAsync();
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel
                {
                    LogViewModel = logViewModel,
                }
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}