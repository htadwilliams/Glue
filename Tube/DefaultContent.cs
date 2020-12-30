using Glue.Actions;
using Glue.Events;
using Glue.Triggers;
using NerfDX.DirectInput;
using System.Collections.Generic;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;
using static Glue.Events.EventMouse;
using static Glue.Tube;

namespace Glue
{
    class DefaultContent
    {
        // used to generate example content 
        private const string PROCESS_NAME_NOTEPAD           = "notepad";      // also matches Notepad++
        private const string PROCESS_NAME_WASD              = "fallout4.exe";
        private const string NAME_CONTROLLER                = "Throttle - HOTAS Warthog";
        private const int BUTTON_CONTROLLER                 = 23;

        // example content tweakables
        private const int TIME_REPEAT_SOUND_MS              = 5 * 1000;       // For several sound delay loops 
        private const int TIME_DELAY_GLOBAL_MS              = 100;            // Delay fudge everywhere - can crank up for debugging
        private const int TIME_DWELL_GLOBAL_MS              = 250;            // Pressed keys are held this long
        private const int TIME_DELAY_ACTION                 = 3 * 1000;       // Bound to trigger for single delayed action

        public static void AddRemap(string keyOld, string keyNew, string procName)
        {
            Keys keyCodeOld = Keyboard.GetKey(keyOld).Keys;

            Tube.KeyMap.Add(
                (VirtualKeyCode) keyCodeOld, 
                new KeyboardRemapEntry(keyOld, keyNew, procName));
        }

        public static void Generate()
        {
            GenerateMacros();
            GenerateKeyRemaps();
        }

