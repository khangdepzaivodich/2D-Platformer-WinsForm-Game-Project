using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        private Enemy enemy;
        private List<Bullet> bullets;
        private Timer gameTimer;
        private Timer enemyShootTimer;
        private Timer bulletMoveTimer;
        private List<PictureBox> heartBoxes; 
        private Image[] heartImages;
        private Timer heartAnimationTimer;
        public Main()
        {
            InitializeComponent();
            InitializeGame();
        }
        private void InitializeGame()
        {
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            player = new Player(this);
            player.SizeMode = PictureBoxSizeMode.CenterImage;
            player.Location = new Point(100, 240);
            player.IsOnGround = false;
            this.Controls.Add(player);
            player.CreateHitBox();
            enemy = new Enemy();
            bullets = new List<Bullet>();
            enemy.Location = new Point(200, 220);
            enemy.SizeMode = PictureBoxSizeMode.CenterImage;
            this.Controls.Add(enemy);
            enemy.BringToFront();
            Ground.BringToFront();
            CreateHeartBoxes();
            SetupTimers();
            
        }
        private void SetupTimers()
        {
            gameTimer = new Timer();
            gameTimer.Interval = 20;
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            enemyShootTimer = new Timer();
            enemyShootTimer.Interval = 2000; 
            enemyShootTimer.Tick += EnemyShoot;

            bulletMoveTimer = new Timer();
            bulletMoveTimer.Interval = 20;
            bulletMoveTimer.Tick += MoveBullets;
            bulletMoveTimer.Start();


        }
        private void GameLoop(object sender, EventArgs e)
        {
            player.PlayerMove();
            player.ApplyGravity();
            player.UpdateAnimation();
            UpdateEnemyBehavior();
            CheckCollisions();
        }
        private void UpdateEnemyBehavior()
        {
            if (!enemy.isActivate)
            {
                if (enemy.IsPlayerInSight(player.Location))
                {
                    enemyShootTimer.Start();
                    enemy.isActivate = true;    
                }
            }
            if (enemy.isActivate)
            {
                if (enemy.IsPlayerInSight(player.Location))
                {
                    enemyShootTimer.Start();
                    enemy.isActivate = true;
                    enemy.isRunning = false;
                }
                else
                {
                    enemyShootTimer.Stop();
                    enemy.isRunning = true;
                    enemy.ChasePlayer(player.Location);
                }
            }
            label2.Text = enemy.IsPlayerInSight(player.Location).ToString();
            label3.Text = player.Location.ToString();
            enemy.UpdateEnemyAnimation(player.Location);
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
                        player.Top = x.Top - player.Height + 7;
                        label1.Text = "Touched Ground";
                    }
                    else
                    {
                        player.IsOnGround = false;
                        label1.Text = "Not Touch";
                    }
                }
            }
        }
        private void EnemyShoot(object sender, EventArgs e)
        {
            Bullet newBullet = enemy.Shoot(player.Location);
            bullets.Add(newBullet);
            this.Controls.Add(newBullet);
        }
        private void MoveBullets(object sender, EventArgs e)
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].BringToFront();
                bullets[i].Shoot();
                if (bullets[i].Left < 0 || bullets[i].Right > this.ClientSize.Width)
                {
                    this.Controls.Remove(bullets[i]);
                    bullets.RemoveAt(i);
                }
                else if (player.HitBox.Bounds.IntersectsWith(bullets[i].Bounds))
                {
                    player.TakeDamage(20);
                    this.Controls.Remove(bullets[i]);
                    bullets.RemoveAt(i);
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
        private void CreateHeartBoxes()
        {
            heartImages = Directory.GetFiles("HearTile", "*.png").Select(img => Image.FromFile(img)).ToArray();
            heartBoxes = new List<PictureBox>();

            for (int i = 0; i < 5; i++)
            {
                PictureBox heartBox = new PictureBox();
                heartBox.Image = heartImages[0];
                heartBox.Size = new Size(16, 16);
                heartBox.Location = new Point(i * 20, 0);
                heartBox.SizeMode = PictureBoxSizeMode.Normal;

                heartBoxes.Add(heartBox);
                this.Controls.Add(heartBox);
            }

            heartAnimationTimer = new Timer();
            heartAnimationTimer.Interval = 100;
            heartAnimationTimer.Tick += HeartAnimationTick;
            heartAnimationTimer.Start(); 
        }

        private void HeartAnimationTick(object sender, EventArgs e)
        {
            foreach (var heartBox in heartBoxes)
            {
                var currentImageIndex = Array.IndexOf(heartImages, heartBox.Image);
                currentImageIndex++;
                if (currentImageIndex >= heartImages.Length)
                {
                    currentImageIndex = 0; 
                }

                heartBox.Image = heartImages[currentImageIndex];
            }
        }
        public void RemoveHeart()
        {
            if (heartBoxes.Count > 0)
            {
                var heartToRemove = heartBoxes[heartBoxes.Count - 1]; 
                heartBoxes.RemoveAt(heartBoxes.Count - 1); 
                this.Controls.Remove(heartToRemove); 
            }
        }
    }
}
