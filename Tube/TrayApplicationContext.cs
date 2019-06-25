using Glue.Forms;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Glue
{
    class TrayApplicationContext : ApplicationContext
    {
        public Container Container { get => this.container; set => this.container = value; }
        public NotifyIcon NotifyIcon { get => this.notifyIcon; set => this.notifyIcon = value; }

        private Container container;
        private NotifyIcon notifyIcon;

        internal void ShowForm()
        {
            if (null == MainForm || MainForm.IsDisposed)
            {
                MainForm = new Main(this);
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
            NotifyIcon = new NotifyIcon(container)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = Properties.Resources.glue,
                Text = "Glue",
                Visible = true
            };

            NotifyIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            NotifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("&Show Window", null, Show_Click));
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("&About Glue", null, HelpAbout_Click));
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("E&xit", null, Exit_Click));

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
            if (disposing && container != null) 
            { 
                container.Dispose(); 
            }
        }

        protected override void ExitThreadCore()
        {
            if (MainForm != null && !MainForm.IsDisposed) 
            { 
                MainForm.Close(); 
            }

            // should remove lingering tray icon
            notifyIcon.Visible = false; 

            base.ExitThreadCore();
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowForm();
        }

        private void HelpAbout_Click(object sender, EventArgs e)
        {
            HelpAbout helpAbout = new HelpAbout();
            helpAbout.ShowDialog();
        }

        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            // TODO customize context menu strip here if needed
            e.Cancel = false;
        }
    }
}
