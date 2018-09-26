namespace ViciniServer.Hardware {

    public enum HardwareStatus {
        Reserved, // Reserved by the current session
        Available, // Not reserved by anyone

        Unavailable, // Not valid, or reserved by a different session
    }
}