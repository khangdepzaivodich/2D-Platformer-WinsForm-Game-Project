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
            
        private int idleFrame;
        private int shootFrame;
        private int runningFrame;
        private bool isShootRight;
        private bool isShooting = false;
        private Timer ShootAnimationTimer;

        private const int DetectionDistance = 600;
        private const int AttackHeightTolerance = 70; 
        private const int ChaseSpeed = 8;
        public bool isActivate = false;
        public bool isRunning = false;
        public Enemy()
        {
            LoadAnimations();
            InitializeProperties();
        }
        private void LoadAnimations()
        {
            idleLeft = Directory.GetFiles("EnemyIdle_Left1", "*.png").ToList();
            idleRight = Directory.GetFiles("EnemyIdle_Right1", "*.png").ToList();
            shootLeft = Directory.GetFiles("EnemyShot_Left", "*.png").ToList();
            shootRight = Directory.GetFiles("EnemyShot_Right", "*.png").ToList();
            runningRight = Directory.GetFiles("EnemyRunning_Right", "*.png").ToList();
            runningLeft = Directory.GetFiles("EnemyRunning_Left", "*.png").ToList();
        }
        private void InitializeProperties()
        {
            this.Size = new Size(60, 70);
            this.BackColor = Color.Transparent;
            ShootAnimationTimer = new Timer();
            ShootAnimationTimer.Interval = 20;
            ShootAnimationTimer.Tick += UpdateShootAnimation;
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
            }
            else
            {
                this.Left += ChaseSpeed;
            }
        }
        private int slowCounter = 0;
        public void UpdateEnemyAnimation(Point playerPosition)
        {
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
    }
}
