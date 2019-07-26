using Glue.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;

namespace Glue.Forms
{
    public partial class DialogEditMacros : Form
    {
        private List<Macro> macros;

        public DialogEditMacros(ReadOnlyDictionary<string, Macro> macros)
        {
            // Copying is cleaner OOP, and also allows user to cancel after making mistakes
            // TODO Deeper copy of macros - all objects referenced should be copied including actions etc.
            this.macros = new List<Macro>(macros.Values);

            InitializeComponent();
        }

        private void ListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            ListViewItem listViewItem = new ListViewItem();
            Macro macro = this.macros[e.ItemIndex];

            listViewItem.Text = macro.Name;

            listViewItem.SubItems.Add(new ListViewSubItem().Text = macro.Actions.Count.ToString());
            listViewItem.SubItems.Add(new ListViewSubItem().Text = FormatDuration.StringFromMillis(macro.DelayTimeMS));

            e.Item = listViewItem;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            listViewMacros.VirtualListSize = this.macros.Count;
        }
    }
}
