namespace GameProject
{
    partial class RetryForm
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
            this.RetryBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.RetryBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // RetryBox1
            // 
            this.RetryBox1.BackColor = System.Drawing.Color.SlateGray;
            this.RetryBox1.BackgroundImage = global::GameProject.Properties.Resources.Icon_28;
            this.RetryBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.RetryBox1.Location = new System.Drawing.Point(91, 69);
            this.RetryBox1.Name = "RetryBox1";
            this.RetryBox1.Size = new System.Drawing.Size(52, 47);
            this.RetryBox1.TabIndex = 0;
            this.RetryBox1.TabStop = false;
            this.RetryBox1.Click += new System.EventHandler(this.ClickRetry);
            // 
            // RetryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::GameProject.Properties.Resources.Interface_windows;
            this.ClientSize = new System.Drawing.Size(234, 296);
            this.Controls.Add(this.RetryBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "RetryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RetryForm";
            ((System.ComponentModel.ISupportInitialize)(this.RetryBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox RetryBox1;
    }
}