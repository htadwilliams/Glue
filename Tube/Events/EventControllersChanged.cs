using NerfDX;
using SharpDX.DirectInput;
using System.Collections.ObjectModel;

namespace Glue.Events
{
    public class EventControllersChanged : Event
    {
        public ReadOnlyCollection<ConnectedDeviceInfo> ConnectedDeviceInfos { get; }

        public EventControllersChanged(ReadOnlyCollection<ConnectedDeviceInfo> connectedDeviceInfos)
        {
            ConnectedDeviceInfos = connectedDeviceInfos;
        }
    }
}
