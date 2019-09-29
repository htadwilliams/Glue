using Glue.Native;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WindowsInput.Native;

namespace Glue.Forms
{
    public partial class ViewButtons : Form
    {
        protected readonly string labelHeadingFormat;
        private FormSettingsHandler formSettingsHandler;

        public ViewButtons()
        {
            InitializeComponent();

            // Attach for form settings persistence
            formSettingsHandler = new FormSettingsHandler(this);
            
            labelHeadingFormat = labelHeading.Text;
            SetHeadingText(0);
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

        internal void UpdateKeys(List<VirtualKeyCode> keys)
        {
            SetHeadingText(keys.Count);
            this.textBoxButtonStates.Clear();
            
            foreach(VirtualKeyCode keyCode in keys)
            {
                this.textBoxButtonStates.Text += keyCode.ToString();
                this.textBoxButtonStates.Text += "\r\n";
            }

            WindowHandleUtils.HideCaret(this.textBoxButtonStates.Handle);
        }

        private void ViewButtons_Load(object sender, EventArgs e)
        {
            formSettingsHandler.OnFormLoad(sender, e);
        }
    }
}
