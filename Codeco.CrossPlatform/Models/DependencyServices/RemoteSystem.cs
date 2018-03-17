namespace Codeco.CrossPlatform.Models.DependencyServices
{
    public class RemoteSystem
    {
        public string DisplayName { get; set; }
        public string Id { get; set; }
        public RemoteSystemStatus Status { get; set; }
        /// <summary>
        /// Checks whether the given remote system is available through proximal connection
        /// (such as a Bluetooth or local network connection) as opposed to cloud connection.
        /// </summary>
        public bool IsAvailableByProximity { get; set; }
        public RemoteSystemKind Kind { get; set; }
    }

    public enum RemoteSystemStatus
    {        
        Unavailable = 0,        
        DiscoveringAvailability = 1,        
        Available = 2,        
        Unknown = 3
    }

    public enum RemoteSystemKind
    {
        Desktop,
        Holographic,
        Hub,
        Iot,
        Laptop,
        Phone,
        Tablet,
        Unknown,
        Xbox
    }
}
