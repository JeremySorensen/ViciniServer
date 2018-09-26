using System;
using System.Collections.Generic;
using ViciniServer.Hardware;
using ViciniServer.WebSockets;
using CreatorFunc = System.Func<ViciniServer.Hardware.HardwareDetails,
                                ViciniServer.WebSockets.SocketSender,
                                ViciniServer.Plugins.IPlugin>;
using CreatorLookup = System.Collections.Generic.Dictionary<
    ViciniServer.Hardware.ControllerType,
    System.Collections.Generic.Dictionary<
        string,
        System.Func<
            ViciniServer.Hardware.HardwareDetails,
            ViciniServer.WebSockets.SocketSender,
            ViciniServer.Plugins.IPlugin>>>;
        

namespace ViciniServer.Plugins {
    public class PluginProvider {

        private CreatorLookup pluginCreators = new CreatorLookup();

        private IPlugin CreateFakeSerialPlugin(HardwareDetails details, SocketSender socketSender) {
            return new FakeSerialPlugin(details, socketSender);
        }

        public PluginProvider() {
            // TODO build up list of plugin creators. Possibly hardcode a all the creator functions
            // and have a .toml file that links every controller/board name combo to the correct
            // creator function.
            pluginCreators[ControllerType.FakeSerial] = new Dictionary<string, CreatorFunc> {
                { "FakeSerialBoard", CreateFakeSerialPlugin }
            };
        }

        public IPlugin Create(HardwareDetails details, SocketSender socketSender) {
            var controllerType = details.ControllerType;
            var boardName = details
            .BoardName;
            return pluginCreators[controllerType][boardName]?.Invoke(details, socketSender);
        }
    }
}