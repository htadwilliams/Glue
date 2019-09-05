# Keyboard remapping and macroing tool for Windows written in C#.

WARNING: This globally hooks keyboard/mouse input and logs to file if DEBUG mode is enabled. Don't run in DEBUG mode if you don't want your keys logged!

Originally written in C++ and used to play EverQuest for over 10 years, this a rewrite in C# including low-level keyboard and mouse hooks (with lots of native windows API calls).

Better than placing a stack of nickels on keys, or using an oscillating desk fan to hit a button repeatedly!

# Usage

Glue.exe [macros.glue]

If optionally specified script file does not exist, one will be created with examples of Glue's features.
Default file name if not specified is "macros.glue".

# Example Script

## Example macros and triggers
* Ctrl-C cancels all queued actions, loops, etc.
* Ctrl-Z will play a sound after a delay.
* Ctrl-S "ripple fire" example that alternates between playing two sounds. If the sound is already playing it will be restarted.
* Ctrl-Space toggles the space bar so it's held with every other press. Doesn't cause windows key repeat messages while held down.
* MBX1 / MBX2 (mouse side buttons) trigger a press of F10.
* Alt-, Alt-. Alt-/ begin looping different sounds every N seconds.
* Ctrl-, Ctrl-. Ctrl-/ cancel individual sound loops. 

## Example key remapping
* V and B are swapped if typing into notepad. Note that this is applied to any .exe with "notepad" in the name, so this includes things like Notepad++. Insert evil laugh here.
* LEFT SHIFT types an A if input window process name contains "skies.exe" so it can be mapped in Sunless Skies (and easily change for other games where SHIFT can't be remapped).
* WASD is EVIL! WASD and typical rotation keys Q/E are remapped to ESDF W/R (R/F slide to left over Q/A to make room) for "fallout4.exe". Change to your .exe name to use.

# Dependencies

## NuGet managed dependencies
* SharpDX and SharpDX.DirectInput used for DirectX manipulation of gaming input devices: http://sharpdx.org/ 
* NewtonSoft.Json for serialization of macros, triggers, etc (except user preferences).

##Other dependencies
* Priority queue implementation is courtesy of BlueRaja.admin@gmail.com (https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp). The application creates a thread that monitors a queue of scheduled macro action events to fire (See EventQueue class).
* Windows Input simulator to wrap Windows API SendInput https://archive.codeplex.com/?p=inputsimulator and provide all the native constants needed. See https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-sendinput.

# Completed features

##Core

* Macros may be triggered by multiple key and key chording combinations.
* Macros may be looped or canceled.
* Keys may be remapped, with optional filtering by process (running .exe) name.
* Keys and buttons may be toggled - held until released.
* Multiple macros may be "ripple fired" from a single trigger.
* Mouse movement and clicking supports multiple coordinate modes (relative, absolute, and normalized 0-65,535).
* Macro actions include:
  - keyboard keys
  - mouse buttons (clicking with or without movement)
  - mouse movement (with or without clicking buttons)
  - typing long strings
  - playing sounds
  - repeating other macros
  - cancelling actions by name (or all actions)

##GUI

* Logging of keys and macro events (useful for creating new macros).
* View -> Button States shows queued actions, updated as they are scheduled or canceled.
* View -> Queued Actions displays set of currently pressed keys.
* Closes to system tray for unobtrusive operation. See https://github.com/htadwilliams/TrayTemplate for re-usable code.
* Status bar display of mouse coordinates and click logging, in native or normalized coordinates (useful for building resolution-independent mouse control macros).
* Edit -> Macros partially implemented and can be used to view actions and macros.
* Ctrl-Click on main form close box exits application instead of minimizing to system tray.

# Feature TODO list

## Core

* Game device button triggers, macro events, and button remapping.
* Queue view should update every second if nothing else is happening.
* Remote control - trigger events via network client (most likely REST interface).
* Triggers and actions may be filtered by target process name the same way key remapping works.

## GUI

* GUI shows current set of keyboard remap entries.
* GUI shows current set of triggers.
* GUI for recording / editing macros.
* GUI for remapping keys / buttons.
* GUI allows binding of macros to triggers and selecting trigger keys.
* GUI Add mouse button state to buttons view.