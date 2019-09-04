namespace Glue.Forms
{
    partial class ViewMain
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
            if (disposing)
            {
                if (null != viewButtons && !viewButtons.IsDisposed)
                {
                    viewButtons.Close();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewMain));
            this.textBoxInputStream = new System.Windows.Forms.TextBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.checkBoxLogDisplay = new System.Windows.Forms.CheckBox();
            this.checkBoxRawKeyNames = new System.Windows.Forms.CheckBox();
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.macrosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.triggersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.remapKeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemViewButtons = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemViewQueue = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripMousePos = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripMousePosLastClick = new System.Windows.Forms.ToolStripStatusLabel();
            this.checkBoxNormalizeMouseCoords = new System.Windows.Forms.CheckBox();
            this.menuStripMain.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxInputStream
            // 
            this.textBoxInputStream.AcceptsReturn = true;
            this.textBoxInputStream.AcceptsTab = true;
            this.textBoxInputStream.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxInputStream.Font = new System.Drawing.Font("Courier New", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxInputStream.Location = new System.Drawing.Point(-2, 50);
            this.textBoxInputStream.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxInputStream.Multiline = true;
            this.textBoxInputStream.Name = "textBoxInputStream";
            this.textBoxInputStream.ReadOnly = true;
            this.textBoxInputStream.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxInputStream.Size = new System.Drawing.Size(1476, 850);
            this.textBoxInputStream.TabIndex = 0;
            // 
            // buttonClear
            // 
            this.buttonClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonClear.Location = new System.Drawing.Point(13, 908);
            this.buttonClear.Margin = new System.Windows.Forms.Padding(4);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(128, 40);
            this.buttonClear.TabIndex = 1;
            this.buttonClear.Text = "&Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.ButtonClear_Click);
            // 
            // checkBoxLogDisplay
            // 
            this.checkBoxLogDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxLogDisplay.AutoSize = true;
            this.checkBoxLogDisplay.Location = new System.Drawing.Point(171, 920);
            this.checkBoxLogDisplay.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxLogDisplay.Name = "checkBoxLogDisplay";
            this.checkBoxLogDisplay.Size = new System.Drawing.Size(133, 29);
            this.checkBoxLogDisplay.TabIndex = 2;
            this.checkBoxLogDisplay.Text = "&Log input";
            this.checkBoxLogDisplay.UseVisualStyleBackColor = true;
            // 
            // checkBoxRawKeyNames
            // 
            this.checkBoxRawKeyNames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxRawKeyNames.AutoSize = true;
            this.checkBoxRawKeyNames.Location = new System.Drawing.Point(309, 920);
            this.checkBoxRawKeyNames.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxRawKeyNames.Name = "checkBoxRawKeyNames";
            this.checkBoxRawKeyNames.Size = new System.Drawing.Size(196, 29);
            this.checkBoxRawKeyNames.TabIndex = 3;
            this.checkBoxRawKeyNames.Text = "&Raw key names";
            this.checkBoxRawKeyNames.UseVisualStyleBackColor = true;
            // 
            // menuStripMain
            // 
            this.menuStripMain.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(1478, 40);
            this.menuStripMain.TabIndex = 4;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemFileOpen,
            this.menuItemFileExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(72, 36);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // menuItemFileOpen
            // 
            this.menuItemFileOpen.Name = "menuItemFileOpen";
            this.menuItemFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuItemFileOpen.Size = new System.Drawing.Size(309, 44);
            this.menuItemFileOpen.Text = "&Open...";
            this.menuItemFileOpen.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.menuItemFileOpen.Click += new System.EventHandler(this.MenuItemFileOpen_Click);
            // 
            // menuItemFileExit
            // 
            this.menuItemFileExit.Name = "menuItemFileExit";
            this.menuItemFileExit.Size = new System.Drawing.Size(309, 44);
            this.menuItemFileExit.Text = "E&xit";
            this.menuItemFileExit.Click += new System.EventHandler(this.MenuItemFileExit_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.preferencesToolStripMenuItem,
            this.macrosToolStripMenuItem,
            this.triggersToolStripMenuItem,
            this.remapKeysToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(75, 36);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Enabled = false;
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(293, 44);
            this.preferencesToolStripMenuItem.Text = "&Preferences...";
            // 
            // macrosToolStripMenuItem
            // 
            this.macrosToolStripMenuItem.Name = "macrosToolStripMenuItem";
            this.macrosToolStripMenuItem.Size = new System.Drawing.Size(293, 44);
            this.macrosToolStripMenuItem.Text = "&Macros...";
            this.macrosToolStripMenuItem.Click += new System.EventHandler(this.MenuItemEditMacros_Click);
            // 
            // triggersToolStripMenuItem
            // 
            this.triggersToolStripMenuItem.Name = "triggersToolStripMenuItem";
            this.triggersToolStripMenuItem.Size = new System.Drawing.Size(293, 44);
            this.triggersToolStripMenuItem.Text = "&Triggers...";
            this.triggersToolStripMenuItem.Click += new System.EventHandler(this.MenuItemEditTriggers_Click);
            // 
            // remapKeysToolStripMenuItem
            // 
            this.remapKeysToolStripMenuItem.Name = "remapKeysToolStripMenuItem";
            this.remapKeysToolStripMenuItem.Size = new System.Drawing.Size(293, 44);
            this.remapKeysToolStripMenuItem.Text = "&Remap Keys...";
            this.remapKeysToolStripMenuItem.Click += new System.EventHandler(this.MenuItemEditRemaps_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.CheckOnClick = true;
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemViewButtons,
            this.menuItemViewQueue});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(86, 36);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // menuItemViewButtons
            // 
            this.menuItemViewButtons.CheckOnClick = true;
            this.menuItemViewButtons.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.menuItemViewButtons.Name = "menuItemViewButtons";
            this.menuItemViewButtons.Size = new System.Drawing.Size(359, 44);
            this.menuItemViewButtons.Text = "&Button States";
            this.menuItemViewButtons.Click += new System.EventHandler(this.MenuItemViewButtons_Click);
            // 
            // menuItemViewQueue
            // 
            this.menuItemViewQueue.CheckOnClick = true;
            this.menuItemViewQueue.Name = "menuItemViewQueue";
            this.menuItemViewQueue.Size = new System.Drawing.Size(359, 44);
            this.menuItemViewQueue.Text = "&Queued Actions";
            this.menuItemViewQueue.Click += new System.EventHandler(this.MenuItemViewQueue_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemHelpAbout});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(85, 36);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // menuItemHelpAbout
            // 
            this.menuItemHelpAbout.Name = "menuItemHelpAbout";
            this.menuItemHelpAbout.Size = new System.Drawing.Size(270, 44);
            this.menuItemHelpAbout.Text = "&About Glue";
            this.menuItemHelpAbout.Click += new System.EventHandler(this.MenuItemHelpAbout_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMousePos,
            this.toolStripMousePosLastClick});
            this.statusStrip1.Location = new System.Drawing.Point(0, 952);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1478, 42);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripMousePos
            // 
            this.toolStripMousePos.Name = "toolStripMousePos";
            this.toolStripMousePos.Size = new System.Drawing.Size(1190, 32);
            this.toolStripMousePos.Spring = true;
            this.toolStripMousePos.Text = "No mouse movement detected";
            this.toolStripMousePos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripMousePosLastClick
            // 
            this.toolStripMousePosLastClick.Name = "toolStripMousePosLastClick";
            this.toolStripMousePosLastClick.Size = new System.Drawing.Size(211, 32);
            this.toolStripMousePosLastClick.Text = "No clicks detected";
            this.toolStripMousePosLastClick.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkBoxNormalizeMouseCoords
            // 
            this.checkBoxNormalizeMouseCoords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxNormalizeMouseCoords.AutoSize = true;
            this.checkBoxNormalizeMouseCoords.Location = new System.Drawing.Point(513, 920);
            this.checkBoxNormalizeMouseCoords.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxNormalizeMouseCoords.Name = "checkBoxNormalizeMouseCoords";
            this.checkBoxNormalizeMouseCoords.Size = new System.Drawing.Size(328, 29);
            this.checkBoxNormalizeMouseCoords.TabIndex = 6;
            this.checkBoxNormalizeMouseCoords.Text = "&Normalize mouse coordinates";
            this.checkBoxNormalizeMouseCoords.UseVisualStyleBackColor = true;
            // 
            // ViewMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1478, 994);
            this.Controls.Add(this.checkBoxNormalizeMouseCoords);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.checkBoxRawKeyNames);
            this.Controls.Add(this.checkBoxLogDisplay);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.textBoxInputStream);
            this.Controls.Add(this.menuStripMain);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripMain;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ViewMain";
            this.Text = "Glue";
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxInputStream;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.CheckBox checkBoxLogDisplay;
        private System.Windows.Forms.CheckBox checkBoxRawKeyNames;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemFileOpen;
        private System.Windows.Forms.ToolStripMenuItem menuItemFileExit;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemHelpAbout;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem macrosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem triggersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem remapKeysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemViewButtons;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripMousePos;
        private System.Windows.Forms.ToolStripStatusLabel toolStripMousePosLastClick;
        private System.Windows.Forms.CheckBox checkBoxNormalizeMouseCoords;
    }
}

