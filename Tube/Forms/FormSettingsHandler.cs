using System;
using System.Linq;
using System.Windows.Forms;

namespace Glue.Forms
{
    public class FormSettingsHandler
    {
        private readonly Form form;
        private FormSettings settings;

        public FormSettingsHandler(Form form)
        {
            this.form = form;

            form.Load += new System.EventHandler(OnFormLoad);
            form.FormClosing += new System.Windows.Forms.FormClosingEventHandler(OnFormClosing);
        }

        internal void OnFormLoad(object sender, EventArgs e)
        {
            this.settings = FormSettings.Get(form.Name);

            if (null != this.settings)
            {
                if (this.settings.IsMaximized)
                {
                    form.WindowState = FormWindowState.Maximized;
                }
                else if (Screen.AllScreens.Any(
                    screen => screen.WorkingArea.IntersectsWith(this.settings.Position)))
                {
                    form.DesktopBounds = this.settings.Position;
                    form.StartPosition = FormStartPosition.Manual;
                    form.WindowState = FormWindowState.Normal;
                }
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            FormSettings settings = new FormSettings(
                form.Name, 
                form.DesktopBounds, 
                form.WindowState == FormWindowState.Maximized);
            FormSettings.Save(form.Name, settings);
        }
    }
}
