using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Glue
{
    class Keyboard
    {
        private static readonly Key[] KEYS = 
        {
            //      Virtual key code            Display         Bindable
            new Key(Keys.None,					"",				false),
            new Key(Keys.LButton,				"",				false),
            new Key(Keys.RButton,               "",             false),
            new Key(Keys.Cancel,				"",				false),
            new Key(Keys.MButton,               "",             false),
            new Key(Keys.XButton1,              "",             false),
            new Key(Keys.XButton2,              "",             false),
            new Key(Keys.Back,					"Backspace",    true),
            new Key(Keys.Tab,                   "",             true),
            new Key(Keys.LineFeed,              "",             false),
            new Key(Keys.Clear,                 "",             false),
            new Key(Keys.Return,                "Return",		true),
            new Key(Keys.ShiftKey,              "",             false),
            new Key(Keys.ControlKey,            "",             false),
            new Key(Keys.Menu,					"",             false),
            new Key(Keys.Pause,                 "",             true),
            new Key(Keys.Capital,               "Capslock",     true),
            new Key(Keys.KanaMode,              "",             false),
            new Key(Keys.JunjaMode,             "",             false),
            new Key(Keys.FinalMode,             "",             false),
            new Key(Keys.HanjaMode,             "",             false),
            new Key(Keys.Escape,                "",             false),
            new Key(Keys.IMEConvert,            "",             false),
            new Key(Keys.IMENonconvert,         "",				false),
            new Key(Keys.IMEAceept,             "",             false),
            new Key(Keys.IMEModeChange,			"",             false),
            new Key(Keys.Space,                 "",             false),
            new Key(Keys.PageUp,                "Page-Up",      true),
            new Key(Keys.Next,                  "Page-Down",    true),
            new Key(Keys.End,                   "",          	true),
            new Key(Keys.Home,                  "",         	true),
            new Key(Keys.Left,                  "",             true),
            new Key(Keys.Up,                    "",             true),
            new Key(Keys.Right,                 "",             true),
            new Key(Keys.Down,                  "",             true),
            new Key(Keys.Select,                "",				false),
            new Key(Keys.Print,                 "",             false),
            new Key(Keys.Execute,               "",             false),
            new Key(Keys.PrintScreen,           "",             true),
            new Key(Keys.Insert,                "",             false),
            new Key(Keys.Delete,                "",             true),
            new Key(Keys.Help,                  "",             false),
            new Key(Keys.D0,                    "0",            true),
            new Key(Keys.D1,                    "1",            true),
            new Key(Keys.D2,                    "2",            true),
            new Key(Keys.D3,                    "3",            true),
            new Key(Keys.D4,                    "4",            true),
            new Key(Keys.D5,                    "5",			true),
            new Key(Keys.D6,                    "6",            true),
            new Key(Keys.D7,                    "7",            true),
            new Key(Keys.D8,                    "8",            true),
            new Key(Keys.D9,                    "9",            true),
            new Key(Keys.A,                     "",             true),
            new Key(Keys.B,                     "",             true),
            new Key(Keys.C,						"",             true),
            new Key(Keys.D,                     "",             true),
            new Key(Keys.E,                     "",             true),
            new Key(Keys.F,                     "",             true),
            new Key(Keys.G,                     "",             true),
            new Key(Keys.H,                     "",             true),
            new Key(Keys.I,                     "",             true),
            new Key(Keys.J,                     "",             true),
            new Key(Keys.K,                     "",             true),
            new Key(Keys.L,                     "",             true),
            new Key(Keys.M,                     "",             true),
            new Key(Keys.N,                     "",             true),
            new Key(Keys.O,                     "",             true),
            new Key(Keys.P,                     "",             true),
            new Key(Keys.Q,                     "",             true),
            new Key(Keys.R,                     "",             true),
            new Key(Keys.S,                     "",             true),
            new Key(Keys.T,                     "",             true),
            new Key(Keys.U,                     "",             true),
            new Key(Keys.V,                     "",             true),
            new Key(Keys.W,                     "",             true),
            new Key(Keys.X,                     "",             true),
            new Key(Keys.Y,                     "",             true),
            new Key(Keys.Z,                     "",             true),
            new Key(Keys.LWin,                  "Win-(L)",   	true),
            new Key(Keys.RWin,                  "Win-(R)",  	true),
            new Key(Keys.Apps,                  "",             false),
            new Key(Keys.Sleep,                 "",             false),
            new Key(Keys.NumPad0,               "Numpad-0",     true),
            new Key(Keys.NumPad1,               "Numpad-1",     true),
            new Key(Keys.NumPad2,               "Numpad-2",     true),
            new Key(Keys.NumPad3,               "Numpad-3",     true),
            new Key(Keys.NumPad4,               "Numpad-4",     true),
            new Key(Keys.NumPad5,               "Numpad-5",     true),
            new Key(Keys.NumPad6,               "Numpad-6",     true),
            new Key(Keys.NumPad7,               "Numpad-7",     true),
            new Key(Keys.NumPad8,               "Numpad-8",     true),
            new Key(Keys.NumPad9,               "Numpad-9",     true),
            new Key(Keys.Multiply,              "Numpad-*",     true),
            new Key(Keys.Add,                   "Numpad-+",     true),
            new Key(Keys.Separator,             "",             false),
            new Key(Keys.Subtract,              "Numpad--",		true),
            new Key(Keys.Decimal,               "Numpad-.",     true),
            new Key(Keys.Divide,                "Numpad-/",     true),
            new Key(Keys.F1,                    "",             true),
            new Key(Keys.F2,                    "",             true),
            new Key(Keys.F3,                    "",             true),
            new Key(Keys.F4,                    "",             true),
            new Key(Keys.F5,                    "",             true),
            new Key(Keys.F6,                    "",             true),
            new Key(Keys.F7,                    "",             true),
            new Key(Keys.F8,                    "",             true),
            new Key(Keys.F9,                    "",             true),
            new Key(Keys.F10,                   "",             true),
            new Key(Keys.F11,                   "",             true),
            new Key(Keys.F12,                   "",             true),
            new Key(Keys.F13,                   "",             false),
            new Key(Keys.F14,                   "",             false),
            new Key(Keys.F15,                   "",             false),
            new Key(Keys.F16,                   "",             false),
            new Key(Keys.F17,                   "",             false),
            new Key(Keys.F18,                   "",             false),
            new Key(Keys.F19,                   "",             false),
            new Key(Keys.F20,                   "",             false),
            new Key(Keys.F21,					"",             false),
            new Key(Keys.F22,                   "",             false),
            new Key(Keys.F23,                   "",             false),
            new Key(Keys.F24,                   "",             false),
            new Key(Keys.NumLock,               "Num-Lock",     true),
            new Key(Keys.Scroll,                "Scroll-Lock",  true),
            new Key(Keys.LShiftKey,             "Shift-(L)",    true),
            new Key(Keys.RShiftKey,             "Shift-(R)",    true),
            new Key(Keys.LControlKey,           "Control-(L)",  true),
            new Key(Keys.RControlKey,           "Control-(R)",  true),
            new Key(Keys.LMenu,                 "Alt-(L)",      true),
            new Key(Keys.RMenu,                 "Alt-(R)",      true),
            new Key(Keys.BrowserBack,           "Browser-Back", false),
            new Key(Keys.BrowserForward,        "Browser-Fwd",  false),
            new Key(Keys.BrowserRefresh,        "",             false),
            new Key(Keys.BrowserStop,           "",             false),
            new Key(Keys.BrowserSearch,         "",             false),
            new Key(Keys.BrowserFavorites,      "",             false),
            new Key(Keys.BrowserHome,           "",             false),
            new Key(Keys.VolumeMute,            "Mute",         true),
            new Key(Keys.VolumeDown,            "Vol-Dn",       true),
            new Key(Keys.VolumeUp,              "Vol-Up",       true),
            new Key(Keys.MediaNextTrack,        "Media-Next",   true),
            new Key(Keys.MediaPreviousTrack,    "Media-Prev",   true),
            new Key(Keys.MediaStop,             "Media-Stop",   true),
            new Key(Keys.MediaPlayPause,        "Media-Play",   true),
            new Key(Keys.LaunchMail,            "",             false),
            new Key(Keys.SelectMedia,           "",             false),
            new Key(Keys.LaunchApplication1,    "",             false),
            new Key(Keys.LaunchApplication2,    "",             false),
            // Oem1: For the US standard keyboard, the ';:' key 
            new Key(Keys.Oem1,                  ";",            true),      
            // Oem2: For the US standard keyboard, the '/?' key 
            // Oem2: 191, same as OemQuestion
            // new Key(Keys.Oem2,                  "",             false),      
            new Key(Keys.Oemplus,               "+",            true),
            new Key(Keys.Oemcomma,              ",",            true),
            new Key(Keys.OemMinus,              "-",            true),
            new Key(Keys.OemPeriod,             ".",            true),
            new Key(Keys.OemQuestion,           "/",            true),
            new Key(Keys.Oemtilde,              "~",            true),
            new Key(Keys.OemOpenBrackets,       "[",            true),
            new Key(Keys.OemCloseBrackets,      "]",            true),
            // Oem5: For the US standard keyboard, the '\|' key
            new Key(Keys.Oem5,                  "\\",           true),
            // Oem6: For the US standard keyboard, the ']}' key
            // Oem6: 221, same as OemCloseBrackets
            // new Key(Keys.Oem6,               "",             false),
            // Oem7: For the US standard keyboard, the 'single-quote/double-quote' key
            new Key(Keys.Oem7,                  "'",            true),
            // Oem8: Used for miscellaneous characters; it can vary by keyboard.
            new Key(Keys.Oem8,                  "",             false),
            // OemBackSlash: Same as Oem3
            // OemBackSlash: 
            new Key(Keys.OemBackslash,          "",             false),
            new Key(Keys.ProcessKey,            "",             false),
            new Key(Keys.Packet,                "",             false),
            new Key(Keys.Attn,                  "",             false),
            new Key(Keys.Crsel,                 "",             false),
            new Key(Keys.Exsel,                 "",             false),
            new Key(Keys.EraseEof,              "",             false),
            new Key(Keys.Play,                  "",             false),
            new Key(Keys.Zoom,                  "",             false),
            new Key(Keys.NoName,                "",             false),
            new Key(Keys.Pa1,                   "",             false),
            new Key(Keys.OemClear,              "",             false),
            new Key(Keys.KeyCode,               "",             false),
            new Key(Keys.Shift,                 "",             false),
            new Key(Keys.Control,				"",             false),
            new Key(Keys.Alt,                   "",             false),
        };

        private static readonly List<Key> s_bindableKeys = new List<Key>();
        private static readonly Dictionary<int, Key> s_keys = new Dictionary<int, Key>();

        public static List<Key> BindableKeys 
        { 
            get
            {
                InitBindableKeys();
                return s_bindableKeys;
            }
        }

        public static Key GetKey(int virtualKeyCode)
        { 
            InitKeys();
            s_keys.TryGetValue(virtualKeyCode, out Key key);

            return key;
        }

        public static Key GetKey(Keys keys)
        {
            return GetKey((int) keys);
        }

        public static bool IsKeyDown(Keys key)
        {
            return Native.Keyboard.IsKeyDown((int) key);
        }

        internal static bool IsKeyToggled(Keys key)
        {
            return Native.Keyboard.IsKeyToggled((int) key);
        }

        private static void InitKeys()
        {
            lock (s_keys)
            {
                if (0 == s_keys.Count)
                {
                    foreach (Key key in KEYS)
                    {
                        s_keys.Add((int) key.Keys, key);
                    }
                }
            }
        }

        private static void InitBindableKeys()
        {
            lock (s_bindableKeys)
            {
                if (0 == s_bindableKeys.Count)
                {
                    foreach (Key key in KEYS)
                    {
                        if (key.Bindable)
                        {
                            s_bindableKeys.Add(key);
                        }
                    }
                }
            }
        }
    }
}
