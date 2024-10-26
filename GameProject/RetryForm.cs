using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameProject
{
    public partial class RetryForm : Form
    {
        private Form currentScene;
        public RetryForm(Form scene)
        {
            InitializeComponent();
            currentScene = scene;
            RetryBox1.Left = (this.ClientSize.Width - RetryBox1.Width) / 2;
            QuitBox1.Left = (this.ClientSize.Width - RetryBox1.Width) / 2;
            QuitBox1.Top = RetryBox1.Bottom + 40;
        }

        private void ClickRetry(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Retry;
            this.Close();
        }

        private void ClickMainMenu(object sender, EventArgs e)
        {
            MainMenu mainMenu = new MainMenu();
            mainMenu.Show();

            currentScene.Close();
            this.Close();
        }
    }
}
