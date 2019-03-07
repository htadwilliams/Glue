Glue

WARNING: This globally hooks keyboard and logs to file. Don't run in DEBUG mode if you don't want your keys logged!

A macro tool.  Originally written in C++, this is my latest attempt to write it in C# including low-level keyboard and mouse hooks (with native windows API calls).

The application by default attempts to read  a JSON file MACROS.JSON from it's working directory. The file name / path may be optionally specified as a command-line parameter.  If this file isn't found or a command-line isn't specified, one will be created with the following mappings: 

* Ctrl-C (making sure input focus is not set to console window or application will exit) will type a string of characters immediately with short delay between presses and releases.

* Ctrl-Z will immediately type an R followed by a Q and ENTER after a delay of ~4 seconds. This is useful for demonstrating asynchronous scheduling of output events.

* Ctrl-S plays a sound immediately. If the sound is already playing it will be restarted.

Keyboard remapping is supported but remaps are hard-coded in GlueTube.exe right now. They need to be added to serialization. These can be found and changed in GlueTube.cs - look for the LoadMacros() method. Remapping is process aware (they can be defined globally or only defined for a given process name).

LEFT SHIFT types an A if input window contains "skies.exe" so I can use it for altrnate fire in Sunless Skies (and easily changge for other unity games where shift can't be remapped).

NOTES:

The application creates a thread that monitors a priority queue of scheduled macro events to fire (See EventQueue class).  The priority queue implementation is courtesy of BlueRaja.admin@gmail.com (https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp). 

DirectX is used to enumerate input devices on startup.

Could be improved by hooking the action queue to a thread pool so that macro events can be processed asynchronously.  Right now macro events must not block and do things like press keys, play sounds, or eat key events (for remapping).

Next steps include:

* Move to Trello or something for feature tracking and move all those damn TODO comments there.
* Move hard-coded key remapping to JSON file.
* Separate macros from triggers (more than one trigger should be able to fire the same macro without duplicating the macro).

Feature TODO list:

* Add queue display window for fun and debugging (shows keyboard state, and representation of events queued for future work).
* Macro looping start/stop events.
* Macros may be triggered from key release events.
* Game device button triggers, macro events, and button remapping.
* Remote control - trigger events via network client (most likely REST interface).

BONUS: 
* GUI for recording / editing macros.
* GUI for remapping keys / buttons.
* Keyboard / button map diagram output.