        public static void GenerateMacros()
        {
            string macroName;
            //
            // Create macro with several actions bound to CTRL-Z
            //
            Macro macro = new Macro(macroName = "delayed-action", TIME_DELAY_ACTION) 
                // .AddAction(new ActionSound(TIME_DELAY_GLOBAL_MS,  "ahha.wav"))
                .AddAction(new ActionSound(TIME_DELAY_GLOBAL_MS,  "fail.wav"))
                .AddAction(new ActionKey(TIME_DELAY_GLOBAL_MS, VirtualKeyCode.RETURN, ButtonStates.Both))
                ;
            Macros.Add(macroName, macro);
            // Setup trigger
            TriggerKeyboard trigger = new TriggerKeyboard(Keys.Z, macroName);
            trigger.AddModifier(Keys.LControlKey);
            Tube.Triggers.Add(trigger);

            //
            // Create and bind a typing macro (string of text) 
            // 
            macro = new Macro(macroName = "typing-stuff", 2000);
            
            macro.AddAction(
                new ActionTyping(
                    // Thinking of Maud you forget everything else
                    "Lorem ipsum dollar sit amet, Cogito Maud obliviscaris aliorum.",
                    10,     // delay MS
                    10));   // dwell time MS
            Macros.Add(macroName, macro);
            trigger = new TriggerKeyboard(Keys.C, macroName);
            trigger.AddModifier(Keys.LControlKey);
            trigger.AddModifier(Keys.LMenu);
            Tube.Triggers.Add(trigger);

            //
            // Create a trigger that alternates between macros - simple ripple fire example
            //
            macro = new Macro(macroName = "sound-servomotor", 0);
            macro.AddAction(new ActionSound(TIME_DELAY_GLOBAL_MS, "sound_servomotor.wav"));
            Macros.Add(macroName, macro);

            macro = new Macro(macroName = "sound-ahha", 0);
            macro.AddAction(new ActionSound(TIME_DELAY_GLOBAL_MS, "ahha.wav"));
            Macros.Add(macroName, macro);

            trigger = new TriggerKeyboard(Keys.S, new List<string> { "sound-servomotor", "sound-ahha" });
            trigger.AddModifier(Keys.LControlKey);
            Tube.Triggers.Add(trigger);

            // Makes mouse wheel noisy - mouse wheel trigger example
            TriggerMouseWheel triggerMouseWheel = new TriggerMouseWheel(WheelMoves.Up, "sound-servomotor", false);
            Tube.Triggers.Add(triggerMouseWheel);
            triggerMouseWheel = new TriggerMouseWheel(WheelMoves.Down, "sound-ahha", false);
            Tube.Triggers.Add(triggerMouseWheel);

            //
            // Drum kit!
            // Schedule repeating sound every N MS 
            //
            (string soundMacroName, string soundFile, string repeatMacroName, string stopMacroName, Keys triggerKey)[] macroTable =
            {
                ("sound-dice",  "dice_roll.wav",        "repeat-sound-dice",    "stop-sound-dice",  Keys.Oemcomma),
                ("sound-tock",  "sound_click_tock.wav", "repeat-sound-tock",    "stop-sound-tock",  Keys.OemPeriod),
                ("sound-estop", "elev_stop.wav",        "repeat-sound-estop",   "stop-sound-estop", Keys.OemQuestion),
            };

            foreach((string soundMacroName, string soundFile, string repeatMacroName, string stopMacroName, Keys triggerKey) in macroTable)
            {
                // Sound macro
                macro = new Macro(macroName = soundMacroName, TIME_DELAY_GLOBAL_MS);
                macro.AddAction(new ActionSound(0, soundFile));
                Macros.Add(macroName, macro);

                // Repeater macro and trigger
                macro = new Macro(macroName = repeatMacroName, TIME_DELAY_GLOBAL_MS);
                macro.AddAction(new ActionRepeat(TIME_REPEAT_SOUND_MS, macroName, soundMacroName ));
                Macros.Add(macroName, macro);
                trigger = new TriggerKeyboard(triggerKey, repeatMacroName);
                trigger.AddModifier(Keys.LMenu);
                Tube.Triggers.Add(trigger);

                // Stopper macro and trigger
                macro = new Macro(macroName = stopMacroName, TIME_DELAY_GLOBAL_MS);
                macro.AddAction(new ActionCancel(repeatMacroName));
                Macros.Add(macroName, macro);
                trigger = new TriggerKeyboard(triggerKey, macroName);
                trigger.AddModifier(Keys.LControlKey);
                Tube.Triggers.Add(trigger);
            }

            //
            // Macro bound to mouse button X1 / X2
            //
            macro = new Macro(macroName = "F10", 0);
            macro.AddAction(new ActionKey(50, VirtualKeyCode.F10, ButtonStates.Both));
            Macros.Add(macroName, macro);

            // Same macro bound to two triggers            
            trigger = new TriggerKeyboard(Keys.XButton1, macroName);
            Tube.Triggers.Add(trigger);
            trigger = new TriggerKeyboard(Keys.XButton2, macroName);
            Tube.Triggers.Add(trigger);

            // 
            // Toggle - hold SPACE key every other time it is pressed 
            //
            macro = new Macro(macroName = "space-press", TIME_DELAY_GLOBAL_MS);
            macro.AddAction(new ActionKey(0, VirtualKeyCode.SPACE, ButtonStates.Press));
            Macros.Add(macroName, macro);

            macro = new Macro(macroName = "space-release", TIME_DELAY_GLOBAL_MS);
            macro.AddAction(new ActionKey(0, VirtualKeyCode.SPACE, ButtonStates.Release));
            Macros.Add(macroName, macro);

            // trigger will ripple fire these macros, creating a toggle for the space bar
            // press LControl + 
            trigger = new TriggerKeyboard(
                Keys.LWin, ButtonStates.Press, new List<string> {"space-press", "space-release"}, true);
            trigger.AddModifier(Keys.LControlKey);
            Tube.Triggers.Add(trigger);

            // 
            // Create mouse movement
            //
            macro = new Macro(macroName = "mouse-nudge-left", TIME_DELAY_GLOBAL_MS);
            macro.AddAction(
                new ActionMouse(
                    0, 
                    ActionMouse.CoordinateMode.RELATIVE,
                    -1, 0));
            Macros.Add(macroName, macro);
            trigger = new TriggerKeyboard(Keys.Left, macroName);
            trigger.AddModifier(Keys.LMenu);
            Tube.Triggers.Add(trigger);

            macro = new Macro(macroName = "mouse-center", TIME_DELAY_GLOBAL_MS);
            macro.AddAction(
                new ActionMouse(
                    0, 
                    ActionMouse.CoordinateMode.ABSOLUTE, 
                    65535 / 2, 65535 / 2));
            Macros.Add(macroName, macro);
            trigger = new TriggerKeyboard(Keys.Home, macroName);
            trigger.AddModifier(Keys.LMenu);
            Tube.Triggers.Add(trigger);

            macro = new Macro(macroName = "mouse-origin", TIME_DELAY_GLOBAL_MS);
            macro.AddAction(
                new ActionMouse(
                    0, 
                    ActionMouse.CoordinateMode.PIXEL, 
                    1, 1));
            Macros.Add(macroName, macro);
            trigger = new TriggerKeyboard(Keys.End, macroName);
            trigger.AddModifier(Keys.LMenu);
            Tube.Triggers.Add(trigger);

            macro = new Macro(macroName = "mouse-click-nomove", TIME_DELAY_GLOBAL_MS);
            macro.AddAction(
                new ActionMouse(
                    0,
                    ActionMouse.ClickType.CLICK,
                    MouseButton.LeftButton));
            Macros.Add(macroName, macro);
            trigger = new TriggerKeyboard(Keys.Delete, macroName);
            trigger.AddModifier(Keys.LMenu);
            Tube.Triggers.Add(trigger);

            macro = new Macro(macroName = "cancel-all", TIME_DELAY_GLOBAL_MS);
            macro.AddAction(new ActionCancel("*"));
            Macros.Add(macroName, macro);

            trigger = new TriggerKeyboard(Keys.C, macroName);
            trigger.AddModifier(Keys.LControlKey);
            Tube.Triggers.Add(trigger);
            trigger = new TriggerKeyboard(Keys.C, macroName);
            trigger.AddModifier(Keys.RControlKey);
            Tube.Triggers.Add(trigger);

            // 
            // Mouse safety 
            //

            // Engage mouse safety
            macro = new Macro(macroName = "lock-mouse-engage", 0);
            macro.AddAction(new ActionMouseLock(MouseLocks.Locked));
            macro.AddAction(new ActionSound(TIME_DELAY_GLOBAL_MS, "sound_click_latch.wav"));
            Macros.Add(macroName, macro);

            // Example trigger optimal for controller that has a toggle switch or POV hat
            TriggerController triggerController = new TriggerControllerButton(
                NAME_CONTROLLER, BUTTON_CONTROLLER, ButtonValues.Press, macroName);
            Tube.Triggers.Add(triggerController);
            triggerController = new TriggerControllerPOV(NAME_CONTROLLER, POVStates.Up, macroName);
            Tube.Triggers.Add(triggerController);

            // Disengage mouse safety
            macro = new Macro(macroName = "lock-mouse-disengage", 0);
            macro.AddAction(new ActionMouseLock(MouseLocks.Unlocked));
            macro.AddAction(new ActionSound(TIME_DELAY_GLOBAL_MS, "sound_click_latch.wav"));
            Macros.Add(macroName, macro);

            triggerController = new TriggerControllerButton(
                NAME_CONTROLLER, BUTTON_CONTROLLER, ButtonValues.Release, macroName );
            Tube.Triggers.Add(triggerController);
            triggerController = new TriggerControllerPOV(NAME_CONTROLLER, POVStates.Down, macroName);
            Tube.Triggers.Add(triggerController);

            // Toggle mouse safety via keyboard
            List<string> macroNames = new List<string>
            {
                "lock-mouse-engage",
                "lock-mouse-disengage"
            };
            trigger = new TriggerKeyboard(Keys.L, macroNames);
            trigger.AddModifier(Keys.LControlKey);
            Tube.Triggers.Add(trigger);
            trigger = new TriggerKeyboard(Keys.L, macroNames);
            trigger.AddModifier(Keys.RControlKey);
            Tube.Triggers.Add(trigger);

            TriggerControllerAxis triggerAxis = new TriggerControllerAxis(
                NAME_CONTROLLER, 
                ControllerAxis.RotationZ,
                3,          // Number of bands
                1,          // Deadzone band index
                12000,      // Deadzone size
                new List<Keys>(new Keys[] {Keys.E, Keys.None, Keys.D}),
                new List<string>(new string[] { "sound-servomotor", "sound-dice", "sound-estop"})
                );

            Tube.Triggers.Add(triggerAxis);

            //
            // Spawn external commands
            //

            (string soundMacroName, string cmd, Keys triggerKey)[] commandTable =
            {
            //  name        command         trigger
                ("dir",     "dir",          Keys.D),            // something with output
                ("success", "success.cmd",  Keys.Add),          // guaranteed exit code 0
                ("error",   "error.cmd",    Keys.Subtract),     // guaranteed exit code 1
            };

            foreach ((string name, string cmd, Keys triggerKey) in commandTable)
            {
                macro = new Macro(name, 0)
                .AddAction(new ActionCmd(TIME_DELAY_GLOBAL_MS, cmd)
                {
                    ErrorSoundPath = "fail.wav",
                    FinishedSoundPath = "sound_servomotor.wav"
                });
                Macros.Add(name, macro);
                trigger = new TriggerKeyboard(triggerKey, name);
                trigger.AddModifier(Keys.LControlKey);
                Tube.Triggers.Add(trigger);
            }
        }

