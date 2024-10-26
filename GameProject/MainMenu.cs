using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameProject
{
    public partial class MainMenu : Form
    {
        private List<string> mmbackground;
        public MainMenu()
        {
            InitializeComponent();
            InitializeMM();
        }

        private void InitializeMM()
        {
            mmbackground = Directory.GetFiles("MMbackground", "*.png").ToList();
            this.BackgroundImage = Image.FromFile(mmbackground[0]);
            this.BackgroundImageLayout = ImageLayout.Stretch;
            label1.Left = ((this.ClientSize.Width - label1.Width) / 2);
            MainTitle.Left = (this.ClientSize.Width - MainTitle.Width) / 2;

            PlayBox.Left = (this.ClientSize.Width - PlayBox.Width) / 2;
            PlayBox.Top = MainTitle.Bottom + 20;

            ExitBox.Left = (this.ClientSize.Width - ExitBox.Width) / 2;
            ExitBox.Top = PlayBox.Bottom + 20;
        }

        private void ClickPlay(object sender, EventArgs e)
        {
            Main.HeartState.Hearts = 5;
            Main mainForm = new Main();
            mainForm.Show();
            this.Hide();
        }

        private void ClickExit(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
