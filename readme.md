# Keyboard remapping and macroing tool for Windows.

WARNING: This globally hooks keyboard/mouse input and logs to file if DEBUG mode is enabled. Don't run in DEBUG mode if you don't want your keys logged!

Originally written in C++ around 1999 and used to play EverQuest for over 10 years. This a total rewrite in C# just for fun including low-level keyboard and mouse hooks (with lots of native windows API calls). 

Better than placing a stack of nickels on keys, or using an oscillating desk fan to hit a button repeatedly.

Unless you're a developer that wants a tool written in C# you can modify or get code / ideas from, you should use AutoHotkey instead of this. But you probably won't find this anyway.

# Usage

Glue.exe [macros.glue]

If optionally specified file does not exist, one will be created with examples of Glue's features.
Default file name if not specified is "macros.glue".

# Example Content

The application by default attempts to read MACROS.GLUE from its working directory. The file name / path may be optionally specified as a command-line parameter. If this file isn't found one will be created with example macros, triggers, and remapping entries.

In Glue, a Trigger detects inputs and fires Macros, which are collections of Actions. Actions may control keyboard keys or other input devices, play sounds, invoke other macros in order to loop or cancel them, and more.

## Example content Macros and Triggers:
* Ctrl-C cancels all queued actions, loops, etc.
* Ctrl-Z will play a sound and type a key after a delay.
* Ctrl-S "ripple fire" example that alternates between playing two sounds. If a sound is already playing it will be stopped and the next one played / restarted.
* Ctrl-Space toggles the space bar so it's held with every other press. Doesn't cause windows key repeat messages while held down.
* MBX1 / MBX2 (mouse side buttons) trigger a press of F10.
* Alt-, Alt-. Alt-/ begin looping different sounds every N seconds.
* Ctrl-, Ctrl-. Ctrl-/ cancel individual sound loops. 
* Ctrl-L toggles mouse pointer lock. 
* Warthog Throttle controller EAC Arm/Off toggle switch (button 23) controls mouse lock. The button number and controller name are easily tweakable after the file is created. This is ideal for controllers with toggle switches.

## Examplse of key remapping:
* V and B are swapped if typing into notepad. Note that this is applied to any .exe with "notepad" in the name, so this includes things like Notepad++. Insert evil laugh here.
* LEFT SHIFT types an A if input window process name contains "skies.exe" so it can be mapped in Sunless Skies (and easily change for other games where SHIFT can't be remapped).
* WASD is EVIL! WASD and typical rotation keys Q/E are remapped to ESDF W/R (R/F slide to left over Q/A to make room) for "fallout4.exe". Change to your .exe name to use.

# DEPENDENCIES

## NuGet managed dependencies:

* SharpDX and SharpDX.DirectInput used for DirectX manipulation of gaming input devices: http://sharpdx.org/ 
* NewtonSoft.Json for serialization of macros, triggers, etc (except user preferences).

## Other dependencies:

* Priority queue implementation is courtesy of BlueRaja.admin@gmail.com (https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp). The application creates a thread that monitors a queue of scheduled macro action events to fire (See EventQueue class).
* Windows Input simulator to wrap Windows API SendInput https://archive.codeplex.com/?p=inputsimulator and provide all the native constants needed. See https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-sendinput.

# Completed features

## Core macroing:

* Macros may be triggered by multiple key and key chording combinations.
* Macros may be looped or canceled.
* Keys may be remapped, with optional filtering by process (running .exe) name.
* Keys and buttons may be toggled - held until released.
* Multiple macros may be "ripple fired" from a single trigger.
* Mouse movement and clicking supports multiple coordinate modes (relative, absolute, and normalized 0-65,535).
* Macro actions include:
  - keyboarding
  - mouse buttons (clicking with or without movement)
  - mouse movement (with or without clicking buttons)
  - playing sounds
  - repeating other macros
  - cancelling queued actions by name (or all actions)
  - toggle mouse input lock

## GUI:

* Logging of keys and macro events (useful for creating new macros).
* View -> Button States displays set of currently pressed keys.
* View -> Queued Actions shows queued actions, updated as they are scheduled or canceled. 
* Closes to system tray for unobtrusive operation. See https://github.com/htadwilliams/TrayTemplate for re-usable code.
* Status bar display of mouse coordinates and click logging, in native or normalized coordinates (useful for building resolution-independent mouse control macros).
* Edit -> Macros partially implemented and can be used to view actions and macros.
* Ctrl-Click on main form close box exits application instead of minimizing to system tray.

# Feature TODO list

## Core TODO
 - [x] Game controller button triggers.
 - [ ] Game controller triggers support chording (with combinations of other controller or mouse buttons and keyboard keys.
 - [X] Game controller hat triggers (with inherited chording support).
 - [ ] Game controller force feedback events.
 - [ ] Game controller axis movement triggers.
 - [ ] Game controller button remapping.
 - [x] Queue view should update every second if nothing else is happening.
 - [ ] Remote control - trigger events via network client (most likely REST interface).
 - [ ] Triggers and actions may be filtered by target process name the same way keyboard remapping does. 
 - [ ] Game controller axis remapping.
 - [x] Mouse "safety" - freeze mouse cursor position (but not buttons)

## GUI TODO
 - [X] Add mouse button state to buttons view.
 - [ ] New view for controller buttons pressed.
 - [ ] View / edit keyboard remapping.
 - [ ] View / edit triggers.
 - [ ] View / editing macros.
 - [ ] Macro recording.
