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

            SetHeadingText(0);
            this.listViewActions.VirtualListSize = 0;
        }

        private void SetHeadingText(int countItems)
        {
            this.labelHeading.Text = String.Format(labelHeadingFormat, countItems);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Still want to re-open when parent is activated unless user closes 
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Properties.Settings.Default.ViewQueue = false;
            }

            Tube.Scheduler.QueueChangeEvent -= OnQueueChange;
            // e.Cancel = true;
            // Hide();
            base.OnFormClosing(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            Tube.Scheduler.QueueChangeEvent += OnQueueChange;

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
            ListViewItem listViewItemScheduled = new ListViewItem();
            ListViewSubItem listViewItemAction = new ListViewSubItem();

            if (null != this.actions && this.actions.Count > e.ItemIndex)
            {
                Action action = actions[e.ItemIndex];

                long scheduledTime = action.ScheduledTick - TimeProvider.GetTickCount();
                listViewItemScheduled.Text = FormatDuration.Format(scheduledTime);

                listViewItemAction.Text = action.ToString();
            }

            listViewItemScheduled.SubItems.Add(listViewItemAction);
            e.Item = listViewItemScheduled;
        }
    }
}
