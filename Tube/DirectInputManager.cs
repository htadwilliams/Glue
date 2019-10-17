﻿using Glue.Events;
using SharpDX;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace Glue
{
    /// <summary>
    /// Friendly translation for JoystickUpdate values when JoystickOffset is a button
    /// </summary>
    public enum ButtonValues
    {
        Unknown = -1,
        Release = 0,
        Press =  128,
    }

    /// <summary>
    /// Friendly translation for POV values when JoystickOffset is a POV hat or d-pad
    /// </summary>
    public enum POVStates
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

    /// <summary>
    /// Fired via EventBus when an update is generated from a controller
    /// </summary>
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
    /// Gets input from DirectInput via SharpDX. See https://github.com/sharpdx/sharpdx
    ///
    /// Note: according to DirectX nomenclature, anything that isn't a keyboard or mouse is a joystick
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

        private static readonly HashSet<JoystickOffset> s_offsetsPOV = new HashSet<JoystickOffset>();
        private static readonly HashSet<JoystickOffset> s_offsetsAxis = new HashSet<JoystickOffset>();
        private static readonly HashSet<JoystickOffset> s_offsetsForceFeedback = new HashSet<JoystickOffset>();

        // Sleep interval between checking for newly connected devices
        private const int DEVICE_CONNECTION_INTERVAL_MS = 1000;
        private const int POLLING_INTERVAL_HZ = 30;     // Polling interval in polls per second
        private const int DEVICE_BUFFER_SIZE = 128;     // Magic number from an example 

        private const string THREAD_NAME_POLLING = "DirectX.Polling";
        private const string THREAD_NAME_CONNECTOR = "DirectX.Connector";

        private static readonly object s_initLock = new object();

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
            this.directInput = new DirectInput();
        }

        internal void Initialize()
        {
            InitializeOffsetGroups();
            UpdateConnectedDevices();

            StartDeviceConnector();
            StartPolling();
        }

        private void InitializeOffsetGroups()
        {
            InitializeOffsetGroup(OFFSETS_POV, s_offsetsPOV);
            InitializeOffsetGroup(OFFSETS_AXIS, s_offsetsAxis);
            InitializeOffsetGroup(OFFSETS_FORCE_FEEDBACK, s_offsetsForceFeedback);
        }

        private void InitializeOffsetGroup(JoystickOffset[] offsetArray, HashSet<JoystickOffset> offsetGroup)
        {
            lock (s_initLock)
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

        /// <summary>
        /// Enumerates devices and adds them to Joysticks if not already present.
        /// Joysticks are removed when disconnection exceptions are thrown while polling them.
        /// </summary>
        private void UpdateConnectedDevices()
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

        public void DeviceConnectorThreadProc()
        {
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
                            PublishControllerEvents(joystick, joystickUpdates);
                        }
                    }

                    if (RemoveJoysticks(joysticksUnplugged))
                    {
                        PublishControllerListChanged();
                    }
                }
            }
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
