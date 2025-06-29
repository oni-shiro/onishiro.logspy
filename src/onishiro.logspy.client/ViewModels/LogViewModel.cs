using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Onishiro.LogSpy.Common.Models;

namespace onishiro.logspy.client.ViewModels;

public class LogViewModel : ViewModelBase
{
    // Gets bind to ui
    public ObservableCollection<LogEntryModel> LogEntries { get; } = new();

    public LogViewModel()
    {
        
    }
    public void ShowLogPackets(LogPacket logPacket)
    {
        if (logPacket == null || logPacket.logData == null)
        {
            return;
        }

        foreach (var kvp in logPacket.logData)
        {
            var source = kvp.Key;
            var lines = kvp.Value;
            if(lines.Count == 0)
            {
                continue;
            }
            foreach (var line in lines)
            {   
                if(string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                LogEntries.Add(new LogEntryModel(source, line, DateTime.Now));
            }
        }
    }
}