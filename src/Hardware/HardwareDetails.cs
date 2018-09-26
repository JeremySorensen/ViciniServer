namespace ViciniServer.Hardware {
    public struct HardwareDetails {
        public ControllerType ControllerType { get; }

        public string BoardName { get; set; }

        public string PartName { get; set; }

        public string[] OtherPartNames { get; set; }

        public HardwareId Id { get; }

        public HardwareDetails(ControllerType controllerType, HardwareId hardwareId)
        {
            this.Id = hardwareId;
            this.ControllerType = controllerType;
            this.BoardName = null;
            this.PartName = null;
            this.OtherPartNames = null;
        }
    }
}