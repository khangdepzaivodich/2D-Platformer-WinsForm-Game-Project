namespace GameProject
{
    partial class MainMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainMenu));
            this.MainTitle = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.PlayBox = new System.Windows.Forms.PictureBox();
            this.ExitBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.MainTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PlayBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExitBox)).BeginInit();
            this.SuspendLayout();
            // 
            // MainTitle
            // 
            this.MainTitle.BackColor = System.Drawing.Color.Transparent;
            this.MainTitle.BackgroundImage = global::GameProject.Properties.Resources.Logo1;
            this.MainTitle.Location = new System.Drawing.Point(1169, 235);
            this.MainTitle.Name = "MainTitle";
            this.MainTitle.Size = new System.Drawing.Size(286, 115);
            this.MainTitle.TabIndex = 0;
            this.MainTitle.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(92)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(1197, 260);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(282, 42);
            this.label1.TabIndex = 1;
            this.label1.Text = "WardenOfTime";
            // 
            // PlayBox
            // 
            this.PlayBox.BackColor = System.Drawing.Color.Transparent;
            this.PlayBox.BackgroundImage = global::GameProject.Properties.Resources.PlayBtn;
            this.PlayBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PlayBox.Location = new System.Drawing.Point(1246, 374);
            this.PlayBox.Name = "PlayBox";
            this.PlayBox.Size = new System.Drawing.Size(145, 76);
            this.PlayBox.TabIndex = 2;
            this.PlayBox.TabStop = false;
            this.PlayBox.Click += new System.EventHandler(this.ClickPlay);
            // 
            // ExitBox
            // 
            this.ExitBox.BackColor = System.Drawing.Color.Transparent;
            this.ExitBox.BackgroundImage = global::GameProject.Properties.Resources.ExitBtn;
            this.ExitBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ExitBox.Location = new System.Drawing.Point(1246, 477);
            this.ExitBox.Name = "ExitBox";
            this.ExitBox.Size = new System.Drawing.Size(145, 75);
            this.ExitBox.TabIndex = 3;
            this.ExitBox.TabStop = false;
            this.ExitBox.Click += new System.EventHandler(this.ClickExit);
            // 
            // MainMenu
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1507, 778);
            this.Controls.Add(this.ExitBox);
            this.Controls.Add(this.PlayBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MainTitle);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainMenu";
            ((System.ComponentModel.ISupportInitialize)(this.MainTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PlayBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExitBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox MainTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox PlayBox;
        private System.Windows.Forms.PictureBox ExitBox;
    }
}