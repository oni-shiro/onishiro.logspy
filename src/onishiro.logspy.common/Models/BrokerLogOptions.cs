namespace Onishiro.LogSpy.Common.Models;

public class BrokerLogOptions
{
    public readonly static string SectionName = "LogSpy.Broker";

    public string BrokerName { get; set; } = "DefaultBroker";
    public string MachineName { get; set; } = Environment.MachineName;
    public Dictionary<string, string> FilePaths { get; set; } = new();


}