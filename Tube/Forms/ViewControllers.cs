using Glue.Events;
using SharpDX.DirectInput;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;

namespace Glue.Forms
{
    public partial class ViewControllers : Form
    {
        private readonly FormSettingsHandler formSettingsHandler;
        private ReadOnlyCollection<Joystick> joysticks;
        private delegate void UpdateViewDelegate();

        public ViewControllers()
        {
            InitializeComponent();

            formSettingsHandler = new FormSettingsHandler(this);
            EventBus<EventControllersChanged>.Instance.EventRecieved += OnControllerPlugEvent;
            
            this.joysticks = Tube.DirectInputManager.GetConnectedJoysticks();
            UpdateListView();
        }

        private void OnControllerPlugEvent(object sender, BusEventArgs<EventControllersChanged> e)
        {
            joysticks = e.BusEvent.Joysticks;
            UpdateListView();
        }

        private void UpdateListView()
        {
            if (!this.IsDisposed)
            {
                // Cross-thread calling safety 
                if (this.InvokeRequired)
                { 
                    UpdateViewDelegate d = new UpdateViewDelegate(UpdateListView);
                    this.Invoke(d, new object[] {});
                }
                else
                {
                    if (null != joysticks && this.listViewControllers.VirtualListSize != joysticks.Count)
                    {
                        this.listViewControllers.VirtualListSize = joysticks.Count;
                        this.listViewControllers.Invalidate();
                    }
                }
            }
        }

        private void ListViewActions_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            Joystick joystick = joysticks[e.ItemIndex];

            ListViewItem listViewItem = new ListViewItem();
            listViewItem.Text = joystick.Information.InstanceName;

            ListViewSubItem listViewSubItem = new ListViewSubItem();
            listViewSubItem.Text = joystick.Information.Type.ToString();
            listViewItem.SubItems.Add(listViewSubItem);

            listViewSubItem = new ListViewSubItem();
            listViewSubItem.Text = joystick.Information.InstanceGuid.ToString();
            listViewItem.SubItems.Add(listViewSubItem);

            e.Item = listViewItem;
        }
    }
}
