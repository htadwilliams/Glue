using SharpDX;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Glue
{
    /// <summary>
    /// 
    /// Gets button presses from DirectInput via SharpDX. See https://github.com/sharpdx/sharpdx
    ///
    /// </summary>
    public class DirectInputManager : IDisposable
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly DirectInput directInput;

        // Note: according to DirectX nomenclature, anything that isn't a keyboard is a joystick

        // List of Joystick instances currently created and aquired via Joystick.Aquire() 
        private readonly List<Joystick> joysticks = new List<Joystick>();

        private Thread threadPolling;                   // Polls and disconnects devices that are connected
        private Thread threadDeviceConnector;           // Adds Joysticks to collection periodically

        private static readonly HashSet<JoystickOffset> s_offsetFilterSet = new HashSet<JoystickOffset>();

        private const int POLLING_INTERVAL_HZ = 30;     // Polling interval in polls per second
        private const int DEVICE_BUFFER_SIZE = 128;     // Magic number from an example 

        private const string THREAD_NAME_POLLING = "DirectX.Polling";
        private const string THREAD_NAME_CONNECTOR = "DirectX.Connector";

        private static readonly DeviceType[] DEVICE_TYPES = 
        { 
            DeviceType.Gamepad, 
            DeviceType.Joystick, 
        };

        private static readonly JoystickOffset[] JOYSTICK_OFFSET_FILTERS =
        {
            JoystickOffset.AccelerationSliders0,
            JoystickOffset.AccelerationSliders1,
            JoystickOffset.RotationX,
            JoystickOffset.RotationY,
            JoystickOffset.RotationZ,
            JoystickOffset.X,
            JoystickOffset.Y,
            JoystickOffset.Z,
            JoystickOffset.Sliders0,
            JoystickOffset.Sliders1,
        };

        public DirectInputManager()
        {
            this.directInput = new DirectInput();
        }

        internal void Initialize()
        {
            InitializeOffsetFilter();
            InitJoysticks();

            StartDeviceConnector();
            StartPolling();
        }

        private void InitializeOffsetFilter()
        {
            lock (s_offsetFilterSet)
            {
                if (s_offsetFilterSet.Count < 1)
                {
                    foreach (JoystickOffset offset in JOYSTICK_OFFSET_FILTERS)
                    {
                        s_offsetFilterSet.Add(offset);
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

                                LOGGER.Info("Adding new Joystick instance: " + joystick.Information.InstanceName);
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
                        try
                        {
                            joystick.Poll();

                            JoystickUpdate[] joystickUpdates = joystick.GetBufferedData();
                            foreach (JoystickUpdate joystickUpdate in joystickUpdates)
                            {
                                if (!s_offsetFilterSet.Contains(joystickUpdate.Offset))
                                {
                                    LOGGER.Info(joystickUpdate);
                                }
                            }
                        }
                        catch (SharpDXException dxException)
                        {
                            // Expected case when controllers are unplugged
                            if (dxException.Descriptor == ResultCode.InputLost)
                            {
                                LOGGER.Info("Unplugged device: " + joystick.Information.Type.ToString() + " [" + joystick.Information.InstanceName + "]");

                                // Add to list for removal after iterating
                                joysticksUnplugged.Add(joystick);
                            }
                            else
                            {
                                LOGGER.Error("SharpDXException thrown while polling: ", dxException);
                            }
                        }
                        catch (Exception e)
                        {
                            LOGGER.Error("Exception thrown while polling: ", e);
                        }
                    }

                    foreach (Joystick joystickUnplugged in joysticksUnplugged)
                    {
                        joysticks.Remove(joystickUnplugged);
                        joystickUnplugged.Dispose();
                    }
                    joysticksUnplugged.Clear();
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
