using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WindowsInput.Native;

namespace Glue
{
    class Keyboard
    {
        private static readonly Key[] KEYS =
        {
            //                                  Used only if interceptor driver is installed
            //      Virtual key code            driver key code                         Display name    Bindable
            new Key(Keys.None,                  0,                                      "",             false),
            new Key(Keys.LButton,               0,                                      "",             false),
            new Key(Keys.RButton,               0,                                      "",             false),
            new Key(Keys.Cancel,                0,                                      "",             false),
            new Key(Keys.MButton,               0,                                      "",             false),
            new Key(Keys.XButton1,              0,                                      "",             false),
            new Key(Keys.XButton2,              0,                                      "",             false),
            new Key(Keys.Back,                  Interceptor.Keys.Backspace,             "Backspace",    true),
            new Key(Keys.Tab,                   Interceptor.Keys.Tab,                   "",             true),
            new Key(Keys.LineFeed,              0,                                      "",             false),
            new Key(Keys.Clear,                 0,                                      "",             false),
            new Key(Keys.Return,                Interceptor.Keys.Enter,                 "Enter",        true),
            new Key(Keys.ShiftKey,              0,                                      "",             false),
            new Key(Keys.ControlKey,            0,                                      "",             false),
            new Key(Keys.Menu,                  0,                                      "",             false),
            new Key(Keys.Pause,                 0,                                      "",             true),
            new Key(Keys.Capital,               Interceptor.Keys.CapsLock,              "Capslock",     true),
            new Key(Keys.KanaMode,              0,                                      "",             false),
            new Key(Keys.JunjaMode,             0,                                      "",             false),
            new Key(Keys.FinalMode,             0,                                      "",             false),
            new Key(Keys.HanjaMode,             0,                                      "",             false),
            new Key(Keys.Escape,                0,                                      "",             false),
            new Key(Keys.IMEConvert,            0,                                      "",             false),
            new Key(Keys.IMENonconvert,         0,                                      "",             false),
            new Key(Keys.IMEAceept,             0,                                      "",             false),
            new Key(Keys.IMEModeChange,         0,                                      "",             false),
            new Key(Keys.Space,                 Interceptor.Keys.Space,                 "Space",        false),
            new Key(Keys.PageUp,                Interceptor.Keys.PageUp,                "",             true),
            new Key(Keys.Next,                  Interceptor.Keys.PageDown,              "PageDown",     true),
            new Key(Keys.End,                   Interceptor.Keys.End,                   "",             true),
            new Key(Keys.Home,                  Interceptor.Keys.Home,                  "",             true),
            new Key(Keys.Left,                  Interceptor.Keys.Left,                  "",             true),
            new Key(Keys.Up,                    Interceptor.Keys.Up,                    "",             true),
            new Key(Keys.Right,                 Interceptor.Keys.Right,                 "",             true),
            new Key(Keys.Down,                  Interceptor.Keys.Down,                  "",             true),
            new Key(Keys.Select,                0,                                      "",             false),
            new Key(Keys.Print,                 0,                                      "",             false),
            new Key(Keys.Execute,               0,                                      "",             false),
            new Key(Keys.PrintScreen,           Interceptor.Keys.PrintScreen,           "",             true),
            new Key(Keys.Insert,                0,                                      "",             false),
            new Key(Keys.Delete,                Interceptor.Keys.Delete,                "",             true),
            new Key(Keys.Help,                  0,                                      "",             false),
            new Key(Keys.D0,                    Interceptor.Keys.Zero,                  "0",            true),
            new Key(Keys.D1,                    Interceptor.Keys.One,                   "1",            true),
            new Key(Keys.D2,                    Interceptor.Keys.Two,                   "2",            true),
            new Key(Keys.D3,                    Interceptor.Keys.Three,                 "3",            true),
            new Key(Keys.D4,                    Interceptor.Keys.Four,                  "4",            true),
            new Key(Keys.D5,                    Interceptor.Keys.Five,                  "5",            true),
            new Key(Keys.D6,                    Interceptor.Keys.Six,                   "6",            true),
            new Key(Keys.D7,                    Interceptor.Keys.Seven,                 "7",            true),
            new Key(Keys.D8,                    Interceptor.Keys.Eight,                 "8",            true),
            new Key(Keys.D9,                    Interceptor.Keys.Nine,                  "9",            true),
            new Key(Keys.A,                     Interceptor.Keys.A,                     "",             true),
            new Key(Keys.B,                     Interceptor.Keys.B,                     "",             true),
            new Key(Keys.C,                     Interceptor.Keys.C,                     "",             true),
            new Key(Keys.D,                     Interceptor.Keys.D,                     "",             true),
            new Key(Keys.E,                     Interceptor.Keys.E,                     "",             true),
            new Key(Keys.F,                     Interceptor.Keys.F,                     "",             true),
            new Key(Keys.G,                     Interceptor.Keys.G,                     "",             true),
            new Key(Keys.H,                     Interceptor.Keys.H,                     "",             true),
            new Key(Keys.I,                     Interceptor.Keys.I,                     "",             true),
            new Key(Keys.J,                     Interceptor.Keys.J,                     "",             true),
            new Key(Keys.K,                     Interceptor.Keys.K,                     "",             true),
            new Key(Keys.L,                     Interceptor.Keys.L,                     "",             true),
            new Key(Keys.M,                     Interceptor.Keys.M,                     "",             true),
            new Key(Keys.N,                     Interceptor.Keys.N,                     "",             true),
            new Key(Keys.O,                     Interceptor.Keys.O,                     "",             true),
            new Key(Keys.P,                     Interceptor.Keys.P,                     "",             true),
            new Key(Keys.Q,                     Interceptor.Keys.Q,                     "",             true),
            new Key(Keys.R,                     Interceptor.Keys.R,                     "",             true),
            new Key(Keys.S,                     Interceptor.Keys.S,                     "",             true),
            new Key(Keys.T,                     Interceptor.Keys.T,                     "",             true),
            new Key(Keys.U,                     Interceptor.Keys.U,                     "",             true),
            new Key(Keys.V,                     Interceptor.Keys.V,                     "",             true),
            new Key(Keys.W,                     Interceptor.Keys.W,                     "",             true),
            new Key(Keys.X,                     Interceptor.Keys.X,                     "",             true),
            new Key(Keys.Y,                     Interceptor.Keys.Y,                     "",             true),
            new Key(Keys.Z,                     Interceptor.Keys.Z,                     "",             true),
            new Key(Keys.LWin,                  Interceptor.Keys.WindowsKey,            "",   	        true),
            new Key(Keys.RWin,                  Interceptor.Keys.WindowsKey,            "",  	        true),
            new Key(Keys.Apps,                  0,                                      "",             false),
            new Key(Keys.Sleep,                 0,                                      "",             false),
            new Key(Keys.NumPad0,               Interceptor.Keys.Numpad0,               "",             true),
            new Key(Keys.NumPad1,               Interceptor.Keys.Numpad1,               "",             true),
            new Key(Keys.NumPad2,               Interceptor.Keys.Numpad2,               "",             true),
            new Key(Keys.NumPad3,               Interceptor.Keys.Numpad3,               "",             true),
            new Key(Keys.NumPad4,               Interceptor.Keys.Numpad4,               "",             true),
            new Key(Keys.NumPad5,               Interceptor.Keys.Numpad5,               "",             true),
            new Key(Keys.NumPad6,               Interceptor.Keys.Numpad6,               "",             true),
            new Key(Keys.NumPad7,               Interceptor.Keys.Numpad7,               "",             true),
            new Key(Keys.NumPad8,               Interceptor.Keys.Numpad8,               "",             true),
            new Key(Keys.NumPad9,               Interceptor.Keys.Numpad9,               "",             true),
            new Key(Keys.Multiply,              Interceptor.Keys.NumpadAsterisk,        "NumPad*",      true),
            new Key(Keys.Add,                   Interceptor.Keys.NumpadPlus,            "NumPad+",      true),
            new Key(Keys.Separator,             0,                                      "",             false),
            new Key(Keys.Subtract,              Interceptor.Keys.NumpadMinus,           "NumPad-",		true),
            new Key(Keys.Decimal,               Interceptor.Keys.NumpadDelete,          "NumPad.",      true),
            new Key(Keys.Divide,                Interceptor.Keys.NumpadDivide,          "NumPad/",      true),
            new Key(Keys.F1,                    Interceptor.Keys.F1,                    "",             true),
            new Key(Keys.F2,                    Interceptor.Keys.F2,                    "",             true),
            new Key(Keys.F3,                    Interceptor.Keys.F3,                    "",             true),
            new Key(Keys.F4,                    Interceptor.Keys.F4,                    "",             true),
            new Key(Keys.F5,                    Interceptor.Keys.F5,                    "",             true),
            new Key(Keys.F6,                    Interceptor.Keys.F6,                    "",             true),
            new Key(Keys.F7,                    Interceptor.Keys.F7,                    "",             true),
            new Key(Keys.F8,                    Interceptor.Keys.F8,                    "",             true),
            new Key(Keys.F9,                    Interceptor.Keys.F9,                    "",             true),
            new Key(Keys.F10,                   Interceptor.Keys.F10,                   "",             true),
            new Key(Keys.F11,                   Interceptor.Keys.F11,                   "",             true),
            new Key(Keys.F12,                   Interceptor.Keys.F12,                   "",             true),
            new Key(Keys.F13,                   0,                                      "",             false),
            new Key(Keys.F14,                   0,                                      "",             false),
            new Key(Keys.F15,                   0,                                      "",             false),
            new Key(Keys.F16,                   0,                                      "",             false),
            new Key(Keys.F17,                   0,                                      "",             false),
            new Key(Keys.F18,                   0,                                      "",             false),
            new Key(Keys.F19,                   0,                                      "",             false),
            new Key(Keys.F20,                   0,                                      "",             false),
            new Key(Keys.F21,					0,                                      "",             false),
            new Key(Keys.F22,                   0,                                      "",             false),
            new Key(Keys.F23,                   0,                                      "",             false),
            new Key(Keys.F24,                   0,                                      "",             false),
            new Key(Keys.NumLock,               0,                                      "",             true),
            new Key(Keys.Scroll,                Interceptor.Keys.ScrollLock,            "ScrollLock",   true),
            new Key(Keys.LShiftKey,             Interceptor.Keys.LeftShift,             "LShift",       true),
            new Key(Keys.RShiftKey,             Interceptor.Keys.RightShift,            "RShift",       true),
            new Key(Keys.LControlKey,           Interceptor.Keys.Control,               "LControl",     true),
            new Key(Keys.RControlKey,           Interceptor.Keys.Control,               "RControl",     true),
            new Key(Keys.LMenu,                 Interceptor.Keys.RightAlt,              "LAlt",         true),
            new Key(Keys.RMenu,                 Interceptor.Keys.RightAlt,              "RAlt",         true),
            new Key(Keys.BrowserBack,           0,                                      "",             false),
            new Key(Keys.BrowserForward,        0,                                      "",             false),
            new Key(Keys.BrowserRefresh,        0,                                      "",             false),
            new Key(Keys.BrowserStop,           0,                                      "",             false),
            new Key(Keys.BrowserSearch,         0,                                      "",             false),
            new Key(Keys.BrowserFavorites,      0,                                      "",             false),
            new Key(Keys.BrowserHome,           0,                                      "",             false),
            new Key(Keys.VolumeMute,            0,                                      "",             true),
            new Key(Keys.VolumeDown,            0,                                      "",             true),
            new Key(Keys.VolumeUp,              0,                                      "",             true),
            new Key(Keys.MediaNextTrack,        0,                                      "",             true),
            new Key(Keys.MediaPreviousTrack,    0,                                      "",             true),
            new Key(Keys.MediaStop,             0,                                      "",             true),
            new Key(Keys.MediaPlayPause,        0,                                      "",             true),
            new Key(Keys.LaunchMail,            0,                                      "",             true),
            new Key(Keys.SelectMedia,           0,                                      "",             true),
            new Key(Keys.LaunchApplication1,    0,                                      "",             true),
            new Key(Keys.LaunchApplication2,    0,                                      "",             true),
            // Oem1: For the US standard keyboard, the ';:' key 
            new Key(Keys.Oem1,                   Interceptor.Keys.SemicolonColon,       ";",            true),      
            // Oem2: For the US standard keyboard, the '/?' key 
            // Oem2: 191, same as OemQuestion         
            // new Key(Keys.Oem2,               0,                                      "",             false),      
            new Key(Keys.Oemplus,               Interceptor.Keys.PlusEquals,            "+",            true),
            new Key(Keys.Oemcomma,              Interceptor.Keys.CommaLeftArrow,        ",",            true),
            new Key(Keys.OemMinus,              Interceptor.Keys.DashUnderscore,        "-",            true),
            new Key(Keys.OemPeriod,             Interceptor.Keys.PeriodRightArrow,      ".",            true),
            new Key(Keys.OemQuestion,           Interceptor.Keys.ForwardSlashQuestionMark, "/",         true),
            new Key(Keys.Oemtilde,              Interceptor.Keys.Tilde,                 "~",            true),
            new Key(Keys.OemOpenBrackets,       Interceptor.Keys.OpenBracketBrace,      "[",            true),
            new Key(Keys.OemCloseBrackets,      Interceptor.Keys.CloseBracketBrace,     "]",            true),
            // Oem5: For the US standard keyboard, the '\|' key
            new Key(Keys.Oem5,                  Interceptor.Keys.BackslashPipe,         "\\",           true),
            // Oem6: For the US standard keyboard, the ']}' key
            // Oem6: 221, same as OemCloseBrackets
            // new Key(Keys.Oem6,               0,                                      "",             false),
            // Oem7: For the US standard keyboar, the 'single-quote/double-quote' key
            new Key(Keys.Oem7,                  Interceptor.Keys.SingleDoubleQuote,     "'",            true),
            // Oem8: Used for miscellaneous characters; it can vary by keyboard.
            new Key(Keys.Oem8,                  0,                                      "",             false),
            // OemBackSlash: Same as Oem3       0,          
            // OemBackSlash:                    0,          
            new Key(Keys.OemBackslash,          0,                                      "",             false),
            new Key(Keys.ProcessKey,            0,                                      "",             false),
            new Key(Keys.Packet,                0,                                      "",             false),
            new Key(Keys.Attn,                  0,                                      "",             false),
            new Key(Keys.Crsel,                 0,                                      "",             false),
            new Key(Keys.Exsel,                 0,                                      "",             false),
            new Key(Keys.EraseEof,              0,                                      "",             false),
            new Key(Keys.Play,                  0,                                      "",             false),
            new Key(Keys.Zoom,                  0,                                      "",             false),
            new Key(Keys.NoName,                0,                                      "",             false),
            new Key(Keys.Pa1,                   0,                                      "",             false),
            new Key(Keys.OemClear,              0,                                      "",             false),
            new Key(Keys.KeyCode,               0,                                      "",             false),
            new Key(Keys.Shift,                 0,                                      "",             false),
            new Key(Keys.Control,				0,                                      "",             false),
            new Key(Keys.Alt,                   0,                                      "",             false),
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

        // All of the following objects are locked via s_locker 
        private static readonly object s_locker = new object();
        private static readonly List<Key> s_bindableKeys = new List<Key>();
        private static readonly List<Key> s_filterDriverKeys = new List<Key>();
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

        public static List<Key> FilterDriverKeys
        {
            get
            {
                Init();
                return s_filterDriverKeys;
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
                        if (key.InterceptorKey != 0)
                        {
                            s_filterDriverKeys.Add(key);
                        }
                    }
                }
            }
        }
    }
}
