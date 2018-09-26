using System.Collections.Generic;
using System.Threading.Tasks;
using ViciniServer.Hardware;
using ViciniServer.WebSockets;

namespace ViciniServer.Plugins {

    public class FakeSerialPlugin : IPlugin
    {
        private FakeSerial device;
        private SocketSender socketSender;

        public HardwareDetails HardwareDetails { get; }

        public FakeSerialPlugin(HardwareDetails details, SocketSender socketSender) {
            this.socketSender = socketSender;
            this.HardwareDetails = details;
        }

        public async Task<CommandResponse> CommandRequestAsync(string command, List<string> args, int? timeout)
        {
            var commandStr = command + ' ' + string.Join(' ', args);
            device.WriteLine(commandStr.Trim(), timeout ?? 1000);
            string firstLine;
            device.ReadLine(0, out firstLine);
            var theRest = device.ReadAll();

            await socketSender.SendTextAsync("command", firstLine + theRest);
            return CommandResponse.Success(commandStr, firstLine);
        }

        public async Task SocketTextMessageAsync(string message)
        {
            await socketSender.SendTextAsync("message", message);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                device = null;
                socketSender = null;

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FakeSerialPlugin() {
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