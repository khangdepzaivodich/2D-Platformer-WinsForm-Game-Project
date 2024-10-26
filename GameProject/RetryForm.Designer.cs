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
            this.QuitBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.RetryBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.QuitBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // RetryBox1
            // 
            this.RetryBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(38)))), ((int)(((byte)(93)))));
            this.RetryBox1.BackgroundImage = global::GameProject.Properties.Resources.Icon_28;
            this.RetryBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.RetryBox1.Location = new System.Drawing.Point(95, 55);
            this.RetryBox1.Margin = new System.Windows.Forms.Padding(2);
            this.RetryBox1.Name = "RetryBox1";
            this.RetryBox1.Size = new System.Drawing.Size(49, 37);
            this.RetryBox1.TabIndex = 0;
            this.RetryBox1.TabStop = false;
            this.RetryBox1.Click += new System.EventHandler(this.ClickRetry);
            // 
            // QuitBox1
            // 
            this.QuitBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(38)))), ((int)(((byte)(93)))));
            this.QuitBox1.BackgroundImage = global::GameProject.Properties.Resources.Icon_35;
            this.QuitBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.QuitBox1.Location = new System.Drawing.Point(95, 122);
            this.QuitBox1.Name = "QuitBox1";
            this.QuitBox1.Size = new System.Drawing.Size(49, 37);
            this.QuitBox1.TabIndex = 1;
            this.QuitBox1.TabStop = false;
            this.QuitBox1.Click += new System.EventHandler(this.ClickMainMenu);
            // 
            // RetryForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackgroundImage = global::GameProject.Properties.Resources.Interface_windows88;
            this.ClientSize = new System.Drawing.Size(241, 227);
            this.Controls.Add(this.QuitBox1);
            this.Controls.Add(this.RetryBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "RetryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RetryForm";
            ((System.ComponentModel.ISupportInitialize)(this.RetryBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.QuitBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox RetryBox1;
        private System.Windows.Forms.PictureBox QuitBox1;
    }
}