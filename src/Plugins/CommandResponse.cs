namespace ViciniServer.Plugins {
    public struct CommandResponse {
        public bool TimedOut { get; }
        
        public string CommandString { get; }

        public string ResponseString { get; }

        public static CommandResponse Timeout(string commandString) {
            return new CommandResponse(true, commandString, null);
        }

        public static CommandResponse Success(string commandString, string responseString) {
            return new CommandResponse(false, commandString, responseString);
        }

        private CommandResponse(bool timedOut, string commandString, string responseString) {
            this.TimedOut = timedOut;
            this.CommandString = commandString;
            this.ResponseString = responseString;
        }
    }
}