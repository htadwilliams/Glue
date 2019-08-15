using Glue.PropertyIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;
using Action = Glue.Actions.Action;

namespace Glue.Forms
{
    public partial class DialogEditMacros : Form
    {
        private List<Macro> macros;
        private Macro macroCurrent = null;
        private Action actionCurrent = null;
        private PropertyBag propertiesCurrent = null;

        public DialogEditMacros(ReadOnlyDictionary<string, Macro> macros)
        {
            // Copying is cleaner OOP, and also allows user to cancel after making mistakes
            // TODO Deeper copy of macros - all objects referenced should be copied including actions etc.
            this.macros = new List<Macro>(macros.Values);
            if (this.macros.Count > 0)
            {
                this.macroCurrent = this.macros[0];
            }
            else
            {
                this.macroCurrent = null;
            }

            InitializeComponent();
        }

        private void ListViewMacros_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            ListViewItem listViewItem = new ListViewItem();
            Macro macro = this.macros[e.ItemIndex];

            listViewItem.Text = macro.Name;

            e.Item = listViewItem;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            this.listViewMacros.VirtualListSize = this.macros.Count;

            // TODO make last macro selection persistent 
            this.listViewMacros.Items[0].Selected = true;
            this.listViewMacros.Select();

            this.listViewMacros.HideSelection = false;
            this.listViewProperties.HideSelection = false;

            if (null != this.macroCurrent)
            {
                this.listViewActions.VirtualListSize = this.macroCurrent.Actions.Count;
            }
        }

        private void ListViewActions_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            ListViewItem listViewItem = new ListViewItem();
            ListViewSubItem listViewItemDelay = new ListViewSubItem();

            if (null != macroCurrent && macroCurrent.Actions.Count >= e.ItemIndex)
            {
                Actions.Action action = macroCurrent.Actions[e.ItemIndex];
                listViewItem.Text = action.ToString();
                listViewItemDelay.Text = FormatDuration.Format(action.DelayMS);
            }
            else
            {
                listViewItem.Text = "null";
                listViewItemDelay.Text = "null";
            }

            listViewItem.SubItems.Add(listViewItemDelay);
            e.Item = listViewItem;
        }

        private void ListViewMacros_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                if (null != this.macros && this.macros.Count >= e.ItemIndex)
                {
                    this.macroCurrent = this.macros[e.ItemIndex];
                    this.listViewActions.VirtualListSize = this.macroCurrent.Actions.Count;
                }
                else
                {
                    this.listViewActions.VirtualListSize = 0;
                }
                this.listViewProperties.HideSelection = false;
                this.listViewActions.Invalidate();
            }
        }

        private void ListViewEvents_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                if (null != this.macroCurrent && this.macroCurrent.Actions.Count >= e.ItemIndex)
                {
                    this.actionCurrent = this.macroCurrent.Actions[e.ItemIndex];
                    this.listViewProperties.VirtualListSize = this.propertiesCurrent.Count;
                }
            }
        }

        private void ListViewProperties_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            ListViewItem listViewItemName = new ListViewItem();
            ListViewSubItem listViewItemValue = new ListViewSubItem();



            listViewItemName.SubItems.Add(listViewItemValue);
            e.Item = listViewItemName;
        }
    }
}
