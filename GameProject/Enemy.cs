using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameProject
{
    public class Enemy : PictureBox
    {
        private List<string> idleLeft;
        private List<string> idleRight;
        private List<string> shootLeft;
        private List<string> shootRight;
        private List<string> runningRight;
        private List<string> runningLeft;
        private List<string> deathLeft;
        private List<string> deathRight;
        private List<string> hurtLeft;
        private List<string> hurtRight;
        private List<Bullet> bullets;

        private int idleFrame;
        private int shootFrame;
        private int runningFrame;
        private bool isShootRight;
        private bool isShooting = false;
        private Timer ShootAnimationTimer;
        private Direction currentDirection;
        private Timer deathAnimationTimer;
        private int deathAnimationFrame;


        private const int DetectionDistance = 600;
        private const int AttackHeightTolerance = 70; 
        private const int ChaseSpeed = 8;
        public bool IsDead { get; private set; } = false;
        public bool isActivate = false;
        public bool isRunning = false;
        public bool IsOnGround = false;
        public int Gravity = 10;
        public int health = 10;
        public Enemy()
        {
            LoadAnimations();
            InitializeProperties();
            bullets = new List<Bullet>();
        }
        private void LoadAnimations()
        {
            idleLeft = Directory.GetFiles("EnemyIdle_Left1", "*.png").ToList();
            idleRight = Directory.GetFiles("EnemyIdle_Right1", "*.png").ToList();
            shootLeft = Directory.GetFiles("EnemyShot_Left", "*.png").ToList();
            shootRight = Directory.GetFiles("EnemyShot_Right", "*.png").ToList();
            runningRight = Directory.GetFiles("EnemyRunning_Right", "*.png").ToList();
            runningLeft = Directory.GetFiles("EnemyRunning_Left", "*.png").ToList();
            deathLeft = Directory.GetFiles("EnemyDeath_Left", "*.png").ToList();
            deathRight = Directory.GetFiles("EnemyDeath_Right", "*.png").ToList();
            hurtLeft = Directory.GetFiles("EnemyHurt_Left", "*.png").ToList();
            hurtRight = Directory.GetFiles("EnemyHurt_Right", "*.png").ToList();
        }
        private void InitializeProperties()
        {
            this.Size = new Size(100, 100);
            this.BackColor = Color.Transparent;
            this.BorderStyle = BorderStyle.FixedSingle;

            ShootAnimationTimer = new Timer();
            ShootAnimationTimer.Interval = 20;
            ShootAnimationTimer.Tick += UpdateShootAnimation;

            deathAnimationTimer = new Timer();
            deathAnimationTimer.Interval = 150;
            deathAnimationTimer.Tick += UpdateDeathAnimation;
        }
        private int enemyIdleAnimationDelayCounter = 0;
        public void UpdateIdleAnimation(Point playerPosition)
        {
            ++enemyIdleAnimationDelayCounter;
            if(enemyIdleAnimationDelayCounter == 3)
            {
                bool isRight = this.Left > playerPosition.X;
                if (!isShooting)
                {
                    if (idleFrame >= idleLeft.Count) idleFrame = 0;
                    this.Image = Image.FromFile(isRight ? idleLeft[idleFrame++] : idleRight[idleFrame++]);
                }
                enemyIdleAnimationDelayCounter = 0;
            }
        }
        public void ApplyGravity()
        {
            if (!IsOnGround)
            {
                this.Top += Gravity;
                if (this.Top + this.Height >= this.Parent.ClientSize.Height)
                {
                    IsOnGround = true;
                    this.Top = this.Parent.ClientSize.Height - this.Height;
                }
            }
        }
        private void UpdateShootAnimation(object sender, EventArgs e)
        {
            isShooting = true;
            if (shootFrame < shootRight.Count)
            {
                this.Image = Image.FromFile(isShootRight ? shootRight[shootFrame++] : shootLeft[shootFrame++]);
            }
            else
            {
                shootFrame = 0;
                ShootAnimationTimer.Stop();
                isShooting = false;
            }
        }
        public Bullet Shoot(Point playerPosition)
        {
            if (IsDead) return null;
            isShootRight = playerPosition.X > this.Left;
            shootFrame = 0;
            ShootAnimationTimer.Start();
            return new Bullet(isShootRight)
            {
                Location = new Point(this.Left, this.Top + this.Height / 3 + 3),
                Speed = 40,
                IsMovingRight = isShootRight
            };

        }
        public bool IsPlayerInSight(Point playerPosition)
        {
            bool withinHorizontalDistance = Math.Abs(playerPosition.X - this.Left) <= DetectionDistance;
            bool sameGround = Math.Abs(playerPosition.Y - this.Top) <= AttackHeightTolerance;
            return withinHorizontalDistance && sameGround;
        }
        public void ChasePlayer(Point playerPosition)
        {
            if (IsPlayerInSight(playerPosition))
            {
                return;
            }

            if (playerPosition.X < this.Left)
            {
                this.Left -= ChaseSpeed;
                currentDirection = Direction.Left;
            }
            else
            {
                this.Left += ChaseSpeed;
                currentDirection = Direction.Right;
            }
        }
        private int slowCounter = 0;
        public void UpdateEnemyAnimation(Point playerPosition)
        {
            if (deathAnimationTimer.Enabled)
            {
                return;
            }

            ++slowCounter;
            if (slowCounter == 1)
            {
                slowCounter = 0;
                bool isRight = this.Left > playerPosition.X;
                if (isRunning)
                {
                    UpdateEnemyRunningAnimation(isRight);
                }
                else
                {
                    UpdateIdleAnimation(playerPosition);
                }
            }
        }
        private void UpdateEnemyRunningAnimation(bool isRight)
        {
            if (runningFrame < runningRight.Count)
            {
                this.Image = Image.FromFile(isRight ? runningLeft[runningFrame++] : runningRight[runningFrame++]);
            }
            else
            {
                runningFrame = 0;
            }
        }
        public enum Direction
        {
            Left,
            Right
        }
        public void TakeDamage(int damage, Point playerPosition)
        {
            if (IsDead) return;
            health -= damage;

            if (health <= 0)
            {
                IsDead = true;
                Die();
            }
        }
        public void Die()
        {
            isShooting = false;
            isRunning = false;
            idleFrame = 0;
            runningFrame = 0;
            deathAnimationFrame = 0;
            
            deathAnimationTimer.Start();
            this.Enabled = false;
        }
        private void UpdateDeathAnimation(object sender, EventArgs e)
        {
            if (deathAnimationFrame < (currentDirection == Direction.Right ? deathRight.Count : deathLeft.Count))
            {
                this.Image = Image.FromFile(currentDirection == Direction.Right ? deathRight[deathAnimationFrame] : deathLeft[deathAnimationFrame]);
                deathAnimationFrame++;
            }
            else
            {
                deathAnimationTimer.Stop();
                this.Visible = false;
                this.Enabled = false;
            }
        }
    }
}
