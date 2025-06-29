using Onishiro.LogSpy.Common.Models;
using Onishiro.LogSpy.Broker.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.SignalR;

namespace Onishiro.LogSpy.Broker.Services.Impl;

public class FileManager : IFileManager
{
    private readonly IOptions<BrokerLogOptions> _brokerLogOptions;
    private readonly IFileReader _fileReader;

    public FileManager(IOptions<BrokerLogOptions> brokerLogOptions, IFileReader fileReader)
    {
        _brokerLogOptions = brokerLogOptions ?? throw new ArgumentNullException(nameof(brokerLogOptions));
        _fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));
    }

    public async Task<LogPacket> GetFileContentForAllFileStreamsAsync()
    {
        var logPacket = InitializeLogPacket();

        foreach (var (key, logPath) in _brokerLogOptions.Value.FilePaths)
        {
            if (IsInvalidPath(key, logPath)) continue;

            var content = await TryReadContentAsync(key, logPath);

            AppendLogContent(logPacket, key, content);
        }

        LogIfNoData(logPacket);

        return logPacket;
    }

    private LogPacket InitializeLogPacket() =>
        new LogPacket { Timestamp = DateTime.UtcNow };

    private bool IsInvalidPath(string key, string logPath)
    {
        if (string.IsNullOrWhiteSpace(logPath))
        {
            Console.WriteLine($"[BRK648] Log path for key '{key}' is null or empty.");
            return true;
        }
        return false;
    }

    private async Task<List<string>> TryReadContentAsync(string key, string logPath)
    {
        try
        {
            var content = await _fileReader.ReadFileAsync(logPath);
            Console.WriteLine($"[BRK649] No new content in file '{logPath}'.");
            return content;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BRK651] Error reading file '{logPath}' for key '{key}': {ex.Message}");
            throw new Exception($"[BRK651] Error reading file '{logPath}' for key '{key}'.", ex);
        }
    }

    private void AppendLogContent(LogPacket packet, string key, List<string> content)
    {
        if (content == null || content.Count == 0 || content.All(string.IsNullOrWhiteSpace))
        {
            Console.WriteLine($"[BRK652] No content found for key '{key}' in file.");
            return;
        }
        if (!packet.logData.ContainsKey(key))
        {
            packet.logData[key] = new List<string>();
        }

        packet.logData[key] = content;
    }

    private void LogIfNoData(LogPacket packet)
    {
        if (packet.logData.Count == 0)
        {
            Console.WriteLine("[BRK650] No log data was collected.");
        }
    }
}
