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
            this.IDE_INPUTSTREAM = new System.Windows.Forms.TextBox();
            this.IDB_CLEAR = new System.Windows.Forms.Button();
            this.IDC_LOGDISPLAY = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // IDE_INPUTSTREAM
            // 
            this.IDE_INPUTSTREAM.AcceptsReturn = true;
            this.IDE_INPUTSTREAM.AcceptsTab = true;
            this.IDE_INPUTSTREAM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.IDE_INPUTSTREAM.Font = new System.Drawing.Font("Courier New", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IDE_INPUTSTREAM.Location = new System.Drawing.Point(-6, -3);
            this.IDE_INPUTSTREAM.Multiline = true;
            this.IDE_INPUTSTREAM.Name = "IDE_INPUTSTREAM";
            this.IDE_INPUTSTREAM.ReadOnly = true;
            this.IDE_INPUTSTREAM.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.IDE_INPUTSTREAM.Size = new System.Drawing.Size(1156, 508);
            this.IDE_INPUTSTREAM.TabIndex = 0;
            // 
            // IDB_CLEAR
            // 
            this.IDB_CLEAR.Location = new System.Drawing.Point(294, 511);
            this.IDB_CLEAR.Name = "IDB_CLEAR";
            this.IDB_CLEAR.Size = new System.Drawing.Size(129, 41);
            this.IDB_CLEAR.TabIndex = 1;
            this.IDB_CLEAR.Text = "&Clear";
            this.IDB_CLEAR.UseVisualStyleBackColor = true;
            this.IDB_CLEAR.Click += new System.EventHandler(this.IDB_CLEAR_Click);
            // 
            // IDC_LOGDISPLAY
            // 
            this.IDC_LOGDISPLAY.AutoSize = true;
            this.IDC_LOGDISPLAY.Location = new System.Drawing.Point(452, 523);
            this.IDC_LOGDISPLAY.Name = "IDC_LOGDISPLAY";
            this.IDC_LOGDISPLAY.Size = new System.Drawing.Size(200, 29);
            this.IDC_LOGDISPLAY.TabIndex = 2;
            this.IDC_LOGDISPLAY.Text = "&Input log display";
            this.IDC_LOGDISPLAY.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1142, 578);
            this.Controls.Add(this.IDC_LOGDISPLAY);
            this.Controls.Add(this.IDB_CLEAR);
            this.Controls.Add(this.IDE_INPUTSTREAM);
            this.Name = "Main";
            this.Text = "Glue";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox IDE_INPUTSTREAM;
        private System.Windows.Forms.Button IDB_CLEAR;
        private System.Windows.Forms.CheckBox IDC_LOGDISPLAY;
    }
}

