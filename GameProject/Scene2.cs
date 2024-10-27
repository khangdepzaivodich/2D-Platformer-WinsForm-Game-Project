using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GameProject.Main;

namespace GameProject
{
    public partial class Scene2 : Form
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
        private List<SoundPlayer> skillsounds;
        private DateTime lastAttackTime;
        private const int attackCooldown = 1000;
        public int heartRemoveCT = 0;
        private bool Scene3OP = false;
        private bool retryFormCheck = false;
        private int currentHealth;
        private int heartRemoveCounter2 = 0;
        private bool slowTime = false;
        private float fadeAmount = 0f;
        public Scene2(int health, int heartRemoveCounter)
        {
            InitializeComponent();
            InitializeGame();
            LoadSoundEffect();
            
            player.Health = health;
            CreateHeartBoxes();
        }
        private void InitializeGame()
        {
            this.DoubleBuffered = true;
            this.KeyPreview = true;

            shootLeft = Directory.GetFiles("EnemyShot_Left", "*.png").ToList();
            shootRight = Directory.GetFiles("EnemyShot_Right", "*.png").ToList();

            player = new Player();
            player.SizeMode = PictureBoxSizeMode.CenterImage;
            player.Location = new Point(0, 381);
            player.IsOnGround = false;
            player.BackColor = Color.Transparent;
            this.Controls.Add(player);
            player.CreateHitBox();

            enemy = new RangedEnemy();
            bullets = new List<Bullet>();
            enemy.Location = new Point(1500, 325);
            enemy.SizeMode = PictureBoxSizeMode.CenterImage;
            this.Controls.Add(enemy);

            enemy2 = new MeleeEnemy();
            enemy2.Location = new Point(800, 325);
            enemy2.SizeMode = PictureBoxSizeMode.CenterImage;
            this.Controls.Add(enemy2);
            enemy2.CreateHitBox();

            player.BringToFront();
            enemy.BringToFront();
            enemy2.BringToFront();

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "Ground")
                {
                    x.BringToFront();
                }
                if (x is PictureBox && (string)x.Tag == "BlockedWall")
                {
                    x.BringToFront();
                }
            }
            SetupTimers();
            SkillBar.BringToFront();
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
        private void ResetTimers()
        {
            if (gameTimer != null)
            {
                gameTimer.Stop();
                gameTimer.Tick -= GameLoop;
            }

            if (enemyShootTimer != null)
            {
                enemyShootTimer.Stop();
                enemyShootTimer.Tick -= EnemyShoot;
            }

            if (bulletMoveTimer != null)
            {
                bulletMoveTimer.Stop();
                bulletMoveTimer.Tick -= MoveBullets;
            }

            if (heartAnimationTimer != null)
            {
                heartAnimationTimer.Stop();
                heartAnimationTimer.Tick -= HeartAnimationTick;
            }

            SetupTimers();
            gameTimer.Start();
            enemyShootTimer.Start();
            bulletMoveTimer.Start();
            heartAnimationTimer.Start();
        }
        private bool checkSound = true;
        private void GameLoop(object sender, EventArgs e)
        {
            if (player.isDead && !retryFormCheck)
            {
                retryFormCheck = true;
                RetryForm retryForm = new RetryForm(this);
                if (retryForm.ShowDialog() == DialogResult.Retry)
                {
                    retryFormCheck = false;
                    player.Respawn();
                    foreach (Control x in Controls)
                    {
                        if (x is Enemy gameEnemy)
                        {
                            gameEnemy.isActivate = false;
                            gameEnemy.slowDownFactor = 1;
                        }
                        else if (x is Player p)
                        {
                            p.slowDownFactor = 1;
                        }
                        else if (x is Bullet bullet)
                        {
                            bullet.slowDownFactor = 1;
                        }
                    }
                    HeartState.Hearts = 5;
                    ResetTimers();
                    enemyShootTimer.Stop();
                    CreateHeartBoxes();
                    if (this.Controls.Contains(player))
                    {
                        this.Controls.Remove(player);
                        this.Controls.Remove(player.HitBox);
                    }
                    player = new Player();
                    player.SizeMode = PictureBoxSizeMode.CenterImage;
                    player.Location = new Point(0, 381);
                    player.IsOnGround = false;
                    player.BackColor = Color.Transparent;
                    player.CreateHitBox();
                    this.Controls.Add(player);

                    if (enemy.IsDead)
                    {
                        if (this.Controls.Contains(enemy))
                        {
                            this.Controls.Remove(enemy);
                        }
                        enemy = new RangedEnemy();
                        enemy.Location = new Point(1500, 325);
                        enemy.SizeMode = PictureBoxSizeMode.CenterImage;
                        this.Controls.Add(enemy);
                        enemy.BringToFront();
                    }
                    else
                    {
                        enemy.Location = new Point(1500, 325);
                    }

                    if (enemy2.IsDead)
                    {
                        if (this.Controls.Contains(enemy2))
                        {
                            this.Controls.Remove(enemy2);
                            this.Controls.Remove(enemy2.hitBox);
                        }

                        enemy2 = new MeleeEnemy();
                        enemy2.Location = new Point(608, 325);
                        enemy2.SizeMode = PictureBoxSizeMode.CenterImage;
                        enemy2.CreateHitBox();
                        this.Controls.Add(enemy2);
                        enemy2.BringToFront();
                    }
                    else
                    {
                        enemy2.Location = new Point(608, 325);
                    }
                    foreach (Control x in this.Controls)
                    {
                        if (x is PictureBox && (string)x.Tag == "Ground")
                        {
                            x.BringToFront();
                        }
                    }
                }
            }
            if (checkSound && SkillBar.Value <= 0)
            {
                checkSound = false;
                skillsounds[3].Play();
            }
            if (slowTime && SkillBar.Value > 0)
            {
                fadeAmount = Math.Min(fadeAmount + 0.1f, 0.85f);
                SkillBar.Value = Math.Max(0, SkillBar.Value - 2);
                foreach (Control x in Controls)
                {
                    if (x is RangedEnemy rangedEnemy)
                    {
                        rangedEnemy.slowDownFactor = 0.5;
                    }
                    else if (x is MeleeEnemy meleeEnemy)
                    {
                        meleeEnemy.slowDownFactor = 0.5;
                    }
                    else if (x is Bullet bullet)
                    {
                        bullet.slowDownFactor = 0.5;
                    }
                }
            }
            else
            {
                fadeAmount = Math.Max(fadeAmount - 0.1f, 0f);
                slowTime = false;
                SkillBar.Value = Math.Min(100, SkillBar.Value + 1);
            }
            this.Invalidate();

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
            UpdatePlayerHealthWithMeleeEnemyAttack();   

            if (player.Location.Y > this.Height && !Scene3OP)
            {
                this.Hide();
                Scene3OP = true;
                currentHealth = player.Health;
                Scene3 scene3 = new Scene3(currentHealth, Main.HeartState.Hearts);
                scene3.Show();
                scene3.Focus();
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (var semiTransparentBrush = new SolidBrush(Color.FromArgb((int)(fadeAmount * 255), Color.Black)))
            {
                e.Graphics.FillRectangle(semiTransparentBrush, this.ClientRectangle);
            }
        }
        private bool flag;
        private void UpdatePlayerHealthWithMeleeEnemyAttack()
        {
            foreach (Control x in Controls)
            {
                if (x is MeleeEnemy meleeEnemy)
                {
                    if (player.HitBox.Bounds.IntersectsWith(meleeEnemy.hitBox.Bounds))
                    {
                        if (meleeEnemy.attackFrame == meleeEnemy.attackFrameSize - 1 && flag)
                        {
                            player.TakeDamage(20);
                            RemoveHeart();
                            flag = false;
                        }
                    }
                }
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
                foreach (Control x in this.Controls)
                {
                    if (x is Enemy gameEnemy)
                    {
                        gameEnemy.isActivate = false;
                        enemyShootTimer.Stop();
                        gameEnemy.Patrol();
                    }
                }
                return;
            }

            foreach (Control x in this.Controls)
            {
                if (x is Enemy gameEnemy)
                {
                    if (x is RangedEnemy rangedEnemey)
                    {
                        if (!rangedEnemey.isActivate)
                        {
                            rangedEnemey.Patrol();
                            if (rangedEnemey.IsPlayerInSight(player.HitBox.Location))
                            {
                                enemyShootTimer.Start();
                                rangedEnemey.isActivate = true;
                            }
                        }

                        if (rangedEnemey.isActivate && !rangedEnemey.IsDead)
                        {
                            bool isRight = rangedEnemey.Left > player.Left;
                            if (rangedEnemey.IsPlayerInSight(player.HitBox.Location))
                            {
                                if (rangedEnemey.isFirst)
                                {
                                    rangedEnemey.isFirst = false;
                                    rangedEnemey.Image = Image.FromFile(isRight ? shootLeft[0] : shootRight[0]);
                                }
                                enemyShootTimer.Start();
                                rangedEnemey.isActivate = true;
                                rangedEnemey.isRunning = false;
                            }
                            else
                            {
                                rangedEnemey.isFirst = true;
                                enemyShootTimer.Stop();
                                rangedEnemey.isRunning = true;
                                rangedEnemey.ChasePlayer(player.HitBox.Location);
                            }
                        }
                    }
                    else if (x is MeleeEnemy meleeEnemy)
                    {
                        if (!meleeEnemy.isActivate && !meleeEnemy.isAttacking && !player.isDead)
                        {
                            meleeEnemy.Patrol();
                            if (meleeEnemy.IsPlayerInSight(player.HitBox.Location))
                            {
                                meleeEnemy.isActivate = true;
                            }
                        }

                        if (meleeEnemy.isActivate && !meleeEnemy.IsDead)
                        {
                            if (meleeEnemy.Bounds.IntersectsWith(player.HitBox.Bounds))
                            {
                                if (!player.isDead && !meleeEnemy.isAttacking)
                                {
                                    meleeEnemy.isRunning = false;
                                    meleeEnemy.Attack(player.Location);
                                    meleeEnemy.isAttacking = true;
                                    flag = true;
                                }
                            }
                            else
                            {
                                meleeEnemy.isActivate = true;
                                meleeEnemy.isRunning = true;
                                meleeEnemy.ChasePlayer(player.HitBox.Location);
                                meleeEnemy.isAttacking = false;
                            }
                        }
                    }
                    gameEnemy.UpdateEnemyAnimation(player.HitBox.Location);
                }
            }
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
                    }
                }
                bool flag = false;
                foreach (Control y in this.Controls)
                {
                    if (y is PictureBox && (string)y.Tag == "Ground")
                    {
                        if (player.HitBox.Bounds.IntersectsWith(y.Bounds) && player.Bottom >= y.Bottom - 50)
                        {
                            flag = true;
                            player.IsFalling = false;
                            player.IsJumping = false;
                            player.IsOnGround = true;
                            player.Top = y.Top - player.Height + 7;
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
        private bool isSlowActive = false;
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
            if (e.KeyCode == Keys.L)
            {
                player.Dash();
            }
            if (e.KeyCode == Keys.K && SkillBar.Value > 0 && !isSlowActive)
            {
                checkSound = true;
                slowTime = true;
                isSlowActive = true;
                skillsounds[2].Play();
            }
            if (e.KeyCode == Keys.J)
            {
                if ((DateTime.Now - lastAttackTime).TotalMilliseconds >= attackCooldown)
                {
                    player.IsAttacking = true;
                    lastAttackTime = DateTime.Now;
                }
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
            if (e.KeyCode == Keys.K)
            {
                if (checkSound)
                {
                    skillsounds[3].Play();
                }
                slowTime = false;
                isSlowActive = false;
                foreach (Control x in Controls)
                {
                    if (x is Player p)
                    {
                        p.slowDownFactor = 1;
                    }
                    else if (x is Enemy gameEnemy)
                    {
                        gameEnemy.slowDownFactor = 1;
                    }
                    else if (x is Bullet bullet)
                    {
                        bullet.slowDownFactor = 1;
                    }
                }
            }
        }
        
        private void CreateHeartBoxes()
        {
            heartImages = Directory.GetFiles("HearTile", "*.png").Select(img => Image.FromFile(img)).ToArray();
            heartBoxes = new List<PictureBox>();

            for (int i = 0; i < Main.HeartState.Hearts; i++)
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

        public void RemoveHeart()
        {
            if (heartBoxes.Count > 0)
            {
                var heartToRemove = heartBoxes[heartBoxes.Count - 1];
                heartBoxes.RemoveAt(heartBoxes.Count - 1);
                this.Controls.Remove(heartToRemove);
                Main.HeartState.Hearts--;
            }
        }

        private void LoadSoundEffect()
        {
            skillsounds = new List<SoundPlayer>();
            string[] soundFiles = Directory.GetFiles("AttackSound", "*.wav");
            foreach (string soundFile in soundFiles)
            {
                if (File.Exists(soundFile))
                {
                    SoundPlayer playerSound = new SoundPlayer(soundFile);
                    playerSound.Load();
                    skillsounds.Add(playerSound);
                }
            }
        }
    }
}