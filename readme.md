Glue

WARNING: This globally hooks keyboard and logs to file if DEBUG mode is enabled. Don't run in DEBUG mode if you don't want your keys logged!

An input remapping and macroing tool for Windows.  Originally written in C++ and used to play EverQuest for over 10 years, this a rewrite in C# including low-level keyboard and mouse hooks (with lots of native windows API calls).

The application by default attempts to read MACROS.GLUE from its working directory. The file name / path may be optionally specified as a command-line parameter.  If this file isn't found one will be created with example macros, triggers, and remapping entries.

DEFAULT EXAMPLE MACROS AND TRIGGERS:
* Ctrl-C (making sure input focus is not set to console window or application will exit) will type a string of characters immediately with short delay between presses and releases.
* Ctrl-Z will immediately type an R followed by a Q and ENTER after a delay of ~4 seconds. This is useful for demonstrating asynchronous scheduling of output events.
* Ctrl-S plays a sound asynchronously. If the sound is already playing it will be restarted. 
* Space is a toggle and will be held down with every other press. This won't show in things like Notepad as the held key won't trigger Windows key repeats, but works very well in games.

DEFAULT EXAMPLE REMAPS:
* LEFT SHIFT types an A if input window process name contains "skies.exe" so it can be mapped in Sunless Skies (and easily change for other unity games where SHIFT can't be remapped).
* WASD are remapped to ESDF (with E<->W and F<->A) for "notepad". Change to your .exe name to use. WASD is EVIL! 
* Swap V and B if typing into notepad.exe.

DEPENDENCIES:

NuGet managed dependencies:

SharpDX and SharpDX.DirectInput used for DirectX manipulation of gaming input devices: http://sharpdx.org/ 
NewtonSoft.Json for serialization of macros, triggers, etc (except user preferences).

Other dependencies:

Priority queue implementation is courtesy of BlueRaja.admin@gmail.com (https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp). The application creates a thread that monitors a queue of scheduled macro action events to fire (See EventQueue class).

Windows Input simulator to wrap Windows API SendInput https://archive.codeplex.com/?p=inputsimulator and provide all the native constants needed. See https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-sendinput.

Feature / TODO list:

* Add queue display window for fun and debugging (shows keyboard state, and representation of events queued for future work).
* Exclusive toggle keys (last key pressed is held and all others released).
* Macro looping start/stop events.
* Game device button triggers, macro events, and button remapping.
* Remote control - trigger events via network client (most likely REST interface).

BONUS: 
* GUI for recording / editing macros.
* GUI for remapping keys / buttons.
* Keyboard / button map diagram output showing remapped items.
