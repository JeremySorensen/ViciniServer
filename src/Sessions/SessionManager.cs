using System;
using System.Collections.Generic;
using ViciniServer.Hardware;
using ViciniServer.Plugins;
using ViciniServer.WebSockets;
using System.Linq;
using System.Threading.Tasks;

namespace ViciniServer.Sessions {
    public class SessionManager: ISessionManager {

        private HardwareManager hardwareManager = new HardwareManager();

        private PluginProvider pluginProvider = new PluginProvider();

        private Dictionary<HardwareId, HardwareInfo> hardware = 
            new Dictionary<HardwareId, HardwareInfo>();

        private Dictionary<Guid, Session> sessions = new Dictionary<Guid, Session>();

        public Guid CreateSession(SocketSender socketSender) {
            var id = Guid.NewGuid();
            sessions[id] = new Session(id, pluginProvider, socketSender);
            return id;
        }

        public HardwareInfo[] FindHardware(Guid sessionId) {
            var currentHardware = hardwareManager.Find();
            hardware.Clear();
            foreach (var h in currentHardware) {
                hardware[h.Id] = h;
            }
            return SetAvailability(sessionId, currentHardware);
        }

        public (HardwareDetails, HardwareInfo[]) Reserve(Guid sessionId, HardwareId hardwareId) {
            var details = hardwareManager.GetDetails(hardware[hardwareId]);
            var plugin = sessions[sessionId].Reserve(details);
            return (
                plugin.HardwareDetails,
                SetAvailability(sessionId, hardware.Values.ToList()));
        }

        public HardwareInfo[] Unreserve(Guid sessionId, HardwareId hardwareId) {
            sessions[sessionId].Unreserve(hardwareId);
            return SetAvailability(sessionId, hardware.Values.ToList());
        }

        public async Task<CommandResponse> CommandRequestAsync(
            Guid sessionId,
            HardwareId hardwareId,
            string command,
            List<string> args,
            int? timeout) {
                return await sessions[sessionId].CommandRequestAsync(hardwareId, command, args, timeout);
            }

        private HardwareInfo[] SetAvailability(Guid sessionId, IList<HardwareInfo> info) {
            var session = sessions[sessionId];
            var result = new HardwareInfo[info.Count];
            for(int i = 0; i < info.Count; ++i) {
                var id = info[i].Id;
                result[i] = info[i];
                if (session.IsReserved(id)) {
                    result[i].Status = HardwareStatus.Reserved;
                } else if (sessions.Values.Any(s => s.Id != sessionId && s.IsReserved(id))) {
                    result[i].Status = HardwareStatus.Unavailable;
                }
            }
            return result;
        }

        public async Task SocketMessageAsync(Guid sessionId, HardwareId hardwareId, string message) {
            await sessions[sessionId].SocketMessageAsync(hardwareId, message);
        }

        public Task CloseSessionAsync(Guid sessionId) {
            Session session;
            sessions.Remove(sessionId, out session);
            session.Dispose();
            return Task.CompletedTask;
        }
    }
}