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
            this.SuspendLayout();
            // 
            // textBoxButtonStates
            // 
            this.textBoxButtonStates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxButtonStates.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxButtonStates.Cursor = System.Windows.Forms.Cursors.Default;
            this.textBoxButtonStates.Location = new System.Drawing.Point(11, 11);
            this.textBoxButtonStates.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxButtonStates.Multiline = true;
            this.textBoxButtonStates.Name = "textBoxButtonStates";
            this.textBoxButtonStates.ReadOnly = true;
            this.textBoxButtonStates.Size = new System.Drawing.Size(104, 224);
            this.textBoxButtonStates.TabIndex = 0;
            // 
            // ViewButtons
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(126, 246);
            this.Controls.Add(this.textBoxButtonStates);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewButtons";
            this.ShowInTaskbar = false;
            this.Text = "Buttons";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxButtonStates;
    }
}