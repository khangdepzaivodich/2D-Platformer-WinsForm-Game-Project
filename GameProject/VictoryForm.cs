using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameProject
{
    public partial class VictoryForm : Form
    {
        private Form currentScene;
        public VictoryForm(Form scene)
        {
            InitializeComponent();
            currentScene = scene;
        }

        private void ExitOnClick(object sender, EventArgs e)
        {
            MainMenu mainMenu = new MainMenu();
            mainMenu.Show();

            currentScene.Close();
            this.Close();
        }
    }
}
