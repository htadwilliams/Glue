# Input remapping and macroing tool for Windows.

Originally written in C++ around 1999 and used to play EverQuest for over 10 years. This a total rewrite in C# just for fun including low-level keyboard and mouse hooks (with lots of native windows API calls). 

Better than placing a stack of nickels on keys, or using an oscillating desk fan to hit a button repeatedly.

Unless you're a developer that wants a tool written in C# you can modify or get code / ideas from, you should use AutoHotkey instead of this. But you probably won't find this anyway.

# Usage

Glue.exe [macros.glue]

If optionally specified file does not exist, one will be created with examples of Glue's features.
Default file name if not specified is "macros.glue".

# Example Content

The application by default attempts to read a file called macros.glue from its working directory. The file name / path may be optionally specified as a command-line parameter. If this file isn't found one will be created with example macros, triggers, and remapping entries. Files may also be opened from the menu File -> Open.

The file is a JSON collection of Macro, Trigger, and Remap entries.

Triggers detect inputs and fires one or more Macros, which are collections of Actions. Actions may control keyboard keys or other input devices, play sounds, invoke other macros in order to loop or cancel them, and more. Actions are scheduled and queued in an output priority queue. The action queue may be viewed from the menu View -> Queued Actions.

Sound paths in the example content are relative to the glue directory, although sounds may be anywhere. Only .wav files are supported. 

CMD.exe paths for spawned processes are also relative to the glue directory. The example one is in the glue directory.

## Example content Macros and Triggers: 

| Trigger                     | Macro name(s)                   | Notes                                             |
| --------------------------- | ------------------------------- | ------------------------------------------------- |
| LControl + C                | cancel-all                      | Cancels all queued actions, loops, etc. |
| LControl + Z                | delayed-action                   | Plays a sound and types the enter key after a delay of three seconds. |
| LControl + S                | sound-servomotor, sound-ahha    | Ripple fire example that alternates between two macros. If a sound is already playing it will be stopped and the next one played / restarted. |
| LControl + Win              | space-press, space-release      | Toggles the space bar so it's held with every other press. Note: Doesn't cause windows to repeat spaces while toggled on. |
| XButton1 (mouse)            | F10                             | Press and release F10 key (not a remap as button still activates). |
| Alt + ,                     | repeat-sound-dice sound-dice    | Starts a loop that plays a sound every few seconds. |
| Alt + .                     | repeat-sound-tock sound-tock    | " |
| Alt + /                     | repeat-sound-estop sound-estop  | " |
| LControl + ,                | stop-sound-dice                 | Cancels all scheduled dice sounds and ends the loop. |
| LControl + .                | stop-sound-tock                 | " |
| LControl + /                | stop-sound-estop                | " |
| LControl + L                | mouse-lock mouse-unlock         | Toggles mouse pointer lock. Mouse will not move while locked. |
| LControl + D                | dir                             | Uses ActionCMD to spawn an external process and desplay the results in Glue's window. |

Note: In example, the Warthog Throttle controller EAC Arm/Off toggle switch (button 23) controls mouse lock. The button number and controller name are easily tweakable after the file is created. This is ideal for controllers with toggle switches.

