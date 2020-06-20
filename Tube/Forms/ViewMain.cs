using Glue.Actions;
using Glue.Events;
using Glue.Native;
using NerfDX.Events;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using WindowsInput.Native;

namespace Glue.Forms
{
    public partial class ViewMain : Form
    {
        // For cross-thread event handling
        private delegate void LogControllerDelegate(EventController eventController);
        private delegate void AppendTextDelegate(string text);

        // Views controlled by this form
        private ViewButtons viewButtons = null;
        private ViewQueue viewQueue = null;
        private ViewControllers viewControllers = null;

        private readonly string baseCaptionText = "";
        private readonly FormSettingsHandler formSettingsHandler;

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Properties are required for control DataBindings.Add()
        public bool LogInput { get; set; } = true;
        public bool NormalizeMouseCoords { get; set; } = false;
        public bool RawKeyNames { get; set; } = false;

        public ViewMain()
        {
            InitializeComponent();

            // Attach for form settings persistence
            formSettingsHandler = new FormSettingsHandler(this);

            LogInput = Properties.Settings.Default.LogInput;
            RawKeyNames = Properties.Settings.Default.RawKeyNames;
            NormalizeMouseCoords = Properties.Settings.Default.NormalizeMouseCoords;

            checkBoxLogInput.DataBindings.Add("Checked", this, "logInput", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxRawKeyNames.DataBindings.Add("Checked", this, "rawKeyNames", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxNormalizeMouseCoords.DataBindings.Add("Checked", this, "normalizeMouseCoords", true, DataSourceUpdateMode.OnPropertyChanged);

            // Cache contents of form caption Text for later output formatting 
            // via SetCaption() to display opened file name
            baseCaptionText = Text;
            SetCaption(Tube.FileName);
        }

        private void ViewMain_Load(object sender, EventArgs e)
        {
            EventBus<EventKeyboard>.Instance.EventRecieved += EventKeyboard_Recieved;
            EventBus<EventMouse>.Instance.EventRecieved += EventMouse_Received;
            EventBus<EventMacro>.Instance.EventRecieved += EventMacro_Received;
            EventBus<EventController>.Instance.EventRecieved += EventController_Received;
        }

        private void EventController_Received(object sender, BusEventArgs<EventController> e)
        {
            DisplayControllerEvent(e.BusEvent);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Really close application if either control key is held
            if (Keyboard.IsKeyDown(Keys.LControlKey) || Keyboard.IsKeyDown(Keys.RControlKey))
            {
                base.OnFormClosing(e);
            }
            // Don't actually close the form - app is still on system tray
            else
            {
                e.Cancel = true;

                Hide();
                viewButtons = ShowView(viewButtons, false);
                viewQueue = ShowView(viewQueue, false);
                viewControllers = ShowView(viewControllers, false);

                base.OnFormClosing(e);
            }
        }

        public void SetCaption(string fileName)
        {
            Text = baseCaptionText + " - " + fileName;
        }

        private void AppendText(string text)
        {
            if (!IsDisposed)
            { 
                if (InvokeRequired)
                { 
                    AppendTextDelegate d = new AppendTextDelegate(AppendText);
                    Invoke(d, new object[] {text});
                }
                else
                {
                    textBoxInputStream.AppendText(text);
                    WindowHandleUtils.HideCaret(textBoxInputStream.Handle);
                }
            }
        }

        protected override void OnShown(EventArgs e)
        {
            menuItemViewButtons.Checked = Properties.Settings.Default.ViewButtons;
            viewButtons = ShowView(viewButtons, Properties.Settings.Default.ViewButtons);

            menuItemViewQueue.Checked = Properties.Settings.Default.ViewQueue;
            viewQueue = ShowView(viewQueue, Properties.Settings.Default.ViewQueue);
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            textBoxInputStream.Clear();
            WindowHandleUtils.HideCaret(textBoxInputStream.Handle);
        }

        protected override void OnDeactivate(EventArgs e)
        {
            SaveSettings();
            base.OnDeactivate(e);
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.LogInput = checkBoxLogInput.Checked;
            Properties.Settings.Default.RawKeyNames = checkBoxRawKeyNames.Checked;
            Properties.Settings.Default.NormalizeMouseCoords = checkBoxNormalizeMouseCoords.Checked;

            Properties.Settings.Default.ViewButtons = menuItemViewButtons.Checked;
            Properties.Settings.Default.ViewQueue = menuItemViewQueue.Checked;

            LOGGER.Info("Saving settings (Properties.Settings.Default.Save())");
            Properties.Settings.Default.Save();
        }

        private void MenuItemFileOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Directory.GetCurrentDirectory(); ;
                if(openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Tube.LoadFile(openFileDialog.FileName);
                    SetCaption(Tube.FileName);
                }
            }
        }

