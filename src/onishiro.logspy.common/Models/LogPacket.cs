namespace Onishiro.LogSpy.Common.Models;

public class LogPacket
{
    public DateTime Timestamp { get; set; }
    public IDictionary<string, List<string>> logData { get; set; } = new Dictionary<string, List<string>>();

    public LogPacket()
    {
        Timestamp = DateTime.UtcNow;
    }
}