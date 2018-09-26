using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViciniServer.Hardware;

namespace ViciniServer.Plugins {
    public interface IPlugin : IDisposable {

        HardwareDetails HardwareDetails { get; }
        
        Task<CommandResponse> CommandRequestAsync(string command, List<string> args, int? timeout);

        Task SocketTextMessageAsync(string message);
    }
}