using Microsoft.AspNetCore.SignalR.Client;
using Onishiro.LogSpy.Common.Models;

namespace Onishiro.LogSpy.Client.Core.SignalR;

public class BrokerConnection
{
    private HubConnection _connection;
    private readonly string _hubUrl;
    public HubConnection Connection => _connection;
    private bool _isHandlersRegistered = false;

    public Action<LogPacket>? OnLogReceived;

    public BrokerConnection(string hubUrl)
    {
        _hubUrl = hubUrl ?? throw new ArgumentNullException(nameof(hubUrl), "Hub URL cannot be null.");
        _connection = BuildConnection();
    }
    private HubConnection BuildConnection()
    {
        return new HubConnectionBuilder()
            .WithUrl(_hubUrl)
            .WithAutomaticReconnect()
            .Build();
    }

    public async Task StartAsync()
    {
        if (_connection == null)
        {
            throw new InvalidOperationException("Connection is not initialized.");
        }

        if (!_isHandlersRegistered)
        {
            RegisterFailureHanlders();
            RegisterHandlers();
            _isHandlersRegistered = true;
        }

        if (_connection.State == HubConnectionState.Disconnected)
        {
            await _connection.StartAsync();
        }
    }

    private void RegisterHandlers()
    {
        _connection.On<string>("OnConnectedAsync", (connectionId) =>
        {
            Console.WriteLine($"Connected with ID: {connectionId}");
        });

        _connection.On<BrokerInformation>("SendBrokerInformation", (brokerInformation) =>
        {
            Console.WriteLine($"Broker Information: {brokerInformation}");
        });

        _connection.On<LogPacket>("SendLogs", (logPacket) =>
        {
            Console.WriteLine("[CLI2002] Received Log Packet");
            foreach (var kvp in logPacket.logData)
            {
                Console.WriteLine($"Source: {kvp.Key}");
                foreach (var line in kvp.Value)
                {
                    Console.WriteLine($"> {line}");
                }
            }

            // Invoke the callback if set
            OnLogReceived?.Invoke(logPacket);
        });
    }

    private void RegisterFailureHanlders()
    {
        if (_connection == null)
        {
            throw new InvalidOperationException("Hub connection is not initialized.");
        }

        _connection.Reconnecting += (err) =>
        {
            Console.WriteLine("[CLI9035] Reconnecting to broker");
            return Task.CompletedTask;
        };

        _connection.Reconnected += connectionId =>
        {
            Console.WriteLine($"[CLI754] Reconnected with ID: {connectionId}");
            return Task.CompletedTask;
        };

        // Re-register handlers on connection close
        _connection.Closed += async (error) =>
        {
            Console.WriteLine("[CLI696] Connection closed. Attempting to reconnect...");
            await Task.Delay(5000); // Wait before reconnecting
            await StartAsync();
        };
    }
}