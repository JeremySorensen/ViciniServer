using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViciniServer.Hardware;
using ViciniServer.Plugins;
using ViciniServer.WebSockets;

namespace ViciniServer.Sessions {
    public class Session : IDisposable {
        
        private PluginProvider pluginProvider;

        private SocketSender socketSender;

        public Guid Id { get; }

        public Session(Guid id, PluginProvider pluginProvider, SocketSender socketSender) {
            this.Id = id;
            this.pluginProvider = pluginProvider;
            this.socketSender = socketSender;
        }
        private Dictionary<HardwareId, IPlugin> plugins = new Dictionary<HardwareId, IPlugin>();

        public bool IsReserved(HardwareId id) { return plugins.ContainsKey(id); }

        public IPlugin Reserve(HardwareDetails details) {

            var plugin = pluginProvider.Create(details, socketSender);
            plugins.Add(details.Id, plugin);
            return plugin;
        }

        public void Unreserve(HardwareId id) {
            IPlugin plugin;
            plugins.Remove(id, out plugin);
            plugin.Dispose();
        }

        public async Task<CommandResponse> CommandRequestAsync(
            HardwareId hardwareId,
            string command,
            List<string> args,
            int? timeout)
        {
            return await plugins[hardwareId].CommandRequestAsync(command, args, timeout);
        }

        public async Task SocketMessageAsync(HardwareId hardwareId, string message) {
            await plugins[hardwareId].SocketTextMessageAsync(message);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var plugin in plugins.Values) {
                        plugin.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                plugins = null;

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Session() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }
}