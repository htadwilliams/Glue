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
        private string baseCaptionText = "";

        public Main()
        {
            InitializeComponent();

            this.logInput = Properties.Settings.Default.LogInput;
            this.rawKeyNames = Properties.Settings.Default.RawKeyNames;

            this.IDC_LOGDISPLAY.DataBindings.Add("Checked", this, "logInput", true, DataSourceUpdateMode.OnPropertyChanged);
            this.IDC_RAWKEYNAMES.DataBindings.Add("Checked", this, "rawKeyNames", true, DataSourceUpdateMode.OnPropertyChanged);

            baseCaptionText = this.Text;
            SetCaption(GlueTube.FileName);

            IDC_RAWKEYNAMES.Enabled = logInput;
        }

        private void SetCaption(string fileName)
        {
            this.Text = baseCaptionText + " - " + fileName;
        }

        internal void AppendText(string text)
        {
             IDE_INPUTSTREAM.AppendText(text);
        }

        private void IDB_CLEAR_Click(object sender, EventArgs e)
        {
            IDE_INPUTSTREAM.Clear();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Properties.Settings.Default.LogInput = this.logInput;
            Properties.Settings.Default.RawKeyNames = this.RawKeyNames;

            LOGGER.Info("Saving settings (Properties.Settings.Default.Save())");
            Properties.Settings.Default.Save();

            base.OnFormClosed(e);
        }

        private void FileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                GlueTube.LoadFile(openFileDialog.FileName);
                SetCaption(GlueTube.FileName);
            }
        }

        private void FileExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void HelpAbout_Click(object sender, EventArgs e)
        {
            HelpAbout helpAbout = new HelpAbout();
            helpAbout.ShowDialog();
        }
    }
}
