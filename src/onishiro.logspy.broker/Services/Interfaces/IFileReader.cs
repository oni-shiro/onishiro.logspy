namespace Onishiro.LogSpy.Broker.Services.Interfaces;

public interface IFileReader
{
    /// <summary>
    /// Reads the content of a file asynchronously.
    /// </summary>
    /// <param name="filePath">The path to the file to read.</param>
    /// <returns>A task that represents the asynchronous read operation. The task result contains the content of the file.</returns>
    Task<List<string>> ReadFileAsync(string filePath);

    Task<List<string>> ReadFileAllLinesAsync(string filePath);
}