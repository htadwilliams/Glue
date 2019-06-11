namespace Glue
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.IDE_INPUTSTREAM = new System.Windows.Forms.TextBox();
            this.IDB_CLEAR = new System.Windows.Forms.Button();
            this.IDC_LOGDISPLAY = new System.Windows.Forms.CheckBox();
            this.IDC_RAWKEYNAMES = new System.Windows.Forms.CheckBox();
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.FileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.macrosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.triggersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.remapKeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // IDE_INPUTSTREAM
            // 
            this.IDE_INPUTSTREAM.AcceptsReturn = true;
            this.IDE_INPUTSTREAM.AcceptsTab = true;
            this.IDE_INPUTSTREAM.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.IDE_INPUTSTREAM.Font = new System.Drawing.Font("Courier New", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IDE_INPUTSTREAM.Location = new System.Drawing.Point(-3, 26);
            this.IDE_INPUTSTREAM.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.IDE_INPUTSTREAM.Multiline = true;
            this.IDE_INPUTSTREAM.Name = "IDE_INPUTSTREAM";
            this.IDE_INPUTSTREAM.ReadOnly = true;
            this.IDE_INPUTSTREAM.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.IDE_INPUTSTREAM.Size = new System.Drawing.Size(817, 460);
            this.IDE_INPUTSTREAM.TabIndex = 0;
            // 
            // IDB_CLEAR
            // 
            this.IDB_CLEAR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.IDB_CLEAR.Location = new System.Drawing.Point(10, 498);
            this.IDB_CLEAR.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.IDB_CLEAR.Name = "IDB_CLEAR";
            this.IDB_CLEAR.Size = new System.Drawing.Size(64, 21);
            this.IDB_CLEAR.TabIndex = 1;
            this.IDB_CLEAR.Text = "&Clear";
            this.IDB_CLEAR.UseVisualStyleBackColor = true;
            this.IDB_CLEAR.Click += new System.EventHandler(this.IDB_CLEAR_Click);
            // 
            // IDC_LOGDISPLAY
            // 
            this.IDC_LOGDISPLAY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.IDC_LOGDISPLAY.AutoSize = true;
            this.IDC_LOGDISPLAY.Location = new System.Drawing.Point(89, 502);
            this.IDC_LOGDISPLAY.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.IDC_LOGDISPLAY.Name = "IDC_LOGDISPLAY";
            this.IDC_LOGDISPLAY.Size = new System.Drawing.Size(70, 17);
            this.IDC_LOGDISPLAY.TabIndex = 2;
            this.IDC_LOGDISPLAY.Text = "&Log input";
            this.IDC_LOGDISPLAY.UseVisualStyleBackColor = true;
            // 
            // IDC_RAWKEYNAMES
            // 
            this.IDC_RAWKEYNAMES.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.IDC_RAWKEYNAMES.AutoSize = true;
            this.IDC_RAWKEYNAMES.Location = new System.Drawing.Point(158, 502);
            this.IDC_RAWKEYNAMES.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.IDC_RAWKEYNAMES.Name = "IDC_RAWKEYNAMES";
            this.IDC_RAWKEYNAMES.Size = new System.Drawing.Size(102, 17);
            this.IDC_RAWKEYNAMES.TabIndex = 3;
            this.IDC_RAWKEYNAMES.Text = "&Raw key names";
            this.IDC_RAWKEYNAMES.UseVisualStyleBackColor = true;
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
            this.menuStripMain.Padding = new System.Windows.Forms.Padding(3, 1, 0, 1);
            this.menuStripMain.Size = new System.Drawing.Size(808, 24);
            this.menuStripMain.TabIndex = 4;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileOpen,
            this.FileExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // FileOpen
            // 
            this.FileOpen.Name = "FileOpen";
            this.FileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.FileOpen.Size = new System.Drawing.Size(155, 22);
            this.FileOpen.Text = "&Open...";
            this.FileOpen.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.FileOpen.Click += new System.EventHandler(this.FileOpen_Click);
            // 
            // FileExit
            // 
            this.FileExit.Name = "FileExit";
            this.FileExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.FileExit.Size = new System.Drawing.Size(155, 22);
            this.FileExit.Text = "E&xit";
            this.FileExit.Click += new System.EventHandler(this.FileExit_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.preferencesToolStripMenuItem,
            this.macrosToolStripMenuItem,
            this.triggersToolStripMenuItem,
            this.remapKeysToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 22);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.preferencesToolStripMenuItem.Text = "&Preferences...";
            // 
            // macrosToolStripMenuItem
            // 
            this.macrosToolStripMenuItem.Name = "macrosToolStripMenuItem";
            this.macrosToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.macrosToolStripMenuItem.Text = "&Macros...";
            // 
            // triggersToolStripMenuItem
            // 
            this.triggersToolStripMenuItem.Name = "triggersToolStripMenuItem";
            this.triggersToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.triggersToolStripMenuItem.Text = "&Triggers...";
            // 
            // remapKeysToolStripMenuItem
            // 
            this.remapKeysToolStripMenuItem.Name = "remapKeysToolStripMenuItem";
            this.remapKeysToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.remapKeysToolStripMenuItem.Text = "&Remap Keys...";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.aboutToolStripMenuItem.Text = "&About Glue";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 530);
            this.Controls.Add(this.IDC_RAWKEYNAMES);
            this.Controls.Add(this.IDC_LOGDISPLAY);
            this.Controls.Add(this.IDB_CLEAR);
            this.Controls.Add(this.IDE_INPUTSTREAM);
            this.Controls.Add(this.menuStripMain);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripMain;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Main";
            this.Text = "Glue";
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox IDE_INPUTSTREAM;
        private System.Windows.Forms.Button IDB_CLEAR;
        private System.Windows.Forms.CheckBox IDC_LOGDISPLAY;
        private System.Windows.Forms.CheckBox IDC_RAWKEYNAMES;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileOpen;
        private System.Windows.Forms.ToolStripMenuItem FileExit;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem macrosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem triggersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem remapKeysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
    }
}

