using SharpDX.DirectInput;

namespace NerfDX
{
    public class ConnectedDeviceInfo
    {
        public DeviceInstance Information { get; }
        public Capabilities Capabilities { get; }
        public DeviceProperties Properties { get; }

        public ConnectedDeviceInfo(WaitableJoystick joystick)
        {
            this.Information = joystick.Information;
            this.Capabilities = joystick.Capabilities;
            this.Properties = joystick.Properties;
        }
    }
}
