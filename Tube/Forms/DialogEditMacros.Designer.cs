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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("ListViewGroup1", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("ListViewGroup2", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "Test",
            "Sub0",
            "Sub1",
            "Sub2"}, -1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("");
            this.listViewEvents = new System.Windows.Forms.ListView();
            this.MacroName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewMacros = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderEvents = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTimeDelay = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.ButtonOk = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewEvents
            // 
            this.listViewEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewEvents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.MacroName,
            this.columnHeader1,
            this.columnHeader2});
            this.listViewEvents.FullRowSelect = true;
            this.listViewEvents.GridLines = true;
            listViewGroup1.Header = "ListViewGroup1";
            listViewGroup1.Name = "listViewGroup1";
            listViewGroup2.Header = "ListViewGroup2";
            listViewGroup2.Name = "listViewGroup2";
            this.listViewEvents.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
            listViewItem2.IndentCount = 2;
            this.listViewEvents.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4});
            this.listViewEvents.Location = new System.Drawing.Point(3, 3);
            this.listViewEvents.Name = "listViewEvents";
            this.listViewEvents.Size = new System.Drawing.Size(654, 280);
            this.listViewEvents.TabIndex = 0;
            this.listViewEvents.UseCompatibleStateImageBehavior = false;
            this.listViewEvents.View = System.Windows.Forms.View.Details;
            // 
            // MacroName
            // 
            this.MacroName.Text = "Name";
            this.MacroName.Width = 188;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 156;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Width = 140;
            // 
            // listViewMacros
            // 
            this.listViewMacros.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewMacros.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderEvents,
            this.columnHeaderTimeDelay});
            this.listViewMacros.FullRowSelect = true;
            this.listViewMacros.GridLines = true;
            this.listViewMacros.Location = new System.Drawing.Point(3, 0);
            this.listViewMacros.Name = "listViewMacros";
            this.listViewMacros.Size = new System.Drawing.Size(654, 556);
            this.listViewMacros.TabIndex = 1;
            this.listViewMacros.UseCompatibleStateImageBehavior = false;
            this.listViewMacros.View = System.Windows.Forms.View.Details;
            this.listViewMacros.VirtualMode = true;
            this.listViewMacros.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.ListView_RetrieveVirtualItem);
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 166;
            // 
            // columnHeaderEvents
            // 
            this.columnHeaderEvents.Text = "Events";
            this.columnHeaderEvents.Width = 132;
            // 
            // columnHeaderTimeDelay
            // 
            this.columnHeaderTimeDelay.Text = "Delay";
            this.columnHeaderTimeDelay.Width = 202;
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer.Location = new System.Drawing.Point(12, 12);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.listViewMacros);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.panel1);
            this.splitContainer.Panel2.Controls.Add(this.listViewEvents);
            this.splitContainer.Size = new System.Drawing.Size(660, 914);
            this.splitContainer.SplitterDistance = 559;
            this.splitContainer.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.ButtonOk);
            this.panel1.Location = new System.Drawing.Point(3, 289);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(657, 64);
            this.panel1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(170, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(161, 61);
            this.button1.TabIndex = 1;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // ButtonOk
            // 
            this.ButtonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ButtonOk.Location = new System.Drawing.Point(3, 3);
            this.ButtonOk.Name = "ButtonOk";
            this.ButtonOk.Size = new System.Drawing.Size(161, 61);
            this.ButtonOk.TabIndex = 0;
            this.ButtonOk.Text = "Ok";
            this.ButtonOk.UseVisualStyleBackColor = true;
            // 
            // DialogEditMacros
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 938);
            this.Controls.Add(this.splitContainer);
            this.Name = "DialogEditMacros";
            this.Text = "Glue - Edit Macros";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewEvents;
        private System.Windows.Forms.ColumnHeader MacroName;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ListView listViewMacros;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderEvents;
        private System.Windows.Forms.ColumnHeader columnHeaderTimeDelay;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button ButtonOk;
    }
}