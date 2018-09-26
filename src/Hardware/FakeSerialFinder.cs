namespace ViciniServer.Hardware {
    public class FakeSerialFinder : IHardwareFinder
    {
        public HardwareInfo[] Find()
        {
            return new HardwareInfo[] {
                new HardwareInfo(ControllerType.FakeSerial, "COM3", HardwareStatus.Available)
            };
        }

        public HardwareDetails GetDetails(HardwareInfo info) {
            var result = new HardwareDetails(ControllerType.FakeSerial, info.Id);
            result.BoardName = "FakeSerialBoard";
            result.PartName = "FakeSerialChip";
            return result;
        }
    }
}