## Examples of key remapping:
* V and B are swapped if typing into notepad. Note that this is applied to any .exe with "notepad" in the name, so this includes things like Notepad++. Insert evil laugh here.
* LEFT SHIFT types an A if input window process name contains "skies.exe" so it can be mapped in Sunless Skies (and easily change for other games where SHIFT can't be remapped).
* WASD is EVIL! WASD and typical rotation keys Q/E are remapped to ESDF W/R (R/F slide to left over Q/A to make room) for "fallout4.exe". Change to your .exe name to use.

## Keyboard and Mouse Interceptor driver 

### Interceptor reference URLs

http://www.oblita.com/interception.html

https://github.com/oblitum/Interception

https://github.com/jasonpang/Interceptor

### Notes

The interceptor driver will optionally be used to simulate input if it's installed. There are a few bugs and remapping is a WIP. If the driver isn't installed, Glue will use SendInput() to inject keystrokes.

The driver uses DeviceIoControl() calls to inject input. These talk whichever existing keyboard or mouse driver is detected first by the interceptor driver, so on startup there is a race condition for the devices. 

Glue uses the jasonpang wrapper to talk to the Interceptor driver. The wrapper does this by calling into the DLL provided by the interceptor driver. 

# Dependencies

## Binary dependencies

Interceptor.dll from https://github.com/jasonpang/Interceptor. 
Log4net.dll 

## NuGet managed dependencies
* SharpDX and SharpDX.DirectInput http://sharpdx.org/ 
* NerfDX facade for SharpDX.DirectInput https://github.com/htadwilliams/NerfDX 
* NewtonSoft.Json for de/serialization of content https://www.newtonsoft.com/json

## Other code forked directly into project
* Priority queue implementation is courtesy of BlueRaja.admin@gmail.com (https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp). The application creates a thread that monitors a queue of scheduled macro action events to fire (See EventQueue class).
* Windows Input simulator to wrap Windows API SendInput https://archive.codeplex.com/?p=inputsimulator and provide all the native constants needed. See https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-sendinput.

# Completed features

## Core macroing:

* All macros, triggers, and remapping persist via JSon de/serialization.
* Macros may be triggered by multiple key and key chording combinations.
* Macros may be looped or canceled.
* Keys may be remapped, with optional filtering by process (running .exe) name.
* Macros may be triggered by a variety of keyboard, mouse, or game controller input combinations, with optional filtering by process name.
* Keys and buttons may be toggled - held until released.
* Multiple macros may be "ripple fired" by repeating a single trigger.
* Mouse movement and clicking supports multiple coordinate modes (relative, absolute, and normalized 0-65,535).
* Macro actions include:
  - keyboarding
  - mouse buttons (clicking with or without movement)
  - mouse movement (with or without clicking buttons)
  - playing sounds
  - repeating other macros
  - cancelling queued actions by name (or all actions)
  - toggle mouse input lock e.g. don't move while I sip (spill) this <insert liquid here>
 
## GUI:

* Logging of keys and macro events (useful for creating new macros).
* View -> Button States displays set of currently pressed keys and mouse buttons.
* View -> Queued Actions shows queued actions, updated as they are scheduled or canceled. 
* Closes to system tray for unobtrusive operation. See https://github.com/htadwilliams/TrayTemplate for re-usable code.
* Status bar display of mouse coordinates and click logging, in native or normalized coordinates (useful for building resolution-independent mouse control macros).
* Edit -> Macros partially implemented and can be used to view actions and macros.
* Ctrl-Click on main form close box exits application instead of minimizing to system tray.

# Feature TODO list

## Core TODO
 - [ ] Action that is a script interpreter for other actions. Adds interface similar to AHK for Glue.
 - [ ] Move Log4net dependency to Nuget.
 - [x] Game controller button triggers.
 - [ ] Game controller triggers support chording (with combinations of other controller or mouse buttons and keyboard keys.
 - [X] Game controller hat triggers.
 - [ ] Game controller force feedback events.
 - [X] Game controller axis movement triggers.
 - [x] Queue view should update every second if nothing else is happening.
 - [ ] Remote control - trigger events via network client (most likely REST interface).
 - [x] Triggers and actions may be filtered by target process name the same way keyboard remapping does. 
 - [x] Mouse "safety" - freeze mouse cursor position (but not buttons)
 - [ ] Add Event that allows a Macro to call another Macro. Use case: unlock mouse and release a key from more than one macro. 

## GUI TODO
 - [X] Add mouse button state to buttons view.
 - [X] New view for controller plugged state and controller info.
 - [ ] New view for controller buttons pressed.
 - [ ] View / edit keyboard remapping.
 - [ ] View / edit triggers.
 - [ ] View / editing macros.
 - [ ] Macro recording.
