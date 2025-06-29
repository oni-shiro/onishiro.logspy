using Microsoft.AspNetCore.SignalR;
using Onishiro.LogSpy.Common.Models;
using Onishiro.LogSpy.Broker.SignalR.Interfaces;

public class BrokerHub : Hub<IBrokerClient>
{
    private readonly IBrokerService _brokerService;

    public BrokerHub(IBrokerService brokerService)
    {
        _brokerService = brokerService;
    }

    public override async Task OnConnectedAsync()
    {
        var brokerInfo = await _brokerService.GetBrokerInformationAsync();
        await Clients.Caller.SendBrokerInformation(brokerInfo);
        await base.OnConnectedAsync();
    }
    public async Task SendLogs(LogPacket logPacket)
    {
        await Clients.All.SendLogs(logPacket);
    }
}