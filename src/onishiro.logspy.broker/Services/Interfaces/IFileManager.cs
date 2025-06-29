
using Onishiro.LogSpy.Common.Models;
namespace Onishiro.LogSpy.Broker.Services.Interfaces;
public interface IFileManager
{
    Task<LogPacket> GetFileContentForAllFileStreamsAsync();
}