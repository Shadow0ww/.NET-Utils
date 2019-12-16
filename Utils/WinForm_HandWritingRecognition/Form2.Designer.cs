namespace WinForm_HandWritingRecognition
{
    partial class InkRecognition
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
            this.gbInkArea = new System.Windows.Forms.GroupBox();
            this.btnRecognize = new System.Windows.Forms.Button();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // gbInkArea
            // 
            this.gbInkArea.Location = new System.Drawing.Point(10, 0);
            this.gbInkArea.Name = "gbInkArea";
            this.gbInkArea.Size = new System.Drawing.Size(336, 146);
            this.gbInkArea.TabIndex = 0;
            this.gbInkArea.TabStop = false;
            this.gbInkArea.Text = "Ink Here";
            // 
            // btnRecognize
            // 
            this.btnRecognize.Location = new System.Drawing.Point(10, 155);
            this.btnRecognize.Name = "btnRecognize";
            this.btnRecognize.Size = new System.Drawing.Size(336, 25);
            this.btnRecognize.TabIndex = 1;
            this.btnRecognize.Text = "Recognize Ink";
            this.btnRecognize.Click += new System.EventHandler(this.btnRecognize_Click);
            // 
            // txtResults
            // 
            this.txtResults.BackColor = System.Drawing.SystemColors.Window;
            this.txtResults.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtResults.Location = new System.Drawing.Point(10, 190);
            this.txtResults.Name = "txtResults";
            this.txtResults.ReadOnly = true;
            this.txtResults.Size = new System.Drawing.Size(336, 21);
            this.txtResults.TabIndex = 2;
            // 
            // InkRecognition
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(362, 236);
            this.Controls.Add(this.txtResults);
            this.Controls.Add(this.btnRecognize);
            this.Controls.Add(this.gbInkArea);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "InkRecognition";
            this.Text = "Ink Recognition Sample";
            this.Load += new System.EventHandler(this.InkRecognition_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}