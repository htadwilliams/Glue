using Glue.Native;
using Glue.PropertyIO;
using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;
using Action = Glue.Actions.Action;

namespace Glue.Forms
{
    public partial class ViewQueue : Form
    {
        private delegate void UpdateViewDelegate();
        private ReadOnlyCollection<Action> actions;

        protected readonly string labelHeadingFormat;

        public void OnQueueChange(ReadOnlyCollection<Action> actions)
        {
            this.actions = actions;
            UpdateView();
        }

        public ViewQueue()
        {
            InitializeComponent();
            labelHeadingFormat = labelHeading.Text;

            Tube.Scheduler.QueueChangeEvent += OnQueueChange;

            SetHeadingText(0);
            this.listViewActions.VirtualListSize = 0;
        }

        private void SetHeadingText(int countItems)
        {
            this.labelHeading.Text = String.Format(labelHeadingFormat, countItems);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
            base.OnFormClosing(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
        }

        protected void UpdateView()
        {
            // Cross-thread calling safety 
            // TODO move cross-thread calling delegate stuff into ActionQueueScheduler?
            if (this.InvokeRequired)
            { 
                UpdateViewDelegate d = new UpdateViewDelegate(UpdateView);
                this.Invoke(d, new object[] {});
            }
            else
            {
                this.SetHeadingText(actions.Count);
                this.listViewActions.VirtualListSize = actions.Count;
            }
        }

        private void ListViewActions_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            ListViewItem listViewItem = new ListViewItem();
            ListViewSubItem listViewItemTime = new ListViewSubItem();

            if (null != this.actions && this.actions.Count > e.ItemIndex)
            {
                Action action = actions[e.ItemIndex];

                listViewItem.Text = action.ToString();

                long scheduledTime = action.ScheduledTick - TimeProvider.GetTickCount();
                listViewItemTime.Text = FormatDuration.Format(scheduledTime);
            }

            listViewItem.SubItems.Add(listViewItemTime);
            e.Item = listViewItem;
        }
    }
}
