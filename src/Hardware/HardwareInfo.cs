namespace ViciniServer.Hardware {
    public struct HardwareInfo {
        public HardwareId Id { get; set; }

        public HardwareStatus Status { get; set; }

        public ControllerType ControllerType { get; }

        public HardwareInfo(ControllerType controllerType, string serialOrPort, HardwareStatus status)
        {
            this.ControllerType = controllerType;
            this.Id = new HardwareId(controllerType, serialOrPort);
            this.Status = status;
        }
    }
}