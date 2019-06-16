using Glue.Native;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WindowsInput.Native;

namespace Glue.Forms
{
    public partial class Main : Form
    {
        public bool LogInput { get => logInput; set { logInput = value; checkBoxRawKeyNames.Enabled = value; } }
        public bool RawKeyNames { get => rawKeyNames; set => rawKeyNames = value; }

        private string BaseCaptionText { get => this.baseCaptionText; set => this.baseCaptionText = value; }

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool logInput = true;
        private bool rawKeyNames = false;
        private string baseCaptionText = "";
        private ViewButtons viewButtons = null;

        public Main()
        {
            InitializeComponent();

            this.logInput = Properties.Settings.Default.LogInput;
            this.rawKeyNames = Properties.Settings.Default.RawKeyNames;

            this.checkBoxLogDisplay.DataBindings.Add("Checked", this, "logInput", true, DataSourceUpdateMode.OnPropertyChanged);
            this.checkBoxRawKeyNames.DataBindings.Add("Checked", this, "rawKeyNames", true, DataSourceUpdateMode.OnPropertyChanged);

            BaseCaptionText = this.Text;
            SetCaption(Tube.FileName);

            checkBoxRawKeyNames.Enabled = logInput;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Don't really close - hide instead so that state isn't lost 
            e.Cancel = true;
            Hide();

            base.OnFormClosing(e);
        }

        private void OnViewButtonsClosing(object sender, EventArgs e)
        {
            this.menuItemViewButtons.Checked = false;
        }

        private void SetCaption(string fileName)
        {
            this.Text = BaseCaptionText + " - " + fileName;
        }

        internal void AppendText(string text)
        {
            if (!IsDisposed)
            { 
                textBoxInputStream.AppendText(text);
                WindowHandleUtils.HideCaret(this.textBoxInputStream.Handle);
            }
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            textBoxInputStream.Clear();
            WindowHandleUtils.HideCaret(this.textBoxInputStream.Handle);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Properties.Settings.Default.LogInput = this.logInput;
            Properties.Settings.Default.RawKeyNames = this.RawKeyNames;

            LOGGER.Info("Saving settings (Properties.Settings.Default.Save())");
            Properties.Settings.Default.Save();

            base.OnFormClosed(e);
        }

        private void MenuItemFileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Tube.LoadFile(openFileDialog.FileName);
                SetCaption(Tube.FileName);
            }
        }

        private void MenuItemFileExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MenuItemHelpAbout_Click(object sender, EventArgs e)
        {
            HelpAbout helpAbout = new HelpAbout();
            helpAbout.ShowDialog();
        }

        public void ToggleViewButtons(bool showView = true)
        {
            if (showView)
            {
                if (null == viewButtons || viewButtons.IsDisposed)
                {
                    this.viewButtons = new ViewButtons();
                    this.viewButtons.FormClosing += new FormClosingEventHandler(this.OnViewButtonsClosing);
                }

                this.viewButtons.Show(this);
            }
            else if (null != viewButtons && !viewButtons.IsDisposed)
            {
                this.viewButtons.Close();
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            WindowHandleUtils.HideCaret(this.textBoxInputStream.Handle);
            base.OnActivated(e);
        }

        private void MenuItemViewButtons_Click(object sender, EventArgs e)
        {
            ToggleViewButtons(this.menuItemViewButtons.Checked);
        }

        internal void UpdateKeys(List<VirtualKeyCode> keys)
        {
            if (this.viewButtons != null && this.viewButtons.Visible)
            { 
                this.viewButtons.UpdateKeys(keys);
            }
        }
    }
}
