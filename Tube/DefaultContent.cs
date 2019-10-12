using Glue.Actions;
using Glue.Events;
using Glue.Triggers;
using System.Collections.Generic;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;
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
        private const int TIME_DELAY_ACTION                 = 8 * 1000;       // Bound to trigger for single delayed action

        public static void Generate()
        {
            string macroName;
            //
            // Create macro with several actions bound to CTRL-Z
            //
            Macro macro = new Macro(macroName = "delayed-action", TIME_DELAY_ACTION) 
                .AddAction(new ActionSound(TIME_DELAY_GLOBAL_MS,  "ahha.wav"))
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
            // Create a trigger that alternates between macros - ripple fire example
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
            macro = new Macro(macroName = "toggle-down", TIME_DELAY_GLOBAL_MS);
            macro.AddAction(new ActionKey(0, VirtualKeyCode.SPACE, ButtonStates.Press));
            Macros.Add(macroName, macro);

            macro = new Macro(macroName = "toggle-up", TIME_DELAY_GLOBAL_MS);
            macro.AddAction(new ActionKey(0, VirtualKeyCode.SPACE, ButtonStates.Release));
            Macros.Add(macroName, macro);

            trigger = new TriggerKeyboard(
                Keys.Space, ButtonStates.Both, new List<string> {"toggle-down", null, "toggle-up", null}, true);
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

            // TODO Trigger mod keys should allow logical combinations e.g. (LControlKey | RControlKey) 
            trigger = new TriggerKeyboard(Keys.C, macroName);
            trigger.AddModifier(Keys.LControlKey);
            Tube.Triggers.Add(trigger);
            trigger = new TriggerKeyboard(Keys.C, macroName);
            trigger.AddModifier(Keys.RControlKey);
            Tube.Triggers.Add(trigger);

            // Toggle mouse safety
            macro = new Macro(macroName = "lock-mouse-toggle", 0);
            macro.AddAction(new ActionMouseLock(LockAction.Toggle));
            macro.AddAction(new ActionSound(TIME_DELAY_GLOBAL_MS, "sound_click_latch.wav"));
            Macros.Add(macroName, macro);

            trigger = new TriggerKeyboard(Keys.L, macroName);
            trigger.AddModifier(Keys.LControlKey);
            Tube.Triggers.Add(trigger);
            trigger = new TriggerKeyboard(Keys.L, macroName);
            trigger.AddModifier(Keys.RControlKey);
            Tube.Triggers.Add(trigger);

            // 
            // Mouse safety - example optimal for controller that 
            // has a toggle switch
            //

            // Engage mouse safety
            macro = new Macro(macroName = "lock-mouse-engage", 0);
            macro.AddAction(new ActionMouseLock(LockAction.Lock));
            macro.AddAction(new ActionSound(TIME_DELAY_GLOBAL_MS, "sound_click_latch.wav"));
            Macros.Add(macroName, macro);
            
            TriggerController triggerController = new TriggerControllerButton(
                 NAME_CONTROLLER, BUTTON_CONTROLLER, ButtonValues.Press, macroName);
            Tube.Triggers.Add(triggerController);

            triggerController = new TriggerControllerPOV(NAME_CONTROLLER, POVStates.Up, macroName);
            Tube.Triggers.Add(triggerController);

            // Disengage mouse safety
            macro = new Macro(macroName = "lock-mouse-disengage", 0);
            macro.AddAction(new ActionMouseLock(LockAction.Unlock));
            macro.AddAction(new ActionSound(TIME_DELAY_GLOBAL_MS, "sound_click_latch.wav"));
            Macros.Add(macroName, macro);
            
            triggerController = new TriggerControllerButton(
                NAME_CONTROLLER, BUTTON_CONTROLLER, ButtonValues.Release, macroName );
            Tube.Triggers.Add(triggerController);
            triggerController = new TriggerControllerPOV(NAME_CONTROLLER, POVStates.Down, macroName);
            Tube.Triggers.Add(triggerController);

            // Sunless skies (and other games) won't allow binding to shift key
            // Mapping A to Shift allows binding game functions to that instead.
            AddRemap(VirtualKeyCode.LSHIFT, VirtualKeyCode.VK_A, "skies.exe");

            // Evil evil swap for people typing into notepad!  
            // Easy way to do quick functional test of remapping by process.
            AddRemap(VirtualKeyCode.VK_B, VirtualKeyCode.VK_V, PROCESS_NAME_NOTEPAD);
            AddRemap(VirtualKeyCode.VK_V, VirtualKeyCode.VK_B, PROCESS_NAME_NOTEPAD);

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
            AddRemap(VirtualKeyCode.VK_Q, VirtualKeyCode.VK_R, PROCESS_NAME_WASD);
            AddRemap(VirtualKeyCode.VK_A, VirtualKeyCode.VK_F, PROCESS_NAME_WASD);

            // Remap WASD movement block to ESDF (plus rotation keys)
            AddRemap(VirtualKeyCode.VK_W, VirtualKeyCode.VK_Q, PROCESS_NAME_WASD);
            AddRemap(VirtualKeyCode.VK_E, VirtualKeyCode.VK_W, PROCESS_NAME_WASD);
            AddRemap(VirtualKeyCode.VK_R, VirtualKeyCode.VK_E, PROCESS_NAME_WASD);
            AddRemap(VirtualKeyCode.VK_S, VirtualKeyCode.VK_A, PROCESS_NAME_WASD);
            AddRemap(VirtualKeyCode.VK_D, VirtualKeyCode.VK_S, PROCESS_NAME_WASD);
            AddRemap(VirtualKeyCode.VK_F, VirtualKeyCode.VK_D, PROCESS_NAME_WASD);
        }
    }
}
