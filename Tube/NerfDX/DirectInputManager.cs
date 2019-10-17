using Glue.Events;
using SharpDX;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace NerfDX
{
    /// <summary>
    /// 
    /// Gets input from DirectInput via SharpDX. See https://github.com/sharpdx/sharpdx
    ///
    /// Note: according to DirectX nomenclature, anything that isn't a keyboard or mouse is a joystick
    /// 
    /// </summary>
    public class DirectInputManager : IDisposable
    {
        private readonly DirectInput directInput;

        // List of Joystick instances currently created and aquired via Joystick.Aquire() 
        private readonly List<Joystick> joysticks = new List<Joystick>();

        private Thread threadPolling;                   // Polls and disconnects devices that are connected
        private Thread threadDeviceConnector;           // Adds Joysticks to collection periodically

        private static readonly HashSet<JoystickOffset> s_offsetsPOV = new HashSet<JoystickOffset>();
        private static readonly HashSet<JoystickOffset> s_offsetsAxis = new HashSet<JoystickOffset>();
        private static readonly HashSet<JoystickOffset> s_offsetsForceFeedback = new HashSet<JoystickOffset>();

        // Sleep interval between checking for newly connected devices
        private const int DEVICE_CONNECTION_INTERVAL_MS = 500;
        private const int POLLING_INTERVAL_HZ = 60;     // Polling interval in polls per second
        private const int DEVICE_BUFFER_SIZE = 128;     // Magic number from an example 

        private const string THREAD_NAME_POLLING = "DirectX.Polling";
        private const string THREAD_NAME_CONNECTOR = "DirectX.Connector";

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly object s_lock = new object();

        // So the ugly tables can be easily hidden in the IDE
        #region Tables (static arrays of JoystickOffset and DeviceType)

        /// <summary>
        /// These DeviceType constants are the sub-set used when enumerating 
        /// and connecting to devices.
        /// </summary>
        private static readonly DeviceType[] DEVICE_TYPES =
        {
            DeviceType.Gamepad,
            DeviceType.Joystick,
            DeviceType.Remote,
            DeviceType.Flight,
        };

        private static readonly JoystickOffset[] OFFSETS_AXIS =
        {
            JoystickOffset.X,
            JoystickOffset.Y,
            JoystickOffset.Z,

            JoystickOffset.RotationX,
            JoystickOffset.RotationY,
            JoystickOffset.RotationZ,

            JoystickOffset.Sliders0,
            JoystickOffset.Sliders1,

            JoystickOffset.TorqueX,
            JoystickOffset.TorqueY,
            JoystickOffset.TorqueZ,

            JoystickOffset.AccelerationX,
            JoystickOffset.AccelerationY,
            JoystickOffset.AccelerationZ,

            JoystickOffset.AccelerationSliders0,
            JoystickOffset.AccelerationSliders1,

            JoystickOffset.AngularAccelerationX,
            JoystickOffset.AngularAccelerationY,
            JoystickOffset.AngularAccelerationZ,

            JoystickOffset.AngularVelocityX,
            JoystickOffset.AngularVelocityY,
            JoystickOffset.AngularVelocityZ,
        };

        private static readonly JoystickOffset[] OFFSETS_FORCE_FEEDBACK =
        {
            JoystickOffset.ForceX,
            JoystickOffset.ForceY,
            JoystickOffset.ForceZ,

            JoystickOffset.ForceSliders0,
            JoystickOffset.ForceSliders1,
        };

        private static readonly JoystickOffset[] OFFSETS_POV =
        {
            JoystickOffset.PointOfViewControllers0,
            JoystickOffset.PointOfViewControllers1,
            JoystickOffset.PointOfViewControllers2,
            JoystickOffset.PointOfViewControllers3,
        };

        #endregion // tables

        /// <summary>
        /// JoystickOffset constants that are POV hats or directional pads.
        /// </summary>
        public static HashSet<JoystickOffset> OffsetsPOV => s_offsetsPOV;

        /// <summary>
        /// JoystickOffset constants that are joystick axes such as sticks or 
        /// sliders.
        /// </summary>
        public static HashSet<JoystickOffset> OffsetsAxis => s_offsetsAxis;

        /// <summary>
        /// Set of JoystickOffset offsets used for force-feedback.
        /// </summary>
        public static HashSet<JoystickOffset> OffsetsForceFeedback => s_offsetsForceFeedback;

        public DirectInputManager()
        {
            directInput = new DirectInput();
        }

        internal void Initialize()
        {
            InitializeOffsetGroups();

            StartConnectorThread();
            StartPollingThread();
        }

        private void InitializeOffsetGroups()
        {
            InitializeOffsetGroup(OFFSETS_POV, s_offsetsPOV);
            InitializeOffsetGroup(OFFSETS_AXIS, s_offsetsAxis);
            InitializeOffsetGroup(OFFSETS_FORCE_FEEDBACK, s_offsetsForceFeedback);
        }

        private void InitializeOffsetGroup(JoystickOffset[] offsetArray, HashSet<JoystickOffset> offsetGroup)
        {
            lock (s_lock)
            {
                if (offsetGroup.Count < 1)
                {
                    foreach (JoystickOffset offset in offsetArray)
                    {
                        offsetGroup.Add(offset);
                    }
                }
            }
        }

        private void StartConnectorThread()
        {
            UpdateConnectedDevices();

            if (null == threadDeviceConnector)
            {
                threadDeviceConnector = new Thread(new ThreadStart(ConnectorThreadProc))
                {
                    Name = THREAD_NAME_CONNECTOR,
                    IsBackground = true
                };

            }
            threadDeviceConnector.Start();
        }

        private void StartPollingThread()
        {
            if (null == threadPolling)
            {
                threadPolling = new Thread(new ThreadStart(PollingThreadProc))
                {
                    Name = THREAD_NAME_POLLING,
                    IsBackground = true
                };
            }
            threadPolling.Start();
        }

        /// <summary>
        /// Enumerates devices and adds them to Joysticks if not already present.
        /// Joysticks are removed when disconnection exceptions are thrown while polling them.
        /// </summary>
        private void UpdateConnectedDevices()
        {
            List<DeviceInstance> devices = GetDevices();

            if (devices.Count != joysticks.Count)
            {
                lock (joysticks)
                {
                    foreach (DeviceInstance device in devices)
                    {
                        if (null == FindJoystick(device))
                        {
                            Joystick joystick = CreateJoystick(device);

                            if (null != joystick)
                            {
                                joysticks.Add(joystick);

                                PublishControllerListChanged();
                                LOGGER.Info("Connected " + joystick.Information.Type + ": " + joystick.Information.InstanceName);
                            }
                        }
                    }
                }
            }
        }

        private Joystick FindJoystick(DeviceInstance device)
        {
            foreach (Joystick joystick in joysticks)
            {
                if (joystick.Information.InstanceGuid == device.InstanceGuid)
                {
                    return joystick;
                }
            }

            return null;
        }

        private List<DeviceInstance> GetDevices()
        {
            List<DeviceInstance> devices = new List<DeviceInstance>();

            try
            {
                foreach (DeviceType deviceType in DEVICE_TYPES)
                {
                    IList<DeviceInstance> foundDevices = directInput.GetDevices(deviceType, DeviceEnumerationFlags.AllDevices);

                    foreach (DeviceInstance device in foundDevices)
                    {
                        devices.Add(device);
                    }
                }
            }
            catch (SharpDXException dxException)
            {
                LOGGER.Error("SharpDXException while getting DirectX devices", dxException);
            }

            return devices;
        }

        private Joystick CreateJoystick(DeviceInstance device)
        {
            Joystick joystick = null;

            try
            {
                joystick = new Joystick(directInput, device.InstanceGuid);

                joystick.Properties.BufferSize = DEVICE_BUFFER_SIZE;
                joystick.Acquire();
            }
            catch (SharpDXException dxException)
            {
                LOGGER.Error("SharpDXException during creation of Joystick instance", dxException);
            }

            return joystick;
        }

        private bool RemoveJoysticks(List<Joystick> joysticksUnplugged)
        {
            bool stickRemoved = false;
            if (joysticksUnplugged.Count > 0)
            {
                foreach (Joystick joystickUnplugged in joysticksUnplugged)
                {
                    joysticks.Remove(joystickUnplugged);
                    joystickUnplugged.Unacquire();
                    joystickUnplugged.Dispose();

                    stickRemoved = true;
                }
                joysticksUnplugged.Clear();
            }
            return stickRemoved;
        }

        private void PublishControllerEvents(Joystick joystick, JoystickUpdate[] joystickUpdates)
        {
            foreach (JoystickUpdate joystickUpdate in joystickUpdates)
            {
                EventBus<EventController>.Instance.SendEvent(this, new EventController(joystick, joystickUpdate));
            }
        }

        private void PublishControllerListChanged()
        {
            EventBus<EventControllersChanged>.Instance.SendEvent(this, new EventControllersChanged(GetConnectedJoysticks()));
        }

        public ReadOnlyCollection<Joystick> GetConnectedJoysticks()
        {
            return joysticks.AsReadOnly();
        }

        public void ConnectorThreadProc()
        {
            LOGGER.Info(string.Format(
                "Thread started with re/connection interval = {0:n0} MS",
                DEVICE_CONNECTION_INTERVAL_MS));

            while (true)
            {
                Thread.Sleep(DEVICE_CONNECTION_INTERVAL_MS);
                UpdateConnectedDevices();
            }
        }

        public void PollingThreadProc()
        {
            int sleepDurationMS = 1000 / POLLING_INTERVAL_HZ;
            List<Joystick> joysticksUnplugged = new List<Joystick>();

            LOGGER.Info(string.Format(
                "Thread started POLLING_INTERVAL_HZ = {0:n0} sleep duration = {1:n0} MS",
                POLLING_INTERVAL_HZ,
                sleepDurationMS));

            while (true)
            {
                Thread.Sleep(sleepDurationMS);
                PollSticks(joysticksUnplugged);
            }
        }

        private void PollSticks(List<Joystick> joysticksUnplugged)
        {
            lock (joysticks)
            {
                foreach (Joystick joystick in joysticks)
                {
                    JoystickUpdate[] joystickUpdates = null;

                    try
                    {
                        joystick.Poll();
                        joystickUpdates = joystick.GetBufferedData();
                    }
                    catch (SharpDXException dxException)
                    {
                        // Expected case when controllers are unplugged
                        if (dxException.Descriptor == ResultCode.InputLost)
                        {
                            // Add to list for removal after iterating
                            joysticksUnplugged.Add(joystick);
                            LOGGER.Info("Disconnected " + joystick.Information.Type.ToString() + ": " + joystick.Information.InstanceName);
                        }
                        else
                        {
                            LOGGER.Error("SharpDXException thrown while polling: ", dxException);
                        }
                    }

                    if (null != joystickUpdates)
                    {
                        PublishControllerEvents(joystick, joystickUpdates);
                    }
                }

                if (RemoveJoysticks(joysticksUnplugged))
                {
                    PublishControllerListChanged();
                }
            }
        }

        public void Dispose()
        {
            directInput.Dispose();
            foreach (Joystick joystick in joysticks)
            {
                joystick.Unacquire();
                joystick.Dispose();
            }
            joysticks.Clear();
        }
    }
}
