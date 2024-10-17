using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GameProject
{
    public abstract class Enemy : PictureBox
    {
        protected List<string> idleRangedLeft;
        protected List<string> idleRangedRight;
        protected List<string> deathRangedLeft;
        protected List<string> deathRangedRight;
        protected List<string> runningRangedLeft;
        protected List<string> runningRangedRight;
        protected List<string> idleMeleeLeft;
        protected List<string> idleMeleeRight;
        protected List<string> runningMeleeLeft;
        protected List<string> runningMeleeRight;
        protected List<string> deathMeleeLeft;
        protected List<string> deathMeleeRight;
        protected int idleFrame;
        protected int runningFrame;
        protected Timer deathAnimationTimer;
        protected int deathAnimationFrame;
        protected Direction currentDirection;
        protected int enemyIdleAnimationDelayCounter = 0;
        protected int slowCounter = 0;
        private const int DetectionDistance = 600;
        private const int AttackHeightTolerance = 70;
        private const int ChaseSpeed = 8;

        public bool IsDead { get; protected set; } = false;
        public bool IsOnGround { get; set; } = false;
        public bool isActivate { get; set; } = false;
        public int health { get; protected set; }
        public int Gravity { get; protected set; } = 10;
        protected int speed = 2;
        public bool isRunning { get; set; } = false;

        public bool isAttacking { get; set; } = false;
        public Enemy()
        {
            InitializeProperties();
        }
        protected virtual void InitializeProperties()
        {
            deathAnimationTimer = new Timer();
            deathAnimationTimer.Interval = 150;
            deathAnimationTimer.Tick += UpdateDeathAnimation;
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
        public abstract void Attack(Point playerPosition);
        public abstract void UpdateEnemyAnimation(Point playerPosition);
        public abstract void UpdateDeathAnimation(object sender, EventArgs e);
        public abstract void UpdateEnemyRunningAnimation(bool isRight);
        public virtual Bullet Shoot(Point playerPosition)
        {
            return null;
        }
        public virtual void ApplyGravity()
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
        public virtual void TakeDamage(int damage, Point playerPosition)
        {
            if (IsDead) return;
            health -= damage;
            currentDirection = playerPosition.X > this.Left ? Direction.Right : Direction.Left;
            if (health <= 0)
            {
                IsDead = true;
                Die();
            }
        }
        public virtual void Die()
        {
            deathAnimationFrame = 0;
            deathAnimationTimer.Start();
            this.Enabled = false;
        }
        public enum Direction
        {
            Left,
            Right
        }
    }
    public class RangedEnemy : Enemy
    {
        private List<string> shootLeft;
        private List<string> shootRight;
        private int shootFrame;
        private Timer shootAnimationTimer;
        private bool isShooting = false;
        private bool isShootRight;

        public RangedEnemy()
        {
            this.Size = new Size(50, 70);
            this.BackColor = Color.Transparent;
            this.BorderStyle = BorderStyle.FixedSingle;

            LoadAnimations();
            InitializeProperties();
        }

        protected override void InitializeProperties()
        {
            base.InitializeProperties();

            shootAnimationTimer = new Timer();
            shootAnimationTimer.Interval = 20;
            shootAnimationTimer.Tick += UpdateShootAnimation;
        }
        private void LoadAnimations()
        {
            idleRangedLeft = Directory.GetFiles("EnemyIdle_Left1", "*.png").ToList();
            idleRangedRight = Directory.GetFiles("EnemyIdle_Right1", "*.png").ToList();
            shootLeft = Directory.GetFiles("EnemyShot_Left", "*.png").ToList();
            shootRight = Directory.GetFiles("EnemyShot_Right", "*.png").ToList();
            runningRangedRight = Directory.GetFiles("EnemyRunning_Right", "*.png").ToList();
            runningRangedLeft = Directory.GetFiles("EnemyRunning_Left", "*.png").ToList();
            deathRangedLeft = Directory.GetFiles("EnemyDeath_Left", "*.png").ToList();
            deathRangedRight = Directory.GetFiles("EnemyDeath_Right", "*.png").ToList();
        }
        public override void Attack(Point playerPosition)
        {
            if (IsDead) return;
            isShootRight = playerPosition.X + 10 > this.Left;
            shootFrame = 0;
            shootAnimationTimer.Start();
        }
        public override Bullet Shoot(Point playerPosition)
        {
            if (IsDead) return null;
            isShootRight = playerPosition.X > this.Left;
            shootFrame = 0;
            shootAnimationTimer.Start();
            return new Bullet(isShootRight)
            {
                Location = new Point(this.Left, this.Top + this.Height / 3 + 3),
                Speed = 40,
                IsMovingRight = isShootRight
            };
        }
        public override void UpdateEnemyRunningAnimation(bool isRight)
        {
            if (runningFrame < runningRangedRight.Count)
            {
                this.Image = Image.FromFile(isRight ? runningRangedLeft[runningFrame++] : runningRangedRight[runningFrame++]);
            }
            else
            {
                runningFrame = 0;
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
                shootAnimationTimer.Stop();
                isShooting = false;
            }
        }
        public override void UpdateEnemyAnimation(Point playerPosition)
        {
            if (IsDead)
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
        private void UpdateIdleAnimation(Point playerPosition)
        {
            ++enemyIdleAnimationDelayCounter;
            if (enemyIdleAnimationDelayCounter == 3)
            {
                bool isRight = this.Left > playerPosition.X;
                if (!isShooting)
                {
                    if (idleFrame >= idleRangedLeft.Count) idleFrame = 0;
                    this.Image = Image.FromFile(isRight ? idleRangedLeft[idleFrame++] : idleRangedRight[idleFrame++]);
                }
                enemyIdleAnimationDelayCounter = 0;
            }
        }
        private bool isFirst = true;
        public override void UpdateDeathAnimation(object sender, EventArgs e)
        {
            if (deathAnimationFrame < (currentDirection == Direction.Right ? deathRangedRight.Count : deathRangedLeft.Count))
            {
                this.Image = Image.FromFile(currentDirection == Direction.Right ? deathRangedRight[deathAnimationFrame] : deathRangedLeft[deathAnimationFrame]);
                deathAnimationFrame++;
            }
            else
            {
                deathAnimationTimer.Stop();
                this.Visible = false;
                this.Enabled = false;
                this.Dispose();
            }
        }
    }

    public class MeleeEnemy : Enemy
    {
        private List<string> attackLeft;
        private List<string> attackRight;
        private int attackFrame;
        private Timer attackAnimationTimer;
        private bool isAttackRight;
        private const int AttackRange = 20;

        public MeleeEnemy()
        {
            this.Size = new Size(50, 150);
            this.BackColor = Color.Transparent;
            this.BorderStyle = BorderStyle.FixedSingle;

            LoadAnimations();
            InitializeProperties();
            this.Image = Image.FromFile(idleMeleeLeft[0]);
        }

        protected override void InitializeProperties()
        {
            base.InitializeProperties();

            attackAnimationTimer = new Timer();
            attackAnimationTimer.Interval = 60;
            attackAnimationTimer.Tick += (s, e) => UpdateAttackAnimation();
        }

        private void LoadAnimations()
        {
            idleMeleeLeft = Directory.GetFiles("Enemy2Idle_Left", "*.png").ToList();
            idleMeleeRight = Directory.GetFiles("Enemy2Idle_Right", "*.png").ToList();
            runningMeleeLeft = Directory.GetFiles("Enemy2Running_Left", "*.png").ToList();
            runningMeleeRight = Directory.GetFiles("Enemy2Running_Right", "*.png").ToList();
            deathMeleeLeft = Directory.GetFiles("Enemy2Death_Left", "*.png").ToList();
            deathMeleeRight = Directory.GetFiles("Enemy2Death_Right", "*.png").ToList();
            attackLeft = Directory.GetFiles("Enemy2Attack_Left", "*.png").ToList();
            attackRight = Directory.GetFiles("Enemy2Attack_Right", "*.png").ToList();
        }
        public override void Attack(Point playerPosition)
        {
            if (IsDead) return;

            isAttackRight = playerPosition.X + 10 > this.Left;
            attackFrame = 0;
            isAttacking = true;
            attackAnimationTimer.Start();
        }
        public override void UpdateEnemyRunningAnimation(bool isRight)
        {
            if (runningFrame < runningMeleeLeft.Count)
            {
                this.Image = Image.FromFile(isRight ? runningMeleeLeft[runningFrame++] : runningMeleeRight[runningFrame++]);
            }
            else
            {
                runningFrame = 0;
            }
        }
        private void UpdateAttackAnimation()
        {
            if (isAttacking)
            {
                if (attackFrame < (isAttackRight ? attackRight.Count : attackLeft.Count))
                {
                    this.Image = Image.FromFile(isAttackRight ? attackRight[attackFrame] : attackLeft[attackFrame]);
                    attackFrame++;
                }
                else
                {
                    attackAnimationTimer.Stop();
                    isAttacking = false;
                    attackFrame = 0; 
                }
            }
        }
        public override void UpdateEnemyAnimation(Point playerPosition)
        {
            if (IsDead)
            {
                return;
            }
            ++slowCounter;
            if (slowCounter == 1)
            {
                slowCounter = 0;
                if (IsPlayerInSight(playerPosition) && Math.Abs(playerPosition.X - this.Left) <= AttackRange)
                {
                    if (!isAttacking)
                    {
                        Attack(playerPosition);
                    }
                    else
                    {
                        UpdateAttackAnimation();
                    }
                }
                else
                {
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
        }
        private void UpdateIdleAnimation(Point playerPosition)
        {
            ++enemyIdleAnimationDelayCounter;
            if (enemyIdleAnimationDelayCounter >= 3)
            {
                bool isRight = this.Left > playerPosition.X;
                if (!isAttacking)
                {
                    if (idleFrame >= idleMeleeLeft.Count) idleFrame = 0;
                    this.Image = Image.FromFile(isRight ? idleMeleeLeft[idleFrame++] : idleMeleeRight[idleFrame++]);
                }
                enemyIdleAnimationDelayCounter = 0;
            }
        }
        private bool isFirst = true;
        public override void UpdateDeathAnimation(object sender, EventArgs e)
        {
            if (deathAnimationFrame < (currentDirection == Direction.Right ? deathMeleeRight.Count : deathMeleeLeft.Count))
            {
                this.Image = Image.FromFile(currentDirection == Direction.Right ? deathMeleeRight[deathAnimationFrame] : deathMeleeLeft[deathAnimationFrame]);
                deathAnimationFrame++;
            }
            else
            {
                deathAnimationTimer.Stop();
                this.Visible = false;
                this.Enabled = false;
                this.Dispose();
            }
        }
    }

}
