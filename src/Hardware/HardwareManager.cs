using System.Collections.Generic;

namespace ViciniServer.Hardware {
    public class HardwareManager {
        private Dictionary<ControllerType, IHardwareFinder> finders = 
            new Dictionary<ControllerType, IHardwareFinder>();

        public HardwareManager() {
            // TODO: dynamicly load assemblies and build list of finders
            finders[ControllerType.FakeSerial] = new FakeSerialFinder();
        }

        public HardwareInfo[] Find() {
            var hardwareInfo = new List<HardwareInfo>();
            foreach (var finder in finders.Values) {
                hardwareInfo.AddRange(finder.Find());
            }
            return hardwareInfo.ToArray();
        }

        public HardwareDetails GetDetails(HardwareInfo info) {
            return finders[info.ControllerType].GetDetails(info);
        }

    }
}