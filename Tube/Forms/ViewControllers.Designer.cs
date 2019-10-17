namespace Glue.Forms
{
    partial class ViewControllers
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
            this.listViewControllers = new System.Windows.Forms.ListView();
            this.columnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnGUID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listViewControllers
            // 
            this.listViewControllers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewControllers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnType,
            this.columnGUID});
            this.listViewControllers.FullRowSelect = true;
            this.listViewControllers.GridLines = true;
            this.listViewControllers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewControllers.Location = new System.Drawing.Point(12, 12);
            this.listViewControllers.MultiSelect = false;
            this.listViewControllers.Name = "listViewControllers";
            this.listViewControllers.ShowGroups = false;
            this.listViewControllers.Size = new System.Drawing.Size(942, 1078);
            this.listViewControllers.TabIndex = 7;
            this.listViewControllers.UseCompatibleStateImageBehavior = false;
            this.listViewControllers.View = System.Windows.Forms.View.Details;
            this.listViewControllers.VirtualMode = true;
            this.listViewControllers.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.ListViewActions_RetrieveVirtualItem);
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            this.columnName.Width = 194;
            // 
            // columnType
            // 
            this.columnType.Text = "Type";
            this.columnType.Width = 164;
            // 
            // columnGUID
            // 
            this.columnGUID.Text = "GUID";
            this.columnGUID.Width = 176;
            // 
            // ViewControllers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(966, 1102);
            this.Controls.Add(this.listViewControllers);
            this.Name = "ViewControllers";
            this.Text = "Connected Game Controllers";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewControllers;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnType;
        private System.Windows.Forms.ColumnHeader columnGUID;
    }
}