        private void MenuItemFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MenuItemHelpAbout_Click(object sender, EventArgs e)
        {
            using (DialogHelpAbout helpAbout = new DialogHelpAbout())
            {
                helpAbout.ShowDialog();
            }
        }

        private T ShowView<T>(T view, bool showView = true) where T : Form, new()
        {
            T shownView = view;
            if (showView)
            {
                if (null == shownView || shownView.IsDisposed)
                {
                    shownView = new T();
                }
                if (!shownView.Visible)
                {
                    shownView.Show(this);
                }
            }
            else if (null != shownView && !shownView.IsDisposed)
            {
                shownView.Hide();
            }
            return shownView;
        }

        protected override void OnActivated(EventArgs e)
        {
            menuItemViewButtons.Checked = Properties.Settings.Default.ViewButtons;
            viewButtons = ShowView(viewButtons, Properties.Settings.Default.ViewButtons);

            menuItemViewQueue.Checked = Properties.Settings.Default.ViewQueue;
            viewQueue = ShowView(viewQueue, Properties.Settings.Default.ViewQueue);

            WindowHandleUtils.HideCaret(textBoxInputStream.Handle);

            base.OnActivated(e);
        }

        private void MenuItemViewButtons_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ViewButtons = !Properties.Settings.Default.ViewButtons;

            viewButtons = ShowView(viewButtons, Properties.Settings.Default.ViewButtons);
            menuItemViewButtons.Checked = Properties.Settings.Default.ViewButtons;
        }

        private void MenuItemViewQueue_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ViewQueue = !Properties.Settings.Default.ViewQueue;

