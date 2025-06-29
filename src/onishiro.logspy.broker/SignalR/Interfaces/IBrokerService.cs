using Onishiro.LogSpy.Common.Models;

namespace Onishiro.LogSpy.Broker.SignalR.Interfaces;
public interface IBrokerService
{
    public Task<BrokerInformation> GetBrokerInformationAsync();
}