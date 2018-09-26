namespace ViciniServer.Hardware {
    public interface IHardwareFinder {
        HardwareInfo[] Find();

        HardwareDetails GetDetails(HardwareInfo info);
    }
}