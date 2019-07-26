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

        public ViewButtons()
        {

            InitializeComponent();

            labelHeadingFormat = labelHeading.Text;

            SetHeadingText(0);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;

            Hide();

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
    }
}
