Glue

WARNING: This globally hooks keyboard and logs to file if DEBUG mode is enabled. Don't run in DEBUG mode if you don't want your keys logged!

An input remapping and macroing tool.  Originally written in C++, this a rewrite in C# including low-level keyboard and mouse hooks (with lots of native windows API calls).

The application by default attempts to read MACROS.JSON from its working directory. The file name / path may be optionally specified as a command-line parameter.  If this file isn't found or a command-line isn't specified, one will be created with default mappings and macros. 

EXAMPLE MACROS:
* Ctrl-C (making sure input focus is not set to console window or application will exit) will type a string of characters immediately with short delay between presses and releases.
* Ctrl-Z will immediately type an R followed by a Q and ENTER after a delay of ~4 seconds. This is useful for demonstrating asynchronous scheduling of output events.
* Ctrl-S plays a sound asynchronously. If the sound is already playing it will be restarted. 

EXAMPLE REMAPS:
* LEFT SHIFT types an A if input window process name contains "skies.exe" so it can be mapped in Sunless Skies (and easily change for other unity games where SHIFT can't be remapped).
* WASD are remapped to ESDF (with E<->W and F<->A) for "notepad". Change to your .exe name to use.  WASD is EVIL!
* Swap V and B if typing into notepad.exe.

NOTES:

The application creates a thread that monitors a priority queue of scheduled macro events to fire (See EventQueue class).  The priority queue implementation is courtesy of BlueRaja.admin@gmail.com (https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp). 

DirectX is used to enumerate input devices on startup (Use NYI though).

Could be improved by hooking the action queue to a thread pool so that macro events can be processed asynchronously.  Right now macro events must not block and do things like press keys, play sounds, or eat key events (for remapping).

Feature TODO list:

* Add queue display window for fun and debugging (shows keyboard state, and representation of events queued for future work).
* Toggle keys (single key toggles and multi-key exclusive toggles similar to radio buttons on a form).
* Macro looping start/stop events.
* Game device button triggers, macro events, and button remapping.
* Remote control - trigger events via network client (most likely REST interface).

BONUS: 
* GUI for recording / editing macros.
* GUI for remapping keys / buttons.
* Keyboard / button map diagram output.
