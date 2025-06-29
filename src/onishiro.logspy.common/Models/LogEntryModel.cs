using System;
namespace Onishiro.LogSpy.Common.Models;
public class LogEntryModel 
{
    public string Source { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public string Message { get; set; } = "";

    public LogEntryModel(string source, string message, DateTime timestamp)
    {
        Source = source;
        Message = message;
        Timestamp = timestamp;
    }
}