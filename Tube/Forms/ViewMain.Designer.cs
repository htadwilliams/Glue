using System;
using System.Windows.Forms;

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
                DisposeView(viewButtons);
                DisposeView(viewQueue);
                DisposeView(viewControllers);
            }
            base.Dispose(disposing);
        }

        private void DisposeView(Form view)
        {
            if (null != view && !view.IsDisposed)
            {
                view.Close();
                view.Dispose();
            }
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
            this.checkBoxLogInput = new System.Windows.Forms.CheckBox();
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
            this.menuItemViewGameControllers = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemViewGameControllerButtons = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemViewGameControllerAxes = new System.Windows.Forms.ToolStripMenuItem();
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
            this.textBoxInputStream.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxInputStream.Font = new System.Drawing.Font("Courier New", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxInputStream.Location = new System.Drawing.Point(0, 26);
            this.textBoxInputStream.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxInputStream.Multiline = true;
            this.textBoxInputStream.Name = "textBoxInputStream";
            this.textBoxInputStream.ReadOnly = true;
            this.textBoxInputStream.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxInputStream.Size = new System.Drawing.Size(571, 358);
            this.textBoxInputStream.TabIndex = 0;
            // 
            // buttonClear
            // 
            this.buttonClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonClear.Location = new System.Drawing.Point(11, 388);
            this.buttonClear.Margin = new System.Windows.Forms.Padding(2);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(64, 21);
            this.buttonClear.TabIndex = 1;
            this.buttonClear.Text = "&Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.ButtonClear_Click);
            // 
            // checkBoxLogInput
            // 
            this.checkBoxLogInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxLogInput.AutoSize = true;
            this.checkBoxLogInput.Location = new System.Drawing.Point(79, 388);
            this.checkBoxLogInput.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxLogInput.Name = "checkBoxLogInput";
            this.checkBoxLogInput.Size = new System.Drawing.Size(70, 17);
            this.checkBoxLogInput.TabIndex = 2;
            this.checkBoxLogInput.Text = "&Log input";
            this.checkBoxLogInput.UseVisualStyleBackColor = true;
            this.checkBoxLogInput.CheckedChanged += new System.EventHandler(this.CheckBoxLogDisplay_CheckedChanged);
            // 
            // checkBoxRawKeyNames
            // 
            this.checkBoxRawKeyNames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxRawKeyNames.AutoSize = true;
            this.checkBoxRawKeyNames.Location = new System.Drawing.Point(153, 388);
            this.checkBoxRawKeyNames.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxRawKeyNames.Name = "checkBoxRawKeyNames";
            this.checkBoxRawKeyNames.Size = new System.Drawing.Size(102, 17);
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
            this.menuStripMain.Padding = new System.Windows.Forms.Padding(3, 1, 0, 1);
            this.menuStripMain.Size = new System.Drawing.Size(570, 24);
            this.menuStripMain.TabIndex = 4;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemFileOpen,
            this.menuItemFileExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // menuItemFileOpen
            // 
            this.menuItemFileOpen.Name = "menuItemFileOpen";
            this.menuItemFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuItemFileOpen.Size = new System.Drawing.Size(155, 22);
            this.menuItemFileOpen.Text = "&Open...";
            this.menuItemFileOpen.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.menuItemFileOpen.Click += new System.EventHandler(this.MenuItemFileOpen_Click);
            // 
            // menuItemFileExit
            // 
            this.menuItemFileExit.Name = "menuItemFileExit";
            this.menuItemFileExit.Size = new System.Drawing.Size(155, 22);
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
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 22);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Enabled = false;
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.preferencesToolStripMenuItem.Text = "&Preferences...";
            // 
            // macrosToolStripMenuItem
            // 
            this.macrosToolStripMenuItem.Name = "macrosToolStripMenuItem";
            this.macrosToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.macrosToolStripMenuItem.Text = "&Macros...";
            this.macrosToolStripMenuItem.Click += new System.EventHandler(this.MenuItemEditMacros_Click);
            // 
            // triggersToolStripMenuItem
            // 
            this.triggersToolStripMenuItem.Name = "triggersToolStripMenuItem";
            this.triggersToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.triggersToolStripMenuItem.Text = "&Triggers...";
            this.triggersToolStripMenuItem.Click += new System.EventHandler(this.MenuItemEditTriggers_Click);
            // 
            // remapKeysToolStripMenuItem
            // 
            this.remapKeysToolStripMenuItem.Name = "remapKeysToolStripMenuItem";
            this.remapKeysToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.remapKeysToolStripMenuItem.Text = "&Remap Keys...";
            this.remapKeysToolStripMenuItem.Click += new System.EventHandler(this.MenuItemEditRemaps_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.CheckOnClick = true;
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemViewButtons,
            this.menuItemViewQueue,
            this.menuItemViewGameControllers,
            this.menuItemViewGameControllerButtons,
            this.menuItemViewGameControllerAxes});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // menuItemViewButtons
            // 
            this.menuItemViewButtons.CheckOnClick = true;
            this.menuItemViewButtons.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.menuItemViewButtons.Name = "menuItemViewButtons";
            this.menuItemViewButtons.Size = new System.Drawing.Size(205, 22);
            this.menuItemViewButtons.Text = "&Button States";
            this.menuItemViewButtons.Click += new System.EventHandler(this.MenuItemViewButtons_Click);
            // 
            // menuItemViewQueue
            // 
            this.menuItemViewQueue.CheckOnClick = true;
            this.menuItemViewQueue.Name = "menuItemViewQueue";
            this.menuItemViewQueue.Size = new System.Drawing.Size(205, 22);
            this.menuItemViewQueue.Text = "&Queued Actions";
            this.menuItemViewQueue.Click += new System.EventHandler(this.MenuItemViewQueue_Click);
            // 
            // menuItemViewGameControllers
            // 
            this.menuItemViewGameControllers.Name = "menuItemViewGameControllers";
            this.menuItemViewGameControllers.Size = new System.Drawing.Size(205, 22);
            this.menuItemViewGameControllers.Text = "&Game Controllers";
            this.menuItemViewGameControllers.Click += new System.EventHandler(this.MenuItemViewGameControllers_Click);
            // 
            // menuItemViewGameControllerButtons
            // 
            this.menuItemViewGameControllerButtons.Name = "menuItemViewGameControllerButtons";
            this.menuItemViewGameControllerButtons.Size = new System.Drawing.Size(205, 22);
            this.menuItemViewGameControllerButtons.Text = "Game &Controller Buttons";
            // 
            // menuItemViewGameControllerAxes
            // 
            this.menuItemViewGameControllerAxes.Name = "menuItemViewGameControllerAxes";
            this.menuItemViewGameControllerAxes.Size = new System.Drawing.Size(205, 22);
            this.menuItemViewGameControllerAxes.Text = "Game Controller &Axes";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemHelpAbout});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // menuItemHelpAbout
            // 
            this.menuItemHelpAbout.Name = "menuItemHelpAbout";
            this.menuItemHelpAbout.Size = new System.Drawing.Size(134, 22);
            this.menuItemHelpAbout.Text = "&About Glue";
            this.menuItemHelpAbout.Click += new System.EventHandler(this.MenuItemHelpAbout_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMousePos,
            this.toolStripMousePosLastClick});
            this.statusStrip1.Location = new System.Drawing.Point(0, 411);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 7, 0);
            this.statusStrip1.Size = new System.Drawing.Size(570, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip";
            // 
            // toolStripMousePos
            // 
            this.toolStripMousePos.Name = "toolStripMousePos";
            this.toolStripMousePos.Size = new System.Drawing.Size(459, 17);
            this.toolStripMousePos.Spring = true;
            this.toolStripMousePos.Text = "No mouse movement detected";
            this.toolStripMousePos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripMousePosLastClick
            // 
            this.toolStripMousePosLastClick.Name = "toolStripMousePosLastClick";
            this.toolStripMousePosLastClick.Size = new System.Drawing.Size(104, 17);
            this.toolStripMousePosLastClick.Text = "No clicks detected";
            this.toolStripMousePosLastClick.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkBoxNormalizeMouseCoords
            // 
            this.checkBoxNormalizeMouseCoords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxNormalizeMouseCoords.AutoSize = true;
            this.checkBoxNormalizeMouseCoords.Location = new System.Drawing.Point(256, 470);
            this.checkBoxNormalizeMouseCoords.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxNormalizeMouseCoords.Name = "checkBoxNormalizeMouseCoords";
            this.checkBoxNormalizeMouseCoords.Size = new System.Drawing.Size(164, 17);
            this.checkBoxNormalizeMouseCoords.TabIndex = 6;
            this.checkBoxNormalizeMouseCoords.Text = "&Normalize mouse coordinates";
            this.checkBoxNormalizeMouseCoords.UseVisualStyleBackColor = true;
            // 
            // ViewMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 433);
            this.Controls.Add(this.checkBoxNormalizeMouseCoords);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.checkBoxRawKeyNames);
            this.Controls.Add(this.checkBoxLogInput);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.textBoxInputStream);
            this.Controls.Add(this.menuStripMain);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripMain;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ViewMain";
            this.Text = "Glue";
            this.Load += new System.EventHandler(this.ViewMain_Load);
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
        private System.Windows.Forms.CheckBox checkBoxLogInput;
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
        private System.Windows.Forms.ToolStripMenuItem menuItemViewQueue;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripMousePos;
        private System.Windows.Forms.ToolStripStatusLabel toolStripMousePosLastClick;
        private System.Windows.Forms.CheckBox checkBoxNormalizeMouseCoords;
        private System.Windows.Forms.ToolStripMenuItem menuItemViewGameControllers;
        private System.Windows.Forms.ToolStripMenuItem menuItemViewGameControllerButtons;
        private System.Windows.Forms.ToolStripMenuItem menuItemViewGameControllerAxes;
    }
}

