using Glue.Actions;
using Glue.Event;
using Glue.Native;
using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using WindowsInput.Native;

namespace Glue.Forms
{
    public partial class ViewMain : Form
    {
        public bool LogInput { get => logInput; set { logInput = value; checkBoxRawKeyNames.Enabled = value; } }
        public bool RawKeyNames { get => rawKeyNames; set => rawKeyNames = value; }
        public bool NormalizeMouseCoords { get => normalizeMouseCoords; set => normalizeMouseCoords = value; }

        private string BaseCaptionText { get => this.baseCaptionText; set => this.baseCaptionText = value; }

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private delegate void LogControllerDelegate(ControllerEventArgs controllerEventArgs, string movement);
        private delegate void AppendTextDelegate(string text);

        private bool logInput = true;
        private bool rawKeyNames = false;
        private bool normalizeMouseCoords;
        private string baseCaptionText = "";
        private ViewButtons viewButtons = null;
        private ViewQueue viewQueue = null;
        private readonly ApplicationContext context;
        private readonly FormSettingsHandler formSettingsHandler;

        public ViewMain(ApplicationContext context)
        {
            this.context = context;

            InitializeComponent();

            // Attach for form settings persistence
            formSettingsHandler = new FormSettingsHandler(this);

            this.logInput = Properties.Settings.Default.LogInput;
            this.rawKeyNames = Properties.Settings.Default.RawKeyNames;
            this.normalizeMouseCoords = Properties.Settings.Default.NormalizeMouseCoords;

            this.checkBoxLogDisplay.DataBindings.Add("Checked", this, "logInput", true, DataSourceUpdateMode.OnPropertyChanged);
            this.checkBoxRawKeyNames.DataBindings.Add("Checked", this, "rawKeyNames", true, DataSourceUpdateMode.OnPropertyChanged);
            this.checkBoxNormalizeMouseCoords.DataBindings.Add("Checked", this, "normalizeMouseCoords", true, DataSourceUpdateMode.OnPropertyChanged);

            BaseCaptionText = this.Text;
            SetCaption(Tube.FileName);

            EventBus<EventKeyboard>.Instance.EventRecieved += EventKeyboard_Recieved;
            EventBus<EventMouse>.Instance.EventRecieved += EventMouse_Received;
            EventBus<EventMacro>.Instance.EventRecieved += EventMacro_Received;

            Tube.DirectInputManager.ControllerButtonEvent += OnControllerButton;
            Tube.DirectInputManager.ControllerHatEvent += OnControllerHat;

            checkBoxRawKeyNames.Enabled = logInput;
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
                this.viewButtons = ShowView(this.viewButtons, false);
                this.viewQueue = ShowView(this.viewQueue, false);

                base.OnFormClosing(e);
            }
        }

        private void SetCaption(string fileName)
        {
            this.Text = BaseCaptionText + " - " + fileName;
        }

        private void AppendText(string text)
        {
            if (this.InvokeRequired)
            { 
                AppendTextDelegate d = new AppendTextDelegate(AppendText);
                this.Invoke(d, new object[] {text});
            }
            else
            {
                if (!IsDisposed)
                { 
                    textBoxInputStream.AppendText(text);
                    WindowHandleUtils.HideCaret(this.textBoxInputStream.Handle);
                }
            }
        }

        protected override void OnShown(EventArgs e)
        {
            this.menuItemViewButtons.Checked = Properties.Settings.Default.ViewButtons;
            this.viewButtons = ShowView(this.viewButtons, Properties.Settings.Default.ViewButtons);

            this.menuItemViewQueue.Checked = Properties.Settings.Default.ViewQueue;
            this.viewQueue = ShowView(this.viewQueue, Properties.Settings.Default.ViewQueue);
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            textBoxInputStream.Clear();
            WindowHandleUtils.HideCaret(this.textBoxInputStream.Handle);
        }

