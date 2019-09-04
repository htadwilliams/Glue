﻿using Glue.Actions;
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
        public bool NormalizeMouseCoords { get => normalizeMouseCoords; set => normalizeMouseCoords = value; }

        private string BaseCaptionText { get => this.baseCaptionText; set => this.baseCaptionText = value; }

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool logInput = true;
        private bool rawKeyNames = false;
        private bool normalizeMouseCoords;
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
            this.normalizeMouseCoords = Properties.Settings.Default.NormalizeMouseCoords;

            this.checkBoxLogDisplay.DataBindings.Add("Checked", this, "logInput", true, DataSourceUpdateMode.OnPropertyChanged);
            this.checkBoxRawKeyNames.DataBindings.Add("Checked", this, "rawKeyNames", true, DataSourceUpdateMode.OnPropertyChanged);
            this.checkBoxNormalizeMouseCoords.DataBindings.Add("Checked", this, "normalizeMouseCoords", true, DataSourceUpdateMode.OnPropertyChanged);

            BaseCaptionText = this.Text;
            SetCaption(Tube.FileName);

            checkBoxRawKeyNames.Enabled = logInput;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Really close application if either control key is held
            if (Keyboard.IsKeyDown(Keys.LControlKey) || Keyboard.IsKeyDown(Keys.RControlKey))
            {
                base.OnFormClosing(e);
            }
            // Don't actually close the form - app is still on system tray
            else
            {
                e.Cancel = true;

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
            Properties.Settings.Default.NormalizeMouseCoords = this.NormalizeMouseCoords;

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
            Properties.Settings.Default.ViewButtons = !Properties.Settings.Default.ViewButtons;

            this.viewButtons = ShowView(this.viewButtons, Properties.Settings.Default.ViewButtons);
            this.menuItemViewButtons.Checked = Properties.Settings.Default.ViewButtons;
        }

        private void MenuItemViewQueue_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ViewQueue = !Properties.Settings.Default.ViewQueue;

            this.viewQueue = ShowView(this.viewQueue, Properties.Settings.Default.ViewQueue);
            this.menuItemViewQueue.Checked = Properties.Settings.Default.ViewQueue;
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

        internal void LogMouseMove(int xPos, int yPos)
        {
            int xOut = xPos;
            int yOut = yPos;
            if (this.NormalizeMouseCoords)
            {
                xOut = ActionMouse.NormalizeX(xPos);
                yOut = ActionMouse.NormalizeY(yPos);
            }
            this.toolStripMousePos.Text = String.Format("Mouse: ({0:n0}, {1:n0})", xOut, yOut);
        }

        internal void LogMouseClick(int xPos, int yPos)
        {
            int xOut = xPos;
            int yOut = yPos;
            if (this.NormalizeMouseCoords)
            {
                xOut = ActionMouse.NormalizeX(xPos);
                yOut = ActionMouse.NormalizeY(yPos);
            }

            if (this.LogInput && this.RawKeyNames)
            {
                this.AppendText(String.Format("CLICK({0}, {1}) ", xOut, yOut));
            }
            this.toolStripMousePosLastClick.Text = String.Format("Last click: ({0:n0}, {1:n0})", xOut, yOut);
        }
    }
}