        public static void GenerateKeyRemaps()
        {
            // Sunless skies (and other games) won't allow binding to shift key
            // Mapping A to Shift allows binding game functions to that instead.
            AddRemap("LShift", "A", "skies.exe");

            // Evil evil swap for people typing into notepad!  
            // Easy way to do quick functional test of remapping by process.
            AddRemap("B", "V", PROCESS_NAME_NOTEPAD);
            AddRemap("V", "B", PROCESS_NAME_NOTEPAD);

            // KILL WASD
            // 
            // Remap movement block from this:
            // 
            // QWErtyuiop
            // ASDfghjkl;
            // 
            // to this:
            //
            // rQWEtyuiop
            // fASDghjkl;

            // WASD block will slide to right so this displaces R and F to make room
            AddRemap("Q", "R", PROCESS_NAME_WASD);
            AddRemap("A", "F", PROCESS_NAME_WASD);

            // Remap WASD movement block to ESDF (plus rotation keys)
            AddRemap("W", "Q", PROCESS_NAME_WASD);
            AddRemap("E", "W", PROCESS_NAME_WASD);
            AddRemap("R", "E", PROCESS_NAME_WASD);
            AddRemap("S", "A", PROCESS_NAME_WASD);
            AddRemap("D", "S", PROCESS_NAME_WASD);
            AddRemap("F", "D", PROCESS_NAME_WASD);
        }
    }
}
