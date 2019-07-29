﻿using Glue.Native;
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
            if (!Keyboard.IsKeyDown(Keys.LControlKey) && !Keyboard.IsKeyDown(Keys.RControlKey))
            {
                e.Cancel = true;

                Hide();
                ShowViewButtons(false);
                base.OnFormClosing(e);
            }
            
            else
            {
                context.ExitThread();
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
            ShowViewButtons(Properties.Settings.Default.ViewButtons);
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
            DialogHelpAbout helpAbout = new DialogHelpAbout();
            helpAbout.ShowDialog();
        }

        public void ShowViewButtons(bool showView = true)
        {
            if (showView)
            {
                if (null == viewButtons || viewButtons.IsDisposed)
                {
                    // Register for close event only - not Hide()
                    this.viewButtons = new ViewButtons();
                }

                this.menuItemViewButtons.Checked = true;

                if (!this.viewButtons.Visible)
                { 
                    this.viewButtons.Show(this);
                }
            }
            else if (null != viewButtons && !viewButtons.IsDisposed)
            {
                this.viewButtons.Hide();
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            ShowViewButtons(Properties.Settings.Default.ViewButtons);
            WindowHandleUtils.HideCaret(this.textBoxInputStream.Handle);

            base.OnActivated(e);
        }

        private void MenuItemViewButtons_Click(object sender, EventArgs e)
        {
            ShowViewButtons(this.menuItemViewButtons.Checked);
            Properties.Settings.Default.ViewButtons = this.menuItemViewButtons.Checked;
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