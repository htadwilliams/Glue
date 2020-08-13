using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Glue
{
    class Keyboard
    {
        private static readonly Key[] KEYS = 
        {
            //      Virtual key code            Display name    Bindable
            new Key(Keys.None,					"",				false),
            new Key(Keys.LButton,				"",				false),
            new Key(Keys.RButton,               "",             false),
            new Key(Keys.Cancel,				"",  		    false),
            new Key(Keys.MButton,               "",             false),
            new Key(Keys.XButton1,              "",             false),
            new Key(Keys.XButton2,              "",             false),
            new Key(Keys.Back,					"Backspace",    true),
            new Key(Keys.Tab,                   "",             true),
            new Key(Keys.LineFeed,              "",             false),
            new Key(Keys.Clear,                 "",             false),
            new Key(Keys.Return,                "Enter",		true),
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
            new Key(Keys.PageUp,                "",             true),
            new Key(Keys.Next,                  "PageDown",     true),
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
            new Key(Keys.NumPad0,               "",             true),
            new Key(Keys.NumPad1,               "",             true),
            new Key(Keys.NumPad2,               "",             true),
            new Key(Keys.NumPad3,               "",             true),
            new Key(Keys.NumPad4,               "",             true),
            new Key(Keys.NumPad5,               "",             true),
            new Key(Keys.NumPad6,               "",             true),
            new Key(Keys.NumPad7,               "",             true),
            new Key(Keys.NumPad8,               "",             true),
            new Key(Keys.NumPad9,               "",             true),
            new Key(Keys.Multiply,              "NumPad*",      true),
            new Key(Keys.Add,                   "NumPad+",      true),
            new Key(Keys.Separator,             "",             false),
            new Key(Keys.Subtract,              "NumPad-",		true),
            new Key(Keys.Decimal,               "NumPad.",      true),
            new Key(Keys.Divide,                "NumPad/",     true),
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
            new Key(Keys.NumLock,               "",             true),
            new Key(Keys.Scroll,                "ScrollLock",   true),
            new Key(Keys.LShiftKey,             "LShift",       true),
            new Key(Keys.RShiftKey,             "RShift",       true),
            new Key(Keys.LControlKey,           "LControl",     true),
            new Key(Keys.RControlKey,           "RControl",     true),
            new Key(Keys.LMenu,                 "LAlt",         true),
            new Key(Keys.RMenu,                 "RAlt",         true),
            new Key(Keys.BrowserBack,           "",             false),
            new Key(Keys.BrowserForward,        "",             false),
            new Key(Keys.BrowserRefresh,        "",             false),
            new Key(Keys.BrowserStop,           "",             false),
            new Key(Keys.BrowserSearch,         "",             false),
            new Key(Keys.BrowserFavorites,      "",             false),
            new Key(Keys.BrowserHome,           "",             false),
            new Key(Keys.VolumeMute,            "",             true),
            new Key(Keys.VolumeDown,            "",             true),
            new Key(Keys.VolumeUp,              "",             true),
            new Key(Keys.MediaNextTrack,        "",             true),
            new Key(Keys.MediaPreviousTrack,    "",             true),
            new Key(Keys.MediaStop,             "",             true),
            new Key(Keys.MediaPlayPause,        "",             true),
            new Key(Keys.LaunchMail,            "",             true),
            new Key(Keys.SelectMedia,           "",             true),
            new Key(Keys.LaunchApplication1,    "",             true),
            new Key(Keys.LaunchApplication2,    "",             true),
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

        internal static string GetKeyName(int virtualKeyCode)
        {
            Key key = GetKey(virtualKeyCode);

            if (key.Display.Length == 0)
            {
                return key.ToString();
            }

            return key.Display;
        }

        private static readonly object s_locker = new object();
        private static readonly List<Key> s_bindableKeys = new List<Key>();
        private static readonly Dictionary<int, Key> s_keyCodeMap = new Dictionary<int, Key>();
        private static readonly Dictionary<string, Key> s_keyNameMap = new Dictionary<string, Key>();

        public static List<Key> BindableKeys 
        { 
            get
            {
                Init();
                return s_bindableKeys;
            }
        }

        public static Key GetKey(int virtualKeyCode)
        { 
            Init();
            s_keyCodeMap.TryGetValue(virtualKeyCode, out Key key);

            return key;
        }

        public static Key GetKey(string keyName)
        {
            Init();
            s_keyNameMap.TryGetValue(keyName, out Key key);

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

        private static void Init()
        {
            lock (s_locker)
            {
                if (0 == s_keyCodeMap.Count)
                {
                    foreach (Key key in KEYS)
                    {
                        s_keyCodeMap.Add((int) key.Keys, key);

                        if (key.Display.Length > 0)
                        {
                            s_keyNameMap.Add(key.Display, key);
                        }
                        // add with default name
                        else
                        {
                            s_keyNameMap.Add(key.Keys.ToString(), key);
                        }

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
