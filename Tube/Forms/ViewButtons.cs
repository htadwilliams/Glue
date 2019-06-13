using Glue.Native;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WindowsInput.Native;

namespace Glue.Forms
{
    public partial class ViewButtons : Form
    {
        public ViewButtons()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            WindowHandleUtils.HideCaret(this.textBoxButtonStates.Handle);
            base.OnActivated(e);
        }

        internal void UpdateKeys(List<VirtualKeyCode> keys)
        {
            this.textBoxButtonStates.Clear();
            foreach(VirtualKeyCode keyCode in keys)
            {
                this.textBoxButtonStates.Text += keyCode.ToString();
                this.textBoxButtonStates.Text += "\r\n";
            }
        }
    }
}
