namespace Glue.Forms
{
    partial class ViewButtons
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewButtons));
            this.textBoxButtonStates = new System.Windows.Forms.TextBox();
            this.separator = new System.Windows.Forms.Label();
            this.labelHeading = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxButtonStates
            // 
            this.textBoxButtonStates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxButtonStates.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxButtonStates.Cursor = System.Windows.Forms.Cursors.Default;
            this.textBoxButtonStates.Location = new System.Drawing.Point(13, 65);
            this.textBoxButtonStates.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxButtonStates.Multiline = true;
            this.textBoxButtonStates.Name = "textBoxButtonStates";
            this.textBoxButtonStates.ReadOnly = true;
            this.textBoxButtonStates.Size = new System.Drawing.Size(268, 544);
            this.textBoxButtonStates.TabIndex = 0;
            // 
            // separator
            // 
            this.separator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.separator.Location = new System.Drawing.Point(22, 45);
            this.separator.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.separator.Name = "separator";
            this.separator.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.separator.Size = new System.Drawing.Size(244, 1);
            this.separator.TabIndex = 1;
            this.separator.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelHeading
            // 
            this.labelHeading.AutoSize = true;
            this.labelHeading.Location = new System.Drawing.Point(15, 9);
            this.labelHeading.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(129, 25);
            this.labelHeading.TabIndex = 2;
            this.labelHeading.Text = "Pressed: {0}";
            // 
            // ViewButtons
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 622);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.separator);
            this.Controls.Add(this.textBoxButtonStates);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewButtons";
            this.ShowInTaskbar = false;
            this.Text = "Button State";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ViewButtons_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxButtonStates;
        private System.Windows.Forms.Label separator;
        private System.Windows.Forms.Label labelHeading;
    }
}