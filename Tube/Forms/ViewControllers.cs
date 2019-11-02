using Glue.Events;
using NerfDX;
using NerfDX.DirectInput;
using NerfDX.Events;
using SharpDX.DirectInput;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;

namespace Glue.Forms
{
    public partial class ViewControllers : Form
    {
        private readonly FormSettingsHandler formSettingsHandler;
        private ReadOnlyCollection<ConnectedDeviceInfo> connectedDeviceInfos;
        private delegate void UpdateViewDelegate();

        public ViewControllers()
        {
            InitializeComponent();

            formSettingsHandler = new FormSettingsHandler(this);
            EventBus<EventControllersChanged>.Instance.EventRecieved += OnControllerPlugEvent;
            
            this.connectedDeviceInfos = Tube.DirectInputManager.GetConnectedDeviceInfos();
            UpdateListView();
        }

        private void OnControllerPlugEvent(object sender, BusEventArgs<EventControllersChanged> e)
        {
            connectedDeviceInfos = e.BusEvent.ConnectedDeviceInfos;
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
                    if (null != connectedDeviceInfos && this.listViewControllers.VirtualListSize != connectedDeviceInfos.Count)
                    {
                        this.listViewControllers.VirtualListSize = connectedDeviceInfos.Count;
                        this.listViewControllers.Invalidate();
                    }
                }
            }
        }

        private void ListViewControllers_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            ConnectedDeviceInfo connectedDeviceInfo = connectedDeviceInfos[e.ItemIndex];

            ListViewItem listViewItem = new ListViewItem();
            listViewItem.Text = connectedDeviceInfo.Information.InstanceName;

            ListViewSubItem listViewSubItem = new ListViewSubItem();
            listViewSubItem.Text = connectedDeviceInfo.Information.Type.ToString();
            listViewItem.SubItems.Add(listViewSubItem);

            listViewSubItem = new ListViewSubItem();
            listViewSubItem.Text = connectedDeviceInfo.Information.InstanceGuid.ToString();
            listViewItem.SubItems.Add(listViewSubItem);

            e.Item = listViewItem;
        }
    }
}