        protected override void OnDeactivate(EventArgs e)
        {
            Properties.Settings.Default.LogInput = this.logInput;
            Properties.Settings.Default.RawKeyNames = this.RawKeyNames;
            Properties.Settings.Default.ViewButtons = this.menuItemViewButtons.Checked;
            Properties.Settings.Default.NormalizeMouseCoords = this.NormalizeMouseCoords;

            LOGGER.Info("Saving settings (Properties.Settings.Default.Save())");
            Properties.Settings.Default.Save();

            base.OnDeactivate(e);
        }

        private void MenuItemFileOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if(openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Tube.LoadFile(openFileDialog.FileName);
                    SetCaption(Tube.FileName);
                }
            }
        }

        private void MenuItemFileExit_Click(object sender, EventArgs e)
        {
            this.Close();
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
            this.menuItemViewButtons.Checked = Properties.Settings.Default.ViewButtons;
            this.viewButtons = ShowView(this.viewButtons, Properties.Settings.Default.ViewButtons);

            this.menuItemViewQueue.Checked = Properties.Settings.Default.ViewQueue;
            this.viewQueue = ShowView(this.viewQueue, Properties.Settings.Default.ViewQueue);

            WindowHandleUtils.HideCaret(this.textBoxInputStream.Handle);

            base.OnActivated(e);
        }

        private void MenuItemViewButtons_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ViewButtons = !Properties.Settings.Default.ViewButtons;

            this.viewButtons = ShowView(this.viewButtons, Properties.Settings.Default.ViewButtons);
            this.menuItemViewButtons.Checked = Properties.Settings.Default.ViewButtons;
        }

        private void MenuItemViewQueue_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ViewQueue = !Properties.Settings.Default.ViewQueue;

            this.viewQueue = ShowView(this.viewQueue, Properties.Settings.Default.ViewQueue);
            this.menuItemViewQueue.Checked = Properties.Settings.Default.ViewQueue;
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
        }

        private void MenuItemEditRemaps_Click(object sender, EventArgs e)
        {
        }

        internal void DisplayMouseMove(int xPos, int yPos)
        {
            int xOut = xPos;
            int yOut = yPos;

            if (this.NormalizeMouseCoords)
            {
                xOut = ActionMouse.NormalizeX(xPos);
                yOut = ActionMouse.NormalizeY(yPos);
            }

            this.toolStripMousePos.Text = String.Format("Mouse: ({0:n0}, {1:n0})", xOut, yOut);
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
            if (rawKeyNames)
            {
                message = String.Format("-click({0}, {1})", xOut, yOut);
            }
            else
            {
                message = String.Format("-click({0:n0} {1:n0})", xOut, yOut);
            }
            AppendText(" " + mouseButton.ToString() + message);

            this.toolStripMousePosLastClick.Text = String.Format("Last click: ({0:n0}, {1:n0})", xOut, yOut);
        }

        public void OnControllerButton(ControllerEventArgs eventArgs)
        {
            DisplayControllerEvent(eventArgs, ((ButtonValues) eventArgs.Update.Value).ToString());
        }

        private void OnControllerHat(ControllerEventArgs eventArgs)
        {
            DisplayControllerEvent(eventArgs, ((HatValues) eventArgs.Update.Value).ToString());
        }

        internal void DisplayControllerEvent(ControllerEventArgs eventArgs, string movement)
        {
            if (this.InvokeRequired)
            { 
                LogControllerDelegate d = new LogControllerDelegate(DisplayControllerEvent);
                this.Invoke(d, new object[] {eventArgs, movement});
            }
            else
            {
                AppendText(" " + eventArgs.Joystick.Information.InstanceName + " " + eventArgs.Update.Offset + "(" + movement + ")");
            }
        }

        private void EventMouse_Received(object sender, BusEventArgs<EventMouse> e)
        {
            EventMouse eventMouse = e.BusEvent;

            // Always update status bar when mouse moves
            DisplayMouseMove(eventMouse.X, eventMouse.Y);

            // Only display click locations though if specified
            if (LogInput)
            {

                if (eventMouse.MouseButton != MouseButtons.None && 
                    eventMouse.ButtonState == ButtonStates.Press)
                {
                    DisplayMouseClick(eventMouse.MouseButton, eventMouse.X, eventMouse.Y);
                }
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
                Tube.MainForm.AppendText(" [" + e.BusEvent.MacroName + "]");
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
    }
}
