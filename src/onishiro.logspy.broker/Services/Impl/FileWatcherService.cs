using Onishiro.LogSpy.Common.Models;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Onishiro.LogSpy.Broker.SignalR.Interfaces;
using Onishiro.LogSpy.Broker.Services.Interfaces;

namespace Onishiro.LogSpy.Broker.Services.Impl;

public class FileWatcherService : BackgroundService
{
    private readonly IFileManager _fileManager;
    private readonly BrokerLogOptions _logOptions;
    private readonly Dictionary<string, FileSystemWatcher> _watchers = new();
    private readonly ConcurrentDictionary<string, DateTime> _lastTriggered = new();
    private readonly IHubContext<BrokerHub, IBrokerClient> _hubContext;

    private readonly TimeSpan _debounceDelay = TimeSpan.FromMilliseconds(500);

    public FileWatcherService(
        IFileManager fileManager,
        IOptions<BrokerLogOptions> logOptions,
        IHubContext<BrokerHub, IBrokerClient> hubContext)
    {
        _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
        _logOptions = logOptions?.Value ?? throw new ArgumentNullException(nameof(logOptions));
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var (key, filePath) in _logOptions.FilePaths)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                Console.WriteLine($"[BRK700] Invalid file path for key '{key}': {filePath}");
                continue;
            }

            var directory = Path.GetDirectoryName(filePath)!;
            var filename = Path.GetFileName(filePath);

            var watcher = new FileSystemWatcher(directory, filename)
            {
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };

            watcher.Changed += async (s, e) => await OnFileChangedAsync(e.FullPath);

            _watchers[filePath] = watcher;

            Console.WriteLine($"[BRK701] Watching file: {filePath}");
        }

        return Task.CompletedTask;
    }

    private async Task OnFileChangedAsync(string path)
    {
        // Debounce rapid duplicate events
        var now = DateTime.UtcNow;
        if (_lastTriggered.TryGetValue(path, out var last) &&
            (now - last) < _debounceDelay)
        {
            return;
        }

        _lastTriggered[path] = now;

        try
        {
            Console.WriteLine($"[BRK702] Detected change in {path}, reading logs...");
            LogPacket packet = await _fileManager.GetFileContentForAllFileStreamsAsync();
            // Print json for testing
            //Console.WriteLine($"[Sisaha] {ConvertToJsonElement(packet)}");
            // Send the log packet to all connected clients
            await _hubContext.Clients.All.SendLogs(packet);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BRK703] Failed to process file change for {path}: {ex.Message}");
        }
    }

    public override void Dispose()
    {
        foreach (var watcher in _watchers.Values)
        {
            watcher.Dispose();
        }
        base.Dispose();
    }

    private JsonElement ConvertToJsonElement(LogPacket packet)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(packet, options);
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }
}
