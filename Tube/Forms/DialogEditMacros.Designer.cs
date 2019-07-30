namespace Glue.Forms
{
    partial class DialogEditMacros
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listViewEvents = new System.Windows.Forms.ListView();
            this.PropertyName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PropertyValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.textBoxNew = new System.Windows.Forms.TextBox();
            this.buttonNew = new System.Windows.Forms.Button();
            this.listViewMacros = new System.Windows.Forms.ListView();
            this.columnHeaderMacroName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewActions = new System.Windows.Forms.ListView();
            this.columnHeaderAction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderDelay = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewEvents
            // 
            this.listViewEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewEvents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.PropertyName,
            this.PropertyValue});
            this.listViewEvents.FullRowSelect = true;
            this.listViewEvents.GridLines = true;
            this.listViewEvents.Location = new System.Drawing.Point(14, 534);
            this.listViewEvents.Name = "listViewEvents";
            this.listViewEvents.Size = new System.Drawing.Size(1490, 272);
            this.listViewEvents.TabIndex = 4;
            this.listViewEvents.UseCompatibleStateImageBehavior = false;
            this.listViewEvents.View = System.Windows.Forms.View.Details;
            this.listViewEvents.VirtualMode = true;
            // 
            // PropertyName
            // 
            this.PropertyName.Text = "Name";
            this.PropertyName.Width = 188;
            // 
            // PropertyValue
            // 
            this.PropertyValue.Text = "Value";
            this.PropertyValue.Width = 1048;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Controls.Add(this.buttonOk);
            this.panel1.Location = new System.Drawing.Point(14, 809);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1490, 64);
            this.panel1.TabIndex = 5;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(170, 3);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(161, 61);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(3, 3);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(161, 61);
            this.buttonOk.TabIndex = 0;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // textBoxNew
            // 
            this.textBoxNew.Location = new System.Drawing.Point(58, 12);
            this.textBoxNew.Name = "textBoxNew";
            this.textBoxNew.Size = new System.Drawing.Size(100, 31);
            this.textBoxNew.TabIndex = 1;
            // 
            // buttonNew
            // 
            this.buttonNew.Location = new System.Drawing.Point(14, 12);
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new System.Drawing.Size(38, 37);
            this.buttonNew.TabIndex = 0;
            this.buttonNew.Text = "+";
            this.buttonNew.UseVisualStyleBackColor = true;
            // 
            // listViewMacros
            // 
            this.listViewMacros.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderMacroName});
            this.listViewMacros.FullRowSelect = true;
            this.listViewMacros.GridLines = true;
            this.listViewMacros.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewMacros.LabelEdit = true;
            this.listViewMacros.Location = new System.Drawing.Point(12, 55);
            this.listViewMacros.MultiSelect = false;
            this.listViewMacros.Name = "listViewMacros";
            this.listViewMacros.ShowGroups = false;
            this.listViewMacros.Size = new System.Drawing.Size(407, 473);
            this.listViewMacros.TabIndex = 2;
            this.listViewMacros.UseCompatibleStateImageBehavior = false;
            this.listViewMacros.View = System.Windows.Forms.View.Details;
            this.listViewMacros.VirtualMode = true;
            this.listViewMacros.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.ListViewMacros_ItemSelectionChanged);
            this.listViewMacros.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.ListViewMacros_RetrieveVirtualItem);
            // 
            // columnHeaderMacroName
            // 
            this.columnHeaderMacroName.Text = "Name";
            this.columnHeaderMacroName.Width = 400;
            // 
            // listViewActions
            // 
            this.listViewActions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewActions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderAction,
            this.columnHeaderDelay});
            this.listViewActions.FullRowSelect = true;
            this.listViewActions.GridLines = true;
            this.listViewActions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewActions.Location = new System.Drawing.Point(425, 55);
            this.listViewActions.MultiSelect = false;
            this.listViewActions.Name = "listViewActions";
            this.listViewActions.ShowGroups = false;
            this.listViewActions.Size = new System.Drawing.Size(1079, 473);
            this.listViewActions.TabIndex = 3;
            this.listViewActions.UseCompatibleStateImageBehavior = false;
            this.listViewActions.View = System.Windows.Forms.View.Details;
            this.listViewActions.VirtualMode = true;
            this.listViewActions.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.ListViewActions_RetrieveVirtualItem);
            // 
            // columnHeaderAction
            // 
            this.columnHeaderAction.Text = "Action";
            this.columnHeaderAction.Width = 456;
            // 
            // columnHeaderDelay
            // 
            this.columnHeaderDelay.Text = "Delay";
            this.columnHeaderDelay.Width = 196;
            // 
            // DialogEditMacros
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1516, 885);
            this.Controls.Add(this.listViewActions);
            this.Controls.Add(this.listViewMacros);
            this.Controls.Add(this.buttonNew);
            this.Controls.Add(this.textBoxNew);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.listViewEvents);
            this.Name = "DialogEditMacros";
            this.Text = "Glue - Edit Macros";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewEvents;
        private System.Windows.Forms.ColumnHeader PropertyName;
        private System.Windows.Forms.ColumnHeader PropertyValue;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.TextBox textBoxNew;
        private System.Windows.Forms.Button buttonNew;
        private System.Windows.Forms.ListView listViewMacros;
        private System.Windows.Forms.ListView listViewActions;
        private System.Windows.Forms.ColumnHeader columnHeaderMacroName;
        private System.Windows.Forms.ColumnHeader columnHeaderAction;
        private System.Windows.Forms.ColumnHeader columnHeaderDelay;
    }
}