            viewQueue = ShowView(viewQueue, Properties.Settings.Default.ViewQueue);
            menuItemViewQueue.Checked = Properties.Settings.Default.ViewQueue;
        }

        private void MenuItemEditMacros_Click(object sender, EventArgs e)
        {
            using (Form macros = new DialogEditMacros(new ReadOnlyDictionary<string, Macro>(Tube.Macros)))
            {
                macros.ShowDialog();
            }
        }

        private void MenuItemEditTriggers_Click(object sender, EventArgs e)
        {
            // TODO
        }

        private void MenuItemEditRemaps_Click(object sender, EventArgs e)
        {
            // TODO
        }

        internal void DisplayMouseMove(int xPos, int yPos)
        {
            int xOut = xPos;
            int yOut = yPos;

            if (NormalizeMouseCoords)
            {
                xOut = ActionMouse.NormalizeX(xPos);
                yOut = ActionMouse.NormalizeY(yPos);
            }

            toolStripMousePos.Text = String.Format("Mouse: ({0:n0}, {1:n0})", xOut, yOut);
        }

        internal void DisplayMouseClick(MouseButtons mouseButton, int xPos, int yPos)
        {
            int xOut = xPos;
            int yOut = yPos;

            if (NormalizeMouseCoords)
            {
                xOut = ActionMouse.NormalizeX(xPos);
                yOut = ActionMouse.NormalizeY(yPos);
            }

            string message;
            if (RawKeyNames)
            {
                message = String.Format("-click({0}, {1})", xOut, yOut);
            }
            else
            {
                message = String.Format("-click({0:n0} {1:n0})", xOut, yOut);
            }
            AppendText(" " + mouseButton.ToString() + message);

            toolStripMousePosLastClick.Text = String.Format("Last click: ({0:n0}, {1:n0})", xOut, yOut);
        }

        internal void DisplayControllerEvent(EventController controllerEvent)
        {
            if (!IsDisposed)
            {
                if (InvokeRequired)
                { 
                    LogControllerDelegate d = new LogControllerDelegate(DisplayControllerEvent);
                    Invoke(d, new object[] {controllerEvent});
                }
                else
                {
                    if (controllerEvent.Type == EventController.EventType.Button)
                    {
                        AppendText(" " + controllerEvent.Joystick.Information.InstanceName + 
                            " (Button " + controllerEvent.Button + " " + controllerEvent.ButtonValue + ")");
                    }
                    else if (controllerEvent.Type == EventController.EventType.POV)
                    {
                        AppendText(" " + controllerEvent.Joystick.Information.InstanceName + 
                            " (POV " + controllerEvent.POVState + ")");
                    }
                }
            }
        }

        private void EventMouse_Received(object sender, BusEventArgs<EventMouse> e)
        {
            EventMouse eventMouse = e.BusEvent;

            // Always update status bar when mouse moves
            DisplayMouseMove(eventMouse.X, eventMouse.Y);

            if (eventMouse.MouseButton != MouseButtons.None && 
                eventMouse.ButtonState == ButtonStates.Press)
            {
                DisplayMouseClick(eventMouse.MouseButton, eventMouse.X, eventMouse.Y);
            }
        }

        private void EventKeyboard_Recieved(object sender, BusEventArgs<EventKeyboard> e)
        {
            if (LogInput)
            {
                EventKeyboard eventKeyboard = e.BusEvent;

                if (eventKeyboard.ButtonState == ButtonStates.Press)
                {
                    DisplayKeyDown(eventKeyboard.VirtualKeyCode);
                }
                else
                {
                    DisplayKeyUp(eventKeyboard.VirtualKeyCode);
                }
            }
        }

        private void EventMacro_Received(object sender, BusEventArgs<EventMacro> e)
        {
            if (LogInput)
            {
                AppendText(" [" + e.BusEvent.MacroName + "]");
            }
        }

        public void DisplayKeyUp(int vkCode)
        {
            if (RawKeyNames)
            {
                AppendText(" -" + (VirtualKeyCode) vkCode);
            }
        }

        public void DisplayKeyDown(int vkCode)
        {
            string output;
            if (RawKeyNames)
            {
                output = " +" + (VirtualKeyCode) vkCode;
            }
            else
            {
                output = " " + FormatKeyString(vkCode);
            }

            AppendText(output);
        }

        // TODO FormatKeyString should be in its own wrapper around Keyboard Key
        private static string FormatKeyString(int vkCode)
        {
            string output = "";
            
            Key key = Keyboard.GetKey(vkCode);
            if (null == key)
            {
                return output;
            }

            output = key.Display;
            if (output == "")
            {
                output = key.ToString();
            }

            // Only printed characters are a single character long 
            if (output.Length == 1)
            {
                // Could be simplified but this is super clear to read
                if (Keyboard.IsKeyToggled(Keys.CapsLock))
                {
                    if (Keyboard.IsKeyDown(Keys.LShiftKey) || Keyboard.IsKeyDown(Keys.RShiftKey))
                    {
                        output = output.ToLower();
                    }
                }
                else
                {
                    if (!Keyboard.IsKeyDown(Keys.LShiftKey) && !Keyboard.IsKeyDown(Keys.RShiftKey))
                    {
                        output = output.ToLower();
                    }
                }
            }

            return output;
        }

        private void CheckBoxLogDisplay_CheckedChanged(object sender, EventArgs e)
        {
            // Save when checked to publish this setting
            // This prevents keyboard INFO logging from keyboard handler and elsewhere
            SaveSettings();
        }

        private void MenuItemViewGameControllers_Click(object sender, EventArgs e)
        {
            viewControllers = ShowView(viewControllers, true);
        }
    }
}
