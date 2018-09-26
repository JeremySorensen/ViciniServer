using System;

namespace ViciniServer.Hardware {
    public struct HardwareId {
        public string Value { get; }

        public HardwareId(ControllerType controllerType, string serialOrPort) {
            Value = $"{serialOrPort},{controllerType}";
        }

        public HardwareId(string id) {
            var fields = id.Split(',');
            ControllerType controllerType;
            if (!ControllerType.TryParse(fields[1], false, out controllerType)) {
                throw new ArgumentException("Not a valid HardwareId string");
            }
            Value = id;
        }
    }
}