Glue

A macro tool.  Originally written in C++, this is my latest attempt to write it in C# including low-level keyboard and mouse hooks (with native windows API calls).

Press Ctrl-C (making sure input focus is not set to console window or application will exit) to fire hard-coded macro which only causes logged output until SendInput() or equivelent is hooked up. 

WARNING: Globally hooking the keyboard and logging the results can be used for evil! This code does just that and can be seriously abused.  Please don't do that!
