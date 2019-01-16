using System;
using System.Windows.Forms;

namespace Glue
{
    public partial class Main : Form
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool logInput = true;

        public Main()
        {
            InitializeComponent();
            this.IDC_LOGDISPLAY.DataBindings.Add("Checked", this, "logInput");
        }

        public bool LogInput { get => logInput; set => logInput = value; }

        internal void AppendText(String text)
        {
             IDE_INPUTSTREAM.AppendText(text);
        }

        private void IDB_CLEAR_Click(object sender, EventArgs e)
        {
            IDE_INPUTSTREAM.Clear();
        }
    }
}
