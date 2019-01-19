using System;
using System.Windows.Forms;

namespace Glue
{
    public partial class Main : Form
    {

        public bool LogInput { get => logInput; set { logInput = value; IDC_RAWKEYNAMES.Enabled = value; } }
        public bool RawKeyNames { get => rawKeyNames; set => rawKeyNames = value; }

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool logInput = true;
        private bool rawKeyNames = false;

        public Main()
        {
            InitializeComponent();
            this.IDC_LOGDISPLAY.DataBindings.Add("Checked", this, "logInput", true, DataSourceUpdateMode.OnPropertyChanged);
            this.IDC_RAWKEYNAMES.DataBindings.Add("Checked", this, "rawKeyNames", true, DataSourceUpdateMode.OnPropertyChanged);
            IDC_RAWKEYNAMES.Enabled = logInput;
        }

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
