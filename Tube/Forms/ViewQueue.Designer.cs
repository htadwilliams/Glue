namespace Glue.Forms
{
    partial class ViewQueue
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewQueue));
            this.labelHeading = new System.Windows.Forms.Label();
            this.listViewActions = new System.Windows.Forms.ListView();
            this.columnHeaderAction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTimeScheduled = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // labelHeading
            // 
            this.labelHeading.AutoSize = true;
            this.labelHeading.Location = new System.Drawing.Point(13, 14);
            this.labelHeading.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(203, 25);
            this.labelHeading.TabIndex = 5;
            this.labelHeading.Text = "Queued Actions: {0}";
            // 
            // listViewActions
            // 
            this.listViewActions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewActions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderTimeScheduled,
            this.columnHeaderAction});
            this.listViewActions.FullRowSelect = true;
            this.listViewActions.GridLines = true;
            this.listViewActions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewActions.Location = new System.Drawing.Point(12, 65);
            this.listViewActions.MultiSelect = false;
            this.listViewActions.Name = "listViewActions";
            this.listViewActions.ShowGroups = false;
            this.listViewActions.Size = new System.Drawing.Size(1214, 991);
            this.listViewActions.TabIndex = 6;
            this.listViewActions.UseCompatibleStateImageBehavior = false;
            this.listViewActions.View = System.Windows.Forms.View.Details;
            this.listViewActions.VirtualMode = true;
            this.listViewActions.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.ListViewActions_RetrieveVirtualItem);
            // 
            // columnHeaderAction
            // 
            this.columnHeaderAction.Text = "Action";
            this.columnHeaderAction.Width = 400;
            // 
            // columnHeaderTimeScheduled
            // 
            this.columnHeaderTimeScheduled.Text = "Time Scheduled";
            this.columnHeaderTimeScheduled.Width = 206;
            // 
            // ViewQueue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1238, 1068);
            this.Controls.Add(this.listViewActions);
            this.Controls.Add(this.labelHeading);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewQueue";
            this.ShowInTaskbar = false;
            this.Text = "Queue";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.ListView listViewActions;
        private System.Windows.Forms.ColumnHeader columnHeaderTimeScheduled;
        private System.Windows.Forms.ColumnHeader columnHeaderAction;
    }
}