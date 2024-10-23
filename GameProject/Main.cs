﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameProject
{
    public partial class Main : Form
    {
        private Player player;
        private Enemy enemy, enemy2;
        private List<Bullet> bullets;
        private Timer gameTimer;
        private Timer enemyShootTimer;
        private Timer bulletMoveTimer;
        private List<PictureBox> heartBoxes;
        private Image[] heartImages;
        private Timer heartAnimationTimer;
        private List<string> shootLeft;
        private List<string> shootRight;

        private List<PictureBox> backgrounds;
        private DateTime lastAttackTime;
        private const int attackCooldown = 700;
        private bool Scene2OP = false;
        private int currentHealth;
        private int heartRemoveCounter = 0;

        public Main()
        {
            InitializeComponent();
            InitializeGame();
        }
        
        private void InitializeGame()
        {

            this.DoubleBuffered = true;
            this.KeyPreview = true;

            
            shootLeft = Directory.GetFiles("EnemyShot_Left", "*.png").ToList();
            shootRight = Directory.GetFiles("EnemyShot_Right", "*.png").ToList();

            player = new Player();

            player.SizeMode = PictureBoxSizeMode.CenterImage;
            player.Location = new Point(100, 490);
            player.IsOnGround = false;
            player.BackColor = Color.Transparent;
            this.Controls.Add(player);
            player.CreateHitBox();
            player.HitBox.BorderStyle = BorderStyle.FixedSingle;

            enemy = new RangedEnemy();
            bullets = new List<Bullet>();
            enemy.Location = new Point(900, 490);
            enemy.SizeMode = PictureBoxSizeMode.CenterImage;
            this.Controls.Add(enemy);

            enemy2 = new MeleeEnemy();
            enemy2.Location = new Point(700, 490);
            enemy2.SizeMode = PictureBoxSizeMode.CenterImage;
            this.Controls.Add(enemy2);
            enemy2.CreateHitBox();
            enemy2.hitBox.BorderStyle = BorderStyle.FixedSingle;

            enemy.BringToFront();
            enemy2.BringToFront();
            
            
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "Ground")
                {
                    x.BringToFront();
                }
            }
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
            player.UpdateHitboxPosition();
            enemy2.UpdateHitboxPosition();
            player.PlayerMove();
            player.ApplyGravity();
            enemy.ApplyGravity();
            enemy2.ApplyGravity();
            player.UpdateAnimation();
            UpdateEnemyBehavior();
            CheckCollisions();
            CheckTakeDeflectedBullet();

            if (player.Location.X > this.Width && !Scene2OP)
            {
                this.Hide();
                Scene2OP = true;
                currentHealth = player.Health;
                Scene2 scene2 = new Scene2(currentHealth, heartRemoveCounter);
                scene2.Show();
                scene2.Focus();
            }
        }
        private void CheckTakeDeflectedBullet()
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                if (bullets[i].deflected)
                {
                    foreach (Control x in this.Controls)
                    {
                        if (x is MeleeEnemy || x is RangedEnemy)
                        {
                            if (x is MeleeEnemy meleeEnemy)
                            {
                                if (bullets[i].Bounds.IntersectsWith(meleeEnemy.hitBox.Bounds))
                                {
                                    meleeEnemy.TakeDamage(10, this.Location);
                                    bullets[i].Dispose();
                                    bullets.RemoveAt(i);
                                }
                            }
                            else if (x is RangedEnemy rangedEnemy)
                            {
                                if (bullets[i].Bounds.IntersectsWith(rangedEnemy.Bounds))
                                {
                                    rangedEnemy.TakeDamage(10, this.Location);
                                    bullets[i].Dispose();
                                    bullets.RemoveAt(i);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateEnemyBehavior()
        {
            if (player.isDead)
            {
                enemy.isActivate = false;
                enemyShootTimer.Stop();
                enemy2.isActivate = false;
                enemy.Patrol();
                enemy2.Patrol();
                return;
            }
            if (!enemy.isActivate)
            {
                enemy.Patrol();
                if (enemy.IsPlayerInSight(player.HitBox.Location))
                {
                    enemyShootTimer.Start();
                    enemy.isActivate = true;
                }
            }

            if (enemy.isActivate && !enemy.IsDead)
            {
                bool isRight = enemy.Left > player.Left;
                if (enemy.IsPlayerInSight(player.HitBox.Location))
                {
                    if (enemy.isFirst)
                    {
                        enemy.isFirst = false;
                        enemy.Image = Image.FromFile(isRight ? shootLeft[0] : shootRight[0]);
                    }
                    enemyShootTimer.Start();
                    enemy.isActivate = true;
                    enemy.isRunning = false;
                }
                else
                {
                    enemy.isFirst = true;
                    enemyShootTimer.Stop();
                    enemy.isRunning = true;
                    enemy.ChasePlayer(player.HitBox.Location);
                }
            }

            if (!enemy2.isActivate && !enemy2.isAttacking && !player.isDead)
            {
                enemy2.Patrol();
                if (enemy2.IsPlayerInSight(player.HitBox.Location))
                {
                    enemy2.isActivate = true;
                }
            }

            if (enemy2.isActivate && !enemy2.IsDead)
            {
                if (enemy2.Bounds.IntersectsWith(player.HitBox.Bounds))
                {
                    if (!player.isDead && !enemy2.isAttacking)
                    {
                        enemy2.isRunning = false;
                        enemy2.Attack(player.Location);
                        enemy2.isAttacking = true;
                        player.TakeDamage(20);
                        RemoveHeart();
                    }
                }
                else
                {
                    enemy2.isActivate = true;
                    enemy2.isRunning = true;
                    enemy2.ChasePlayer(player.HitBox.Location);
                    enemy2.isAttacking = false;
                }
            }


            enemy2.UpdateEnemyAnimation(player.HitBox.Location);
            label2.Text = enemy.IsPlayerInSight(player.HitBox.Location).ToString();
            label3.Text = player.Location.ToString();
            enemy.UpdateEnemyAnimation(player.HitBox.Location);
        }
        private void CheckCollisions()
        {
            foreach (Control x in this.Controls)
            {
                if (x is Enemy enemy1)
                {
                    if (enemy1.IsDead)
                        continue;
                    if (player.Bounds.IntersectsWith(enemy1.Bounds))
                    {
                        if (player.IsAttacking)
                        {
                            enemy1.TakeDamage(10, this.Location);
                        }
                        else if (!enemy1.isAttacking)
                        {
                            enemy1.Attack(player.Location);
                        }
                    }
                }
                bool flag = false;
                foreach (Control y in this.Controls)
                {
                    if (y is PictureBox && (string)y.Tag == "Ground")
                    {
                        if (player.HitBox.Bounds.IntersectsWith(y.Bounds) && player.Height > y.Height)
                        {
                            flag = true;
                            player.IsFalling = false;
                            player.IsJumping = false;
                            player.IsOnGround = true;
                            player.Top = y.Top - player.Height + 7;
                            label1.Text = "Touched Ground";
                            break;
                        }
                    }
                }
                if (!flag)
                {
                    player.IsOnGround = false;
                }
                flag = false;
                if (x is PictureBox && (string)x.Tag == "Ground")
                {
                    if (enemy.Bounds.IntersectsWith(x.Bounds))
                    {
                        enemy.IsOnGround = true;
                        enemy.Top = x.Top - enemy.Height + 2;
                    }
                    else
                    {
                        enemy.IsOnGround = false;
                    }
                    if (enemy2.Bounds.IntersectsWith(x.Bounds))
                    {
                        enemy2.IsOnGround = true;
                        enemy2.Top = x.Top - enemy2.Height + 12;
                    }
                    else
                    {
                        enemy2.IsOnGround = false;
                    }
                }
            }
        }
        private void EnemyShoot(object sender, EventArgs e)
        {
            if (!enemy.IsDead)
            {
                Bullet newBullet = enemy.Shoot(player.HitBox.Location);
                bullets.Add(newBullet);
                this.Controls.Add(newBullet);
            }
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
                else if (player.Bounds.IntersectsWith(bullets[i].Bounds) && player.IsAttacking && ((bullets[i].IsMovingRight && player.IsFlipped) || (!bullets[i].IsMovingRight && !player.IsFlipped)))
                {
                    ScreenShake();
                    DeflectParticle deflectParticle = new DeflectParticle(new Point(bullets[i].Left, bullets[i].Top));
                    this.Controls.Add(deflectParticle);
                    deflectParticle.BringToFront();
                    bullets[i].IsMovingRight = !bullets[i].IsMovingRight;
                    bullets[i].Speed += 25;
                    bullets[i].deflected = true;
                }
                else if (player.HitBox.Bounds.IntersectsWith(bullets[i].Bounds))
                {
                    player.TakeDamage(20);
                    this.Controls.Remove(bullets[i]);
                    bullets.RemoveAt(i);
                    RemoveHeart();
                }
            }
        }
        private void ScreenShake()
        {
            int shakeAmount = 5;
            int shakeDuration = 100;
            int shakeInterval = 20;

            Timer shakeTimer = new Timer();
            shakeTimer.Interval = shakeInterval;

            int elapsed = 0;
            Random rnd = new Random();
            Point originalLocation = this.Location;

            shakeTimer.Tick += (s, e) =>
            {
                elapsed += shakeInterval;
                if (elapsed >= shakeDuration)
                {
                    this.Location = originalLocation;
                    shakeTimer.Stop();
                }
                else
                {
                    int offsetX = rnd.Next(-shakeAmount, shakeAmount);
                    int offsetY = rnd.Next(-shakeAmount, shakeAmount);
                    this.Location = new Point(originalLocation.X + offsetX, originalLocation.Y + offsetY);
                }
            };

            shakeTimer.Start();
        }
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                player.IsLeft = true;
                player.IsRight = false;
            }
            if (e.KeyCode == Keys.D)
            {
                player.IsRight = true;
                player.IsLeft = false;
            }
            if (e.KeyCode == Keys.Space)
            {
                player.Jump();
            }
        }
        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                player.IsLeft = false;
            }
            if (e.KeyCode == Keys.D)
            {
                player.IsRight = false;
            }
        }
        private void MouseIsDown(object sender, MouseEventArgs e)
        {
            if ((DateTime.Now - lastAttackTime).TotalMilliseconds >= attackCooldown)
            {
                player.IsAttacking = true;
                lastAttackTime = DateTime.Now;
            }
        }
        private void CreateHeartBoxes()
        {
            heartImages = Directory.GetFiles("HearTile", "*.png").Select(img => Image.FromFile(img)).ToArray();
            heartBoxes = new List<PictureBox>();

            for (int i = 0; i < 5; i++)
            {
                PictureBox heartBox = new PictureBox();
                heartBox.Image = heartImages[0];
                heartBox.Size = new Size(30, 30);
                heartBox.Location = new Point(i * 35, 0);
                heartBox.SizeMode = PictureBoxSizeMode.CenterImage;
                heartBox.BackColor = Color.FromArgb(255, 24, 36, 52);
               
                heartBoxes.Add(heartBox);
                this.Controls.Add(heartBox);
                heartBox.BringToFront();
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

        private void Main_Load(object sender, EventArgs e)
        {

        }

        public void RemoveHeart()
        {
            if (heartBoxes.Count > 0)
            {
                var heartToRemove = heartBoxes[heartBoxes.Count - 1];
                heartBoxes.RemoveAt(heartBoxes.Count - 1);
                this.Controls.Remove(heartToRemove);
                heartRemoveCounter++;
            }
        }
    }
}