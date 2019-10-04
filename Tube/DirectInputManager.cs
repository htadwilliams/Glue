using SharpDX;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Glue
{
    public enum ButtonValues
    {
        Release = 0,
        Press =  128,
    }

    public enum HatValues
    {
        Release = -1,
        Up = 0,
        Upright = 4500,
        Right = 9000,
        Downright = 13500,
        Down = 18000,
        Downleft = 22500,
        Left = 27000,
        Upleft = 31500,
    }

    public class ControllerEventArgs : EventArgs
    {
        public JoystickUpdate Update => update;
        public Joystick Joystick => joystick;

        private readonly JoystickUpdate update;
        private readonly Joystick joystick;

        public ControllerEventArgs(Joystick joystick, JoystickUpdate update)
        {
            this.update = update;
            this.joystick = joystick;
        }
    }

    /// <summary>
    /// 
    /// Gets button presses from DirectInput via SharpDX. See https://github.com/sharpdx/sharpdx
    ///
    /// Note: according to DirectX nomenclature, anything that isn't a keyboard is a joystick
    /// 
    /// </summary>
    public class DirectInputManager : IDisposable
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly DirectInput directInput;

        // List of Joystick instances currently created and aquired via Joystick.Aquire() 
        private readonly List<Joystick> joysticks = new List<Joystick>();

        private Thread threadPolling;                   // Polls and disconnects devices that are connected
        private Thread threadDeviceConnector;           // Adds Joysticks to collection periodically

        private static readonly HashSet<JoystickOffset> s_offsetsHat = new HashSet<JoystickOffset>();
        private static readonly HashSet<JoystickOffset> s_offsetsAxis = new HashSet<JoystickOffset>();
        private static readonly HashSet<JoystickOffset> s_offsetsForceFeedback = new HashSet<JoystickOffset>();

        private const int POLLING_INTERVAL_HZ = 30;     // Polling interval in polls per second
        private const int DEVICE_BUFFER_SIZE = 128;     // Magic number from an example 

        private const string THREAD_NAME_POLLING = "DirectX.Polling";
        private const string THREAD_NAME_CONNECTOR = "DirectX.Connector";

        private static readonly object s_initLock = new object();

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

        private static readonly JoystickOffset[] OFFSETS_HAT =
        {
            JoystickOffset.PointOfViewControllers0,
            JoystickOffset.PointOfViewControllers1,
            JoystickOffset.PointOfViewControllers2,
            JoystickOffset.PointOfViewControllers3,
        };

        public event OnControllerButton ControllerButtonEvent;
        public delegate void OnControllerButton(ControllerEventArgs eventArgs);
        
        public event OnControllerHat ControllerHatEvent;
        public delegate void OnControllerHat(ControllerEventArgs eventArgs);

        public event OnControllerAxis ControllerAxisEvent;
        public delegate void OnControllerAxis(ControllerEventArgs eventArgs);

        public DirectInputManager()
        {
            this.directInput = new DirectInput();
        }

        internal void Initialize()
        {
            InitializeOffsetFilters();
            InitJoysticks();

            StartDeviceConnector();
            StartPolling();
        }

        private void InitializeOffsetFilters()
        {
            InitializeOffsets(OFFSETS_HAT, s_offsetsHat);
            InitializeOffsets(OFFSETS_AXIS, s_offsetsAxis);
            InitializeOffsets(OFFSETS_FORCE_FEEDBACK, s_offsetsForceFeedback);
        }

        private void InitializeOffsets(JoystickOffset[] offsetArray, HashSet<JoystickOffset> offsetsToInitialize)
        {
            lock (s_initLock)
            {
                if (offsetsToInitialize.Count < 1)
                {
                    foreach (JoystickOffset offset in offsetArray)
                    {
                        offsetsToInitialize.Add(offset);
                    }
                }
            }
        }

        /// <summary>
        /// Enumerates devices and adds them to Joysticks if not already present.
        /// 
        /// Joysticks are removed when disconnection exceptions are thrown while polling them.
        /// </summary>
        private void InitJoysticks()
        {
            List<DeviceInstance> devices = GetDevices();

            if (devices.Count != this.joysticks.Count)
            {
                foreach (DeviceInstance device in devices)
                {
                    lock (this.joysticks)
                    {
                        if (null == FindJoystick(device))
                        {
                            Joystick joystick = CreateJoystick(device);

                            if (null != joystick)
                            {
                                this.joysticks.Add(joystick);

                                LOGGER.Info("Connected " + joystick.Information.Type + ": " + joystick.Information.InstanceName);
                            }
                        }
                    }
                }
            }
        }

        private Joystick FindJoystick(DeviceInstance device)
        {
            foreach (Joystick joystick in this.joysticks)
            {
                if (joystick.Information.InstanceGuid == device.InstanceGuid)
                {
                    return joystick;
                }
            }

            return null;
        }

        private void StartDeviceConnector()
        {
            if (null == threadDeviceConnector)
            {
                threadDeviceConnector = new Thread(new ThreadStart(DeviceConnectorThreadProc))
                {
                    Name = THREAD_NAME_CONNECTOR,
                    IsBackground = true
                };

                threadDeviceConnector.Start();
            }
        }

        private List<DeviceInstance> GetDevices()
        {
            List<DeviceInstance> devices = new List<DeviceInstance>();

            try
            {
                // foreach (DeviceType deviceType in Enum.GetValues(typeof(DeviceType)))
                foreach (DeviceType deviceType in DEVICE_TYPES)
                {
                    IList<DeviceInstance> foundDevices = this.directInput.GetDevices(deviceType, DeviceEnumerationFlags.AllDevices);

                    //if (LOGGER.IsDebugEnabled && foundDevices.Count > 0)
                    //{
                    //    LOGGER.Debug("    Devices of type [" + deviceType.ToString() + "]:");
                    //}

                    foreach (DeviceInstance device in foundDevices)
                    {
                        //if (LOGGER.IsDebugEnabled)
                        //{
                        //    string message = String.Format(
                        //        "        {0} : GUID=[{1}]",
                        //        device.InstanceName,
                        //        device.ProductGuid.ToString());

                        //    LOGGER.Debug(message);
                        //}

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
                joystick = new Joystick(this.directInput, device.InstanceGuid);

                joystick.Properties.BufferSize = DEVICE_BUFFER_SIZE;
                joystick.Acquire();
            }
            catch (SharpDXException dxException)
            {
                LOGGER.Error("SharpDXException during creation of Joystick instance", dxException);
            }

            return joystick;
        }

        private void StartPolling()
        {
            if (null == threadPolling)
            {
                threadPolling = new Thread(new ThreadStart(PollingThreadProc))
                {
                    Name = THREAD_NAME_POLLING,
                    IsBackground = true
                };

                threadPolling.Start();
            }
        }

        private const int DEVICE_CONNECTION_INTERVAL_MS = 1000;

        public void DeviceConnectorThreadProc()
        {
            while (true)
            {
                Thread.Sleep(DEVICE_CONNECTION_INTERVAL_MS);

                InitJoysticks();
            }
        }

        public void PollingThreadProc()
        {
            int sleepDurationMS = 1000 / POLLING_INTERVAL_HZ;
            List<Joystick> joysticksUnplugged = new List<Joystick>();

            LOGGER.Info(String.Format(
                "DirectInput polling thread started POLLING_INTERVAL_HZ = {0:n0} sleep duration = {1:n0}", 
                POLLING_INTERVAL_HZ, 
                sleepDurationMS));

            while (true)
            {
                Thread.Sleep(sleepDurationMS);

                lock (this.joysticks)
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
                            ProcessUpdates(joystick, joystickUpdates);
                        }
                    }

                    RemoveJoysticks(joysticksUnplugged);
                    joysticksUnplugged.Clear();
                }
            }
        }

        private void RemoveJoysticks(List<Joystick> joysticksUnplugged)
        {
            foreach (Joystick joystickUnplugged in joysticksUnplugged)
            {
                joysticks.Remove(joystickUnplugged);
                joystickUnplugged.Dispose();
            }
        }

        private void ProcessUpdates(Joystick joystick, JoystickUpdate[] joystickUpdates)
        {
            foreach (JoystickUpdate joystickUpdate in joystickUpdates)
            {
                if (joystickUpdate.Offset >= JoystickOffset.Buttons0 &&
                    joystickUpdate.Offset <= JoystickOffset.Buttons127)
                {
                    LOGGER.Info(
                        joystick.Information.InstanceName + " " +
                        joystickUpdate.Offset.ToString() + " " +
                        (ButtonValues) joystickUpdate.Value);

                    ControllerButtonEvent?.Invoke(new ControllerEventArgs(joystick, joystickUpdate));
                }
                else if (s_offsetsHat.Contains(joystickUpdate.Offset))
                {
                    ControllerHatEvent?.Invoke(new ControllerEventArgs(joystick, joystickUpdate));

                    LOGGER.Info(
                        joystick.Information.InstanceName + " " +
                        joystickUpdate.Offset.ToString() + " " +
                        (HatValues) joystickUpdate.Value);
                }
                else if (s_offsetsAxis.Contains(joystickUpdate.Offset))
                {
                    ControllerAxisEvent?.Invoke(new ControllerEventArgs(joystick, joystickUpdate));
                }

            }
        }

        public void Dispose()
        {
            directInput.Dispose();
            foreach (Joystick joystick in joysticks)
            {
                joystick.Dispose();
            }
            joysticks.Clear();
        }
    }
}
