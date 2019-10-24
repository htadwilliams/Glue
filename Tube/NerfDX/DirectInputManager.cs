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

        // Lists of Joystick instances currently created and aquired via Joystick.Aquire() 
        private readonly List<Joystick> joysticksPolled = new List<Joystick>();
        private readonly List<Joystick> joysticksWaitable = new List<Joystick>();

        private readonly EventWaitHandle eventNewJoystick = new AutoResetEvent (false);

        private Thread threadPolling;                   // Polls and disconnects devices 
        private Thread threadWaiting;                   // Waits for notification on waitable devices
        private Thread threadDeviceConnector;           // Adds Joysticks to collection periodically

        private static readonly HashSet<JoystickOffset> s_offsetsPOV = new HashSet<JoystickOffset>();
        private static readonly HashSet<JoystickOffset> s_offsetsAxis = new HashSet<JoystickOffset>();
        private static readonly HashSet<JoystickOffset> s_offsetsForceFeedback = new HashSet<JoystickOffset>();

        // Sleep interval between checking for newly connected devices
        private const int DEVICE_CONNECTION_INTERVAL_MS = 500;
 
        private const int POLLING_INTERVAL_HZ = 60;     // Polling interval in polls per second
        private const int DEVICE_BUFFER_SIZE = 128;     // Magic number from an example 

        private const string THREAD_NAME_POLLING = "NerfDX.Polling";
        private const string THREAD_NAME_WAITING = "NerfDX.Waiting";
        private const string THREAD_NAME_CONNECTOR = "NerfDX.Connector";

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
            DeviceType.Driving,
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

        public void Initialize()
        {
            InitializeOffsetGroups();

            // Creates connected Joysticks and prepares them for data acquisition
            // This is also what threadDeviceConnector does periodically
            UpdateConnectedDeviceList();
            
            threadDeviceConnector = StartThread(threadDeviceConnector, THREAD_NAME_CONNECTOR, new ThreadStart(ConnectorThreadProc));
        }

        private void StartThreadPolling()
        {
            threadPolling = StartThread(threadPolling, THREAD_NAME_POLLING, new ThreadStart(PollingThreadProc));
        }

        private void StartThreadWaiting()
        {
            threadWaiting = StartThread(threadWaiting, THREAD_NAME_WAITING, new ThreadStart(WaitingThreadProc));
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

        private Thread StartThread(Thread thread, string threadName, ThreadStart threadstart)
        {
            if (null != thread && thread.IsAlive)
            {
                LOGGER.Error("Thread [" + threadName + "] is already running.");
            }
            else
            {
                thread = new Thread(threadstart)
                {
                    Name = threadName,
                    IsBackground = true
                };
                thread.Start();
            }

            return thread;
        }

        private int GetConnectedCount()
        {
            return joysticksPolled.Count + joysticksWaitable.Count;
        }

        /// <summary>
        /// Enumerates devices and adds them to Joysticks if not already present.
        /// </summary>
        private void UpdateConnectedDeviceList()
        {
            // TODO UpdateConnectedDeviceList() should manage lifecycle of the 
            // other threads, starting and stopping them as needed.

            List<DeviceInstance> devices = GetDevices();

            if (devices.Count > (GetConnectedCount()))
            {
                foreach (DeviceInstance device in devices)
                {
                    if (!IsDeviceConnected(device))
                    {
                        bool connectFlag = false;
                        WaitableJoystick joystick = null;
                        {
                            try
                            {
#pragma warning disable IDE0068 // Use recommended dispose pattern: Can't be disposed until enclosing DirectInputManager is disposed.
                                joystick = CreateJoystick(device);
#pragma warning restore IDE0068 // Use recommended dispose pattern
                                if (null != joystick)
                                {
                                    if (joystick.Connect())
                                    {
                                        AddJoystick(joystick);
                                        PublishControllerListChanged();
                                        connectFlag = true;
                                        LOGGER.Info("Connected " + joystick.Information.Type + ": " + joystick.Information.InstanceName);
                                    }
                                }
                            }
                            finally
                            {
                                if (!connectFlag && joystick != null)
                                {
                                    joystick.Dispose();
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool IsDeviceConnected(DeviceInstance device)
        {
            return 
                (null != FindJoystick(joysticksPolled, device)) 
                || 
                (null != FindJoystick(joysticksWaitable, device));
        }

        private Joystick FindJoystick(List<Joystick> joysticks, DeviceInstance device)
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

        private WaitableJoystick CreateJoystick(DeviceInstance device)
        {
            WaitableJoystick joystick = null;

            try
            {
                joystick = new WaitableJoystick(directInput, device.InstanceGuid);
                joystick.Properties.BufferSize = DEVICE_BUFFER_SIZE;
            }
            catch (SharpDXException dxException)
            {
                LOGGER.Error("SharpDXException during creation of Joystick instance", dxException);
            }

            return joystick;
        }

        private void AddJoystick(WaitableJoystick joystick)
        {
            if (joystick.IsWaitable)
            {
                joysticksWaitable.Add(joystick);
                // Signal waiting thread that there are new Joysticks to wait on
                eventNewJoystick.Set();
            }
            else
            {
                joysticksPolled.Add(joystick);
            }
        }

        private bool RemoveJoystick(Joystick joystickUnplugged)
        {
            bool stickRemoved = false;

            if (joysticksPolled.Remove(joystickUnplugged) || joysticksWaitable.Remove(joystickUnplugged))
            {
                joystickUnplugged.Unacquire();
                joystickUnplugged.Dispose();
                
                stickRemoved = true;
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
            EventBus<EventControllersChanged>.Instance.SendEvent(this, new EventControllersChanged(GetConnectedDeviceInfos()));
        }

        public ReadOnlyCollection<ConnectedDeviceInfo> GetConnectedDeviceInfos()
        {
            List<ConnectedDeviceInfo> connectedDevices = new List<ConnectedDeviceInfo>(GetConnectedCount());

            foreach (WaitableJoystick joystick in joysticksPolled)
            {
                connectedDevices.Add(new ConnectedDeviceInfo(joystick));
            }

            foreach (WaitableJoystick joystick in joysticksWaitable)
            {
                connectedDevices.Add(new ConnectedDeviceInfo(joystick));
            }

            return connectedDevices.AsReadOnly();
        }

        private void ConnectorThreadProc()
        {
            LOGGER.Info(string.Format(
                "Thread started with re/connection interval = {0:n0} MS",
                DEVICE_CONNECTION_INTERVAL_MS));

            while (true)
            {
                Thread.Sleep(DEVICE_CONNECTION_INTERVAL_MS);
                UpdateConnectedDeviceList();

                if (joysticksPolled.Count > 0 && (threadPolling == null || !threadPolling.IsAlive))
                {
                    StartThreadPolling();
                }

                if (joysticksWaitable.Count > 0 && (threadWaiting == null || !threadWaiting.IsAlive))
                {
                    StartThreadWaiting();
                }
            }
        }

        private void WaitingThreadProc()
        {
            LOGGER.Info(THREAD_NAME_WAITING + " thread started");
            List<WaitHandle> waitHandleList = new List<WaitHandle> ();
 
            UpdateWaitHandles(waitHandleList);

            while (joysticksWaitable.Count > 0)
            {
                int indexEvent = WaitHandle.WaitAny(waitHandleList.ToArray());

                // New device has been added
                if (0 == indexEvent)
                {
                    UpdateWaitHandles(waitHandleList);
                }
                // All other events are for Joysticks
                else
                {
                    Joystick joystick = joysticksWaitable[indexEvent - 1];

                    if (false == ReadJoystick(joystick, false))
                    {
                        RemoveJoystick(joystick);
                        UpdateWaitHandles(waitHandleList);
                        PublishControllerListChanged();
                    }
                }
            }

            LOGGER.Info(THREAD_NAME_WAITING + " thread exiting: no joysticks to wait for");
        }

        private void UpdateWaitHandles(List<WaitHandle> waitHandleList)
        {
            waitHandleList.Clear();
            waitHandleList.Add(eventNewJoystick);
            GetJoystickWaitHandles(joysticksWaitable, waitHandleList);
        }

        private void GetJoystickWaitHandles(List<Joystick> joystickList, List<WaitHandle> waitHandleList)
        {
            foreach (WaitableJoystick joystick in joystickList)
            {
                waitHandleList.Add(joystick.WaitEvent);
            }
        }

        public void PollingThreadProc()
        {
            int sleepDurationMS = 1000 / POLLING_INTERVAL_HZ;

            LOGGER.Info(string.Format(
                THREAD_NAME_POLLING + " thread started POLLING_INTERVAL_HZ = {0:n0} sleep duration = {1:n0} MS",
                POLLING_INTERVAL_HZ,
                sleepDurationMS));

            while (joysticksPolled.Count > 0)
            {
                Thread.Sleep(sleepDurationMS);
                UpdateJoysticksPolled();
            }

            LOGGER.Info(THREAD_NAME_POLLING + " thread exiting: no joysticks to poll");
        }

        /// <summary>
        /// Shared code to update Joysticks from both polled and waiting
        /// threads. The only difference is the one call to Poll().
        /// </summary>
        /// <param name="joystick"></param>
        /// <param name="poll"></param>
        /// <returns></returns>
        private bool ReadJoystick(Joystick joystick, bool poll)
        {
            JoystickUpdate[] joystickUpdates = null;
            bool joystickUnplugged = false;

            // TODO most of this method could be moved into WritableJoystick 

            try
            {
                if (poll)
                {
                    joystick.Poll();
                }
                joystickUpdates = joystick.GetBufferedData();
            }
            catch (SharpDXException dxException)
            {
                // Expected case when controllers are unplugged
                if (dxException.Descriptor == ResultCode.InputLost)
                {
                    LOGGER.Info("Disconnected " + joystick.Information.Type.ToString() + ": " + joystick.Information.InstanceName);
                    joystickUnplugged = true;
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

             return !joystickUnplugged;
        }

        private void UpdateJoysticksPolled()
        {
            lock (joysticksPolled)
            {
                Joystick joystick;
                bool controllerListChanged = false;

                 // Iterating backward for removal while iterating
                for (int indexJoystick = joysticksPolled.Count - 1; indexJoystick >= 0; indexJoystick--)
                {
                    joystick = joysticksPolled[indexJoystick];
                    if (!ReadJoystick(joystick, true))
                    {
                        RemoveJoystick(joystick);
                        controllerListChanged = true;
                    }
                }

                if (controllerListChanged)
                {
                    PublishControllerListChanged();
                }
            }
        }

        public void Dispose()
        {
            directInput.Dispose();
            eventNewJoystick.Dispose();

            DisposeJoysticks(this.joysticksPolled);
            DisposeJoysticks(this.joysticksWaitable);
        }

        private void DisposeJoysticks(List<Joystick> joysticks)
        {
            foreach (Joystick joystick in joysticks)
            {
                joystick.Unacquire();
                joystick.Dispose();
            }
        }
    }
}
