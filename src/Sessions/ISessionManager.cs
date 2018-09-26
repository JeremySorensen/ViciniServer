using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViciniServer.Hardware;
using ViciniServer.Plugins;
using ViciniServer.WebSockets;

namespace ViciniServer.Sessions {
    public interface ISessionManager {

        Guid CreateSession(SocketSender socketSender);

        HardwareInfo[] FindHardware(Guid sessionId);

        (HardwareDetails, HardwareInfo[]) Reserve(Guid sessionId, HardwareId hardwareId);

        HardwareInfo[] Unreserve(Guid sessionId, HardwareId hardwareId);

        Task<CommandResponse> CommandRequestAsync(
            Guid sessionId,
            HardwareId hardwareId,
            string command,
            List<string> args,
            int? timeout);

        Task SocketMessageAsync(Guid sessionId, HardwareId hardwareId, string message);

        Task CloseSessionAsync(Guid sessionId);
    }
}