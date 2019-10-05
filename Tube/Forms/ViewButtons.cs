using Glue.Event;
using Glue.Native;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Glue.Forms
{
    public partial class ViewButtons : Form
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string labelHeadingFormat;
        private readonly FormSettingsHandler formSettingsHandler;
        private readonly HashSet<int> keysPressed = new HashSet<int>();

        public ViewButtons()
        {
            InitializeComponent();

            // Attach for form settings persistence
            formSettingsHandler = new FormSettingsHandler(this);
            
            labelHeadingFormat = labelHeading.Text;
            SetHeadingText(0);

            EventBus<EventKeyboard>.Instance.EventRecieved += OnEventKeyboard;
        }

        private void OnEventKeyboard(object sender, BusEventArgs<EventKeyboard> e)
        {
            EventKeyboard eventKeyboard = e.BusEvent;

            if (eventKeyboard.ButtonState == ButtonStates.Press)
            {
                keysPressed.Add(eventKeyboard.VirtualKeyCode);
            }
            else
            {
                keysPressed.Remove(eventKeyboard.VirtualKeyCode);
            }

            UpdateKeys();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Still want to re-open when parent is activated unless user closes 
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Properties.Settings.Default.ViewButtons = false;
            }

            base.OnFormClosing(e);
        }   

        protected override void OnActivated(EventArgs e)
        {
            WindowHandleUtils.HideCaret(this.textBoxButtonStates.Handle);
            base.OnActivated(e);
        }

        protected void SetHeadingText(int countKeysPressed)
        {
            this.labelHeading.Text = String.Format(labelHeadingFormat, countKeysPressed);
        }

        internal void UpdateKeys()
        {
            if (!IsDisposed)
            {
                SetHeadingText(keysPressed.Count);
                this.textBoxButtonStates.Clear();
            
                foreach(int keyCode in keysPressed)
                {
                    Key key = Keyboard.GetKey(keyCode);
                    this.textBoxButtonStates.Text += key.ToString();;
                    this.textBoxButtonStates.Text += "\r\n";
                }

                WindowHandleUtils.HideCaret(this.textBoxButtonStates.Handle);
            }
        }
    }
}
