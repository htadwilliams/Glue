using SharpDX;
using SharpDX.DirectInput;
using System;
using System.Threading;

namespace NerfDX
{
    public class WaitableJoystick : Joystick
    {
        public Joystick Joystick { get; }
        public WaitHandle WaitEvent { get => waitEvent; }

        /// <summary>
        /// Wraps DeviceFlags.PolledDevice flag which in turn wraps DIDC_POLLEDDEVICE.
        /// </summary>
        public bool IsWaitable { get => !this.Capabilities.Flags.HasFlag(DeviceFlags.PolledDevice); }

        private WaitHandle waitEvent;
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public WaitableJoystick(DirectInput directInput, Guid deviceGuid) : base(directInput, deviceGuid)
        {
        }

        public virtual void SetNotification()
        {
            waitEvent = new AutoResetEvent(false);
            SetNotification(waitEvent);
        }

        public bool Connect()
        {
            try
            {
                //
                // From MSDN
                //
                // At least one object on the device is polled, rather than 
                // interrupt-driven. For these objects, the application must 
                // explicitly call the IDirectInputDevice8 Interface method to 
                // obtain data. HID devices can contain a mixture of polled 
                // and nonpolled objects.
                //

                // For simplicity and because polling overhead is required 
                // for the device if even one object on a device must be 
                // polled, the code will only poll the device (and not wait
                // on it as well).
                if (!IsWaitable)
                {
                    Acquire();
                }
                // All other devices are driven by event notification
                else 
                {
                    // Creates wait event and wraps Joystick.SetNotification(WaitHandle)
                    // Must be done before Acquire()
                    SetNotification();
                    Acquire();
                }
                return true;
            }
            catch (SharpDXException dxException)
            {
                LOGGER.Error("SharpDXException during creation of Joystick instance", dxException);
                return false;
            }
        }

        public new void Dispose()
        {
            waitEvent?.Dispose();
            base.Dispose();
        }
    }
}
