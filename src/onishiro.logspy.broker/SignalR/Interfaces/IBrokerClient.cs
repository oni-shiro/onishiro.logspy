using Onishiro.LogSpy.Common.Models;

namespace Onishiro.LogSpy.Broker.SignalR.Interfaces;

public interface IBrokerClient
{
    Task SendLogs(LogPacket logPacket);
    Task SendBrokerInformation(BrokerInformation brokerInformation);
}
