using Glue.Native;
using System;
using System.Windows.Forms;

namespace Glue.Forms
{
    public partial class ViewButtons : Form
    {
        public ViewButtons()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            WindowHandleUtils.HideCaret(this.textBoxButtonStates.Handle);
            base.OnShown(e);
        }
    }
}
