using Glue.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using WindowsInput.Native;

namespace Glue.Forms
{
    public partial class ViewMain : Form
    {
        public bool LogInput { get => logInput; set { logInput = value; checkBoxRawKeyNames.Enabled = value; } }
        public bool RawKeyNames { get => rawKeyNames; set => rawKeyNames = value; }

        private string BaseCaptionText { get => this.baseCaptionText; set => this.baseCaptionText = value; }

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool logInput = true;
        private bool rawKeyNames = false;
        private string baseCaptionText = "";
        private ViewButtons viewButtons = null;
        private ViewQueue viewQueue = null;
        private readonly ApplicationContext context;

        public ViewMain(ApplicationContext context)
        {
            this.context = context;

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
            // Close application if either control key is held, otherwise hide 
            // window and remain on system notification tray
            //

            if (Keyboard.IsKeyDown(Keys.LControlKey) || Keyboard.IsKeyDown(Keys.RControlKey))
            {
                context.ExitThread();
            }
            else
            {
                // Don't actually close the form - we're just hiding
                e.Cancel = true;

                // Hide self and floating views
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

        internal void AppendText(string text)
        {
            if (!IsDisposed)
            { 
                textBoxInputStream.AppendText(text);
                WindowHandleUtils.HideCaret(this.textBoxInputStream.Handle);
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

            LOGGER.Info("Saving settings (Properties.Settings.Default.Save())");
            Properties.Settings.Default.Save();

            base.OnDeactivate(e);
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
            new DialogHelpAbout().ShowDialog();
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
            this.viewButtons = ShowView(this.viewButtons, this.menuItemViewButtons.Checked);
            Properties.Settings.Default.ViewButtons = this.menuItemViewButtons.Checked;
        }

        private void MenuItemViewQueue_Click(object sender, EventArgs e)
        {
            this.viewQueue = ShowView(this.viewQueue, this.menuItemViewQueue.Checked);
            Properties.Settings.Default.ViewQueue = this.menuItemViewQueue.Checked;
        }

        internal void UpdateKeys(List<VirtualKeyCode> keys)
        {
            if (this.viewButtons != null && this.viewButtons.Visible)
            { 
                this.viewButtons.UpdateKeys(keys);
            }
        }

        private void MenuItemEditMacros_Click(object sender, EventArgs e)
        {
            Form macros = new DialogEditMacros(new ReadOnlyDictionary<string, Macro>(Tube.Macros));
            macros.ShowDialog();
        }

        private void MenuItemEditTriggers_Click(object sender, EventArgs e)
        {
        }

        private void MenuItemEditRemaps_Click(object sender, EventArgs e)
        {
        }
    }
}
