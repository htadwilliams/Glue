using Glue.Forms;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Glue
{
    class TrayApplicationContext<T> : ApplicationContext where T : Form, new()
    {
        public Container Container { get; set; }
        public NotifyIcon NotifyIcon { get; set; }

        internal void ShowForm()
        {
            if (null == MainForm || MainForm.IsDisposed)
            {
                MainForm = new T();
                 
            }
            MainForm.Show();
        }

        public TrayApplicationContext()
        {
            InitializeContext();
        }

        private void InitializeContext()
        {
            Container = new Container();
            NotifyIcon = new NotifyIcon(Container)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = Properties.Resources.glue,
                Text = "Glue",
                Visible = true
            };

            NotifyIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            NotifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            NotifyIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("&Show Window", null, Show_Click));
            NotifyIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("&About Glue", null, HelpAbout_Click));
            NotifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            NotifyIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("E&xit", null, Exit_Click));

            ShowForm();
        }

        private void Show_Click(object sender, EventArgs e)
        {
            ShowForm();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            ExitThread();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Container != null) 
            { 
                Container.Dispose(); 
            }
        }

        protected override void ExitThreadCore()
        {
            if (MainForm != null && !MainForm.IsDisposed) 
            { 
                MainForm.Close(); 
            }

            // should remove lingering tray icon
            NotifyIcon.Visible = false; 

            base.ExitThreadCore();
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowForm();
        }

        private void HelpAbout_Click(object sender, EventArgs e)
        {
            DialogHelpAbout helpAbout = new DialogHelpAbout();
            helpAbout.ShowDialog();
        }

        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            // TODO customize context menu strip here if needed
            e.Cancel = false;
        }
    }
}
