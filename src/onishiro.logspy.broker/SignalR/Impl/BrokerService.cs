using Microsoft.Extensions.Options;
using Onishiro.LogSpy.Broker.SignalR.Interfaces;
using Onishiro.LogSpy.Common.Models;
public class BrokerService : IBrokerService
{
    private IOptions<BrokerLogOptions> _brokerLogOptions;
    public BrokerService(IOptions<BrokerLogOptions> brokerLogOptions)
    {
        _brokerLogOptions = brokerLogOptions;
    }

    public Task<BrokerInformation> GetBrokerInformationAsync()
    {
        var brokerInfo = new BrokerInformation
        {
            Name = _brokerLogOptions.Value.BrokerName,
            MachineName = _brokerLogOptions.Value.MachineName
        };
        return Task.FromResult(brokerInfo);
    }
}