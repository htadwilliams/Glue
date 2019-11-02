using Glue.Actions;
using Glue.Events;
using Glue.Native;
using NerfDX.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WindowsInput.Native;

namespace Glue.Triggers
{

    /// <summary>
    /// Values are a JoystickOffset and can be used to decipher 
    /// JoystickUpdate structures from DirectX.
    /// </summary>
    public enum ControllerAxis
        {
            X = 0,
            Y = 4,
            Z = 8,
            RotationX = 12,
            RotationY = 16,
            RotationZ = 20,
            Sliders0 = 24,
            Sliders1 = 28,
            VelocityX = 176,
            VelocityY = 180,
            VelocityZ = 184,
            AngularVelocityX = 188,
            AngularVelocityY = 192,
            AngularVelocityZ = 196,
            VelocitySliders0 = 200,
            VelocitySliders1 = 204,
            AccelerationX = 208,
            AccelerationY = 212,
            AccelerationZ = 216,
            AngularAccelerationX = 220,
            AngularAccelerationY = 224,
            AngularAccelerationZ = 228,
            AccelerationSliders0 = 232,
            AccelerationSliders1 = 236,
            ForceX = 240,
            ForceY = 244,
            ForceZ = 248,
            TorqueX = 252,
            TorqueY = 256,
            TorqueZ = 260,
            ForceSliders0 = 264,
            ForceSliders1 = 268
        }

    //
    // Use case notes
    //
    // airplane throttle
    //
    // bands = 5
    // deadZoneIndex = 0
    // deadZone = axis_max / 10 
    // 
    //  0 1   2   3   4  
    // < --- --- --- --- >
    //
    // throttle with reverse
    //
    //  bands = 5 
    //  deadZoneIndex = 2
    //  deadZone = axis_max / 10 
    //
    //    1   2 0 3   4  
    // < --- --- --- --- >
    //
    // esdf or wasd control axis
    //
    //  bands = 2
    //  deadZoneIndex = 1 
    //  deadZone = axis_max / 5
    //  Needs bands * 2 macros (enter and exit per band), 
    //  but bands * 1 keys if holding a key while in a band
    //   
    //       S   0   F 
    //  < ------- ------- >
    //

    class TriggerControllerAxis : TriggerController
    {
        internal int DeadzoneSize => deadzoneSize;
        internal int DeadzoneIndex => deadzoneIndex;
        internal int BandSize => bandSize;
        public int Bands => bands;
        public List<Keys> KeyList => keyList;
        public ControllerAxis Axis => axis;

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly ControllerAxis axis;

        [JsonProperty]
        private readonly int deadzoneSize;

        [JsonProperty]
        private readonly int deadzoneIndex;

        [JsonProperty]
        private readonly int bands;

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        private readonly List<Keys> keyList = new List<Keys>();

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // TODO Query axis for caps to get max axis and other info
        private const int AXIS_MAX = 65535;
        private readonly int bandSize;
        private int bandPrevious = -1;

        // [JsonConstructor]
        public TriggerControllerAxis(
            string namePart,
            ControllerAxis axis,
            int bands,
            int deadzoneIndex, 
            int deadzoneSize, 
            List<Keys> keyList,
            List<string> macroNames) : base (namePart, macroNames)
        {
            this.axis = axis;
            this.bands = bands;
            this.deadzoneIndex = deadzoneIndex;
            this.deadzoneSize = deadzoneSize;
            this.keyList.AddRange(keyList);

            int bandSpace = AXIS_MAX;

            if (DeadzoneIndex != -1)
            {
                bandSpace -= deadzoneSize;
            }

            this.bandSize = bandSpace;
            if (this.bands > 0)
            {
                this.bandSize = bandSpace / this.bands;
            }

            Type = TriggerType.ControllerAxis;
        }

        protected override void OnEventController(object sender, BusEventArgs<EventController> e)
        {
            EventController eventController = e.BusEvent;

            if (
                (EventController.EventType.Axis == eventController.Type)
                && (Axis == (ControllerAxis) eventController.JoystickUpdate.Offset)
                && (eventController.Joystick.Properties.InstanceName.Contains(NamePart))
                )
            {
                // LOGGER.Info(String.Format("{0} {1} {2:n0}", NamePart, axis, axisValue));
                int bandCurrent = GetBand(eventController.JoystickUpdate.Value);
                if (bandCurrent != this.bandPrevious)
                {
                    LOGGER.Info(
                        String.Format(
                            "{0} {1} Entered band [{2}] from band [{3}] position={4:n0}", 
                            NamePart, axis, bandCurrent, bandPrevious, eventController.JoystickUpdate.Value));

                    // Activate keys for exited / entered bands 
                    if (bandPrevious != -1 && Keys.None != KeyList[bandPrevious])
                    {
                        SimulateKey((int) KeyList[bandPrevious], ButtonStates.Release);
                    }
                    else if (bandCurrent != -1 && Keys.None != KeyList[bandCurrent])
                    {
                        SimulateKey((int) KeyList[bandCurrent], ButtonStates.Press);
                    }

                    // Fire macro for entered band
                    Fire(bandCurrent);

                    this.bandPrevious = bandCurrent;
                }
            }
        }

        private void SimulateKey(int keyCode, ButtonStates buttonState)
        {
                ActionKey actionKey = new ActionKey(TimeProvider.GetTickCount(), (VirtualKeyCode) keyCode, buttonState);
                actionKey.Play();
        }

        private int GetBand(int axisPosition)
        {
            int positionNextBand = 0;
            int bandIndex = 0;
            for (; bandIndex < Bands; bandIndex++)
            {
                positionNextBand += bandIndex == DeadzoneIndex 
                    ? DeadzoneSize 
                    : BandSize; 

                if (axisPosition <= positionNextBand)
                {
                    return bandIndex;
                }
            }

            return bandIndex - 1;
        }
    }
}
