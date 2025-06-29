using System.Collections.Concurrent;
using Onishiro.LogSpy.Broker.Services.Interfaces;

namespace Onishiro.LogSpy.Broker.Services.Impl;

public class FileReader : IFileReader
{
    private readonly ConcurrentDictionary<string, long> _lastReadPositions = new();
    public async Task<List<string>> ReadFileAllLinesAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("[BRK2305] File path cannot be null or empty.", nameof(filePath));
        }

        try
        {
            using Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader reader = new StreamReader(stream);

            var content = await reader.ReadToEndAsync();
            _lastReadPositions.AddOrUpdate(filePath, stream.Position, (key, oldValue) => stream.Position);
            return BreakLines(content);
        }
        catch (FileNotFoundException ex)
        {
            throw new FileNotFoundException($"[BRK2306] The file '{filePath}' was not found.", filePath, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException($"[BRK2307] Access to the file '{filePath}' is denied.", ex);
        }
        catch (IOException ex)
        {
            throw new IOException($"[BRK2308] An error occurred while reading the file '{filePath}'.", ex);
        }
    }

    public async Task<List<string>> ReadFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("[BRK2305] File path cannot be null or empty.", nameof(filePath));
        }

        try
        {
            using Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            stream.Seek(_lastReadPositions.GetOrAdd(filePath, 0), SeekOrigin.Begin);

            using StreamReader reader = new StreamReader(stream);

            var content = await reader.ReadToEndAsync();
            _lastReadPositions.AddOrUpdate(filePath, stream.Position, (key, oldValue) => stream.Position);
            return BreakLines(content);
        }
        catch (FileNotFoundException ex)
        {
            throw new FileNotFoundException($"[BRK2306] The file '{filePath}' was not found.", filePath, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException($"[BRK2307] Access to the file '{filePath}' is denied.", ex);
        }
        catch (IOException ex)
        {
            throw new IOException($"[BRK2308] An error occurred while reading the file '{filePath}'.", ex);
        }
    }

    private static List<string> BreakLines(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return new List<string>();
        }

        // Split the content into lines using Environment.NewLine for cross-platform compatibility
        return content.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
    }
}
