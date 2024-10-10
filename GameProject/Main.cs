using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameProject
{
    public partial class Main : Form
    {
        private Player player;
        private Timer gameTimer;
        public Main()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            player = new Player();
            player.SizeMode = PictureBoxSizeMode.CenterImage;
            player.Location = new Point(100, 240);
            player.IsOnGround = false;
            this.Controls.Add(player);
            player.CreateHitBox();
            SetupTimers();
        }
        private void SetupTimers()
        {
            gameTimer = new Timer();
            gameTimer.Interval = 20;
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }
        private void GameLoop(object sender, EventArgs e)
        {
            player.PlayerMove();
            player.ApplyGravity();
            player.UpdateAnimation();
            CheckCollisions();
        }
        private void CheckCollisions()
        {
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "Ground")
                {
                    
                    if (player.HitBox.Bounds.IntersectsWith(x.Bounds))
                    {
                        player.IsOnGround = true;
                        player.Top = x.Top - player.Height + 1;
                        label1.Text = "Touched Ground";
                    }
                    else
                    {
                        label1.Text = "Not Touch";
                    }
                }
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.A)
            {
                player.IsLeft = true;
                player.IsRight = false;
            }
            if(e.KeyCode == Keys.D)
            {
                player.IsRight = true;
                player.IsLeft = false;
            }
            if(e.KeyCode == Keys.Space)
            {
                player.Jump();
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.A)
            {
                player.IsLeft = false;
            }
            if( e.KeyCode == Keys.D)
            {
                player.IsRight = false;
            }
        }

        private void MouseIsDown(object sender, MouseEventArgs e)
        {
            player.IsAttacking = true;
        }
    }
}
