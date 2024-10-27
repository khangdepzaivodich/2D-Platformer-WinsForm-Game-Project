using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
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
        protected List<string> walkingMeleeLeft;
        protected List<string> walkingMeleeRight;
        protected List<string> walkingRangedRight;
        protected List<string> walkingRangedLeft;
        protected List<SoundPlayer> soundsfx;
        protected int idleFrame;
        protected int runningFrame;
        protected int walkingFrame;
        protected Timer deathAnimationTimer;
        public Point InitPos;
        private List<string> bloodEffect;
        private Timer bloodEffectTimer;
        private PictureBox bloodEffectBox;
        private int bloodEffectFrame = 0;
        protected int deathAnimationFrame;
        private bool isBloodEffectRunning = false;
        protected Direction currentDirection;
        protected int enemyIdleAnimationDelayCounter = 0;
        protected int slowCounter = 0;
        protected const int DetectionDistance = 600;
        protected const int AttackHeightTolerance = 80;
        protected const int ChaseSpeed = 8;
        public PictureBox hitBox;
        public int AttackRange = 20;
        public bool IsDead { get; set; } = false;
        public bool IsOnGround { get; set; } = false;
        public bool isActivate { get; set; } = false;
        public int health { get; protected set; }
        public int Gravity { get; protected set; } = 10;
        public int walkDelayCounter = 0;
        protected int speed = 2;
        public bool isRunning { get; set; } = false;
        public double slowDownFactor = 1;
        public bool isAttacking { get; set; } = false;
        public bool isColliding { get; set; } = false;

        public bool isFirst = true;
        public bool isMovingRight = false;
        public Enemy()
        {
            InitializeProperties();
            LoadSoundEffects();
        }
        protected virtual void InitializeProperties()
        {
            bloodEffect = Directory.GetFiles("Blood", "*.png").ToList();
            deathAnimationTimer = new Timer();
            deathAnimationTimer.Interval = 150;
            deathAnimationTimer.Tick += UpdateDeathAnimation;

            bloodEffectTimer = new Timer();
            bloodEffectTimer.Interval = 1;
            bloodEffectTimer.Tick += UpdateBloodEffect;

            bloodEffectBox = new PictureBox
            {
                Size = new Size(50, 50),
                SizeMode = PictureBoxSizeMode.CenterImage,
                BackColor = Color.Transparent
            };
            this.Controls.Add(bloodEffectBox);
        }
        public virtual bool IsPlayerInSight(Point playerPosition)
        {
            bool withinHorizontalDistance = Math.Abs(playerPosition.X - this.Left) <= DetectionDistance;
            bool sameGround = Math.Abs(playerPosition.Y - this.Top) <= AttackHeightTolerance;
            return withinHorizontalDistance && sameGround;
        }
        public virtual void ChasePlayer(Point playerPosition)
        {
            if (IsPlayerInSight(playerPosition))
            {
                return;
            }
            bool flag = false;
            if (playerPosition.X < this.Left)
            {
                if (this.Parent != null)
                {
                    foreach (Control x in this.Parent.Controls)
                    {
                        if (x is PictureBox && (string)x.Tag == "InvisibleWall" && this.Bounds.IntersectsWith(x.Bounds) && this.Left > x.Left)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        this.Left -= (int)(ChaseSpeed * slowDownFactor);
                        currentDirection = Direction.Left;
                    }
                }
            }
            else
            {
                flag = false;
                foreach (Control x in this.Parent.Controls)
                {
                    if (x is PictureBox && (string)x.Tag == "InvisibleWall" && this.Bounds.IntersectsWith(x.Bounds) && this.Left < x.Left)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    this.Left += ChaseSpeed;
                    currentDirection = Direction.Right;
                }
            }
        }
        public void CreateHitBox()
        {
            hitBox = new PictureBox();
            hitBox.BackColor = Color.Transparent;
            hitBox.Size = new Size(50, 70);
            if (this.Parent != null)
            {
                this.Parent.Controls.Add(hitBox);
            }
        }
        private void LoadSoundEffects()
        {
            soundsfx = new List<SoundPlayer>();
            string[] soundFiles = Directory.GetFiles("AttackSound", "*.wav");
            foreach (string soundFile in soundFiles)
            {
                if (File.Exists(soundFile))
                {
                    SoundPlayer playerSound = new SoundPlayer(soundFile);
                    playerSound.Load();
                    soundsfx.Add(playerSound);
                }
            }
        }
        public void UpdateHitboxPosition()
        {
            hitBox.Left = this.Left + (this.Width / 2) - (hitBox.Width / 2);
            hitBox.Top = this.Top + (this.Height / 2) - 10;
        }
        public abstract void Attack(Point playerPosition);
        public abstract void UpdateEnemyAnimation(Point playerPosition);
        public abstract void UpdateDeathAnimation(object sender, EventArgs e);
        public abstract void UpdateEnemyRunningAnimation(bool isRight);

        public abstract void Patrol();
        public virtual Bullet Shoot(Point playerPosition)
        {
            return null;
        }
        public virtual void ApplyGravity()
        {
            if (this.Parent == null) return;
            if (!IsOnGround && !IsDead)
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
            bloodEffectFrame = 0;
            isBloodEffectRunning = true;
            currentDirection = playerPosition.X > this.Left ? Direction.Right : Direction.Left;
            UpdateBloodEffect(this, EventArgs.Empty);
            bloodEffectTimer.Start();
            if (health <= 0)
            {
                IsDead = true;
                Die();
            }
        }
        private void UpdateBloodEffect(object sender, EventArgs e)
        {
            if (bloodEffectFrame >= bloodEffect.Count)
            {
                bloodEffectFrame = 0;
                bloodEffectTimer.Stop();
                bloodEffectBox.Visible = false;
                isBloodEffectRunning = false;
            }
            else
            {
                bloodEffectBox.Image = Image.FromFile(bloodEffect[bloodEffectFrame++]);
                bloodEffectBox.Location = new Point((this.Width / 2) - (bloodEffectBox.Width / 2), (this.Height / 2) - (bloodEffectBox.Height / 2));
                bloodEffectBox.BringToFront();
                bloodEffectBox.Visible = true;
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
        private Timer idleAnimationTimer;

        private const int PatrolDistance = 100;
        private bool isIdling = false;
        private int idleDuration = 120;
        private int idleCounter = 0;
        private int totalMoved = 0;
        

        public RangedEnemy()
        {
            this.Size = new Size(50, 70);
            this.BackColor = Color.Transparent;
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
            walkingRangedLeft = Directory.GetFiles("EnemyWalk_Left", "*.png").ToList();
            walkingRangedRight = Directory.GetFiles("EnemyWalk_Right", "*.png").ToList();
        }
        public override void Attack(Point playerPosition)
        {
            if (IsDead) return;
            soundsfx[4].Play();
            isShootRight = playerPosition.X + 10 > this.Left;
            shootFrame = 0;
            shootAnimationTimer.Start();
        }

        public override void Patrol()
        {
            if (isShooting) return;
            if (isIdling)
            {
                UpdateIdleAnimation();
                return;
            }
            int patrolSpeed = 2;

            if (isMovingRight)
            {
                this.Left += (int) (patrolSpeed * slowDownFactor);
                totalMoved += (int)(patrolSpeed * slowDownFactor);

                if (totalMoved >= PatrolDistance)
                {
                    isIdling = true;
                    totalMoved = 0;
                }
            }
            else
            {
                this.Left -= (int)(patrolSpeed * slowDownFactor);
                totalMoved += (int)(patrolSpeed * slowDownFactor);

                if (totalMoved >= PatrolDistance)
                {
                    isIdling = true;
                    totalMoved = 0;
                }
            }
            ++walkDelayCounter;
            if(walkDelayCounter >= (int) 2 / slowDownFactor)
            {
                walkDelayCounter = 0;
                if (runningFrame >= runningRangedRight.Count)
                {
                    runningFrame = 0;
                }
                this.Image = Image.FromFile(isMovingRight ? runningRangedRight[runningFrame++] : runningRangedLeft[runningFrame++]);
            }
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
        private int runningDelayCounter = 0;
        public override void UpdateEnemyRunningAnimation(bool isRight)
        {
            ++runningDelayCounter;
            if(runningDelayCounter >= (int) 2 / slowDownFactor)
            {
                runningDelayCounter = 0;
                if (runningFrame < runningRangedRight.Count)
                {
                    this.Image = Image.FromFile(isRight ? runningRangedLeft[runningFrame++] : runningRangedRight[runningFrame++]);
                }
                else
                {
                    runningFrame = 0;
                }
            }
            
        }
        private bool checkShoot = true;
        private void UpdateShootAnimation(object sender, EventArgs e)
        {
            isShooting = true;
            if (shootFrame < shootRight.Count && checkShoot)
            {
                soundsfx[4].Play();
                checkShoot = false;
                this.Image = Image.FromFile(isShootRight ? shootRight[shootFrame++] : shootLeft[shootFrame++]);
            }
            else
            {
                checkShoot = true;
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
            if (slowCounter >= 1)
            {
                slowCounter = 0;
                bool isRight = this.Left > playerPosition.X;
                if (isRunning)
                {
                    UpdateEnemyRunningAnimation(isRight);
                }
                else
                {
                    UpdateIdleAnimation();
                }
            }
        }
        private void UpdateIdleAnimation()
        {
            if (isIdling)
            {
                idleCounter++;
                if (idleCounter >= idleDuration)
                {
                    isIdling = false;
                    idleCounter = 0;
                    isMovingRight = !isMovingRight;
                }
                else
                {
                    ++enemyIdleAnimationDelayCounter;
                    if (enemyIdleAnimationDelayCounter >= (int) (5 / slowDownFactor))
                    {
                        bool isRight = isMovingRight;
                        if (!isShooting)
                        {
                            if (idleFrame >= (isRight ? idleRangedRight.Count : idleRangedLeft.Count)) idleFrame = 0;
                            this.Image = Image.FromFile(isRight ? idleRangedRight[idleFrame++] : idleRangedLeft[idleFrame++]);
                        }
                        enemyIdleAnimationDelayCounter = 0;
                    }
                }
            }
        }
        
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
        public int attackFrame;
        private Timer attackAnimationTimer;
        private bool isAttackRight;
        private const int AttackRanges = 20;
        private const int PatrolDistance = 100;
        private bool isIdling = false;
        private int idleDuration = 120;
        private int idleCounter = 0;
        private int totalMoved = 0;
        public int attackFrameSize;
        public MeleeEnemy()
        {
            this.Size = new Size(50, 150);
            this.BackColor = Color.Transparent;

            LoadAnimations();
            attackFrameSize = attackLeft.Count;
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
        public override bool IsPlayerInSight(Point playerPosition)
        {
            bool withinHorizontalDistance = Math.Abs(playerPosition.X - this.hitBox.Left) <= DetectionDistance;
            bool sameGround = Math.Abs(playerPosition.Y - this.hitBox.Top) <= AttackHeightTolerance;
            return withinHorizontalDistance && sameGround;
        }
        public override void ChasePlayer(Point playerPosition)
        {
            if (playerPosition.X < this.Left)
            {
                this.Left -= (int)(ChaseSpeed * slowDownFactor);
                currentDirection = Direction.Left;
            }
            else
            {
                this.Left += (int)(ChaseSpeed * slowDownFactor);
                currentDirection = Direction.Right;
            }
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
            walkingMeleeLeft = Directory.GetFiles("Enemy2Walk_Left", "*.png").ToList();
            walkingMeleeRight = Directory.GetFiles("Enemy2Walk_Right", "*.png").ToList();
        }
        public override void Attack(Point playerPosition)
        {
            if (IsDead) return;

            isAttackRight = playerPosition.X + 10 > this.Left;
            attackFrame = 0;
            isAttacking = true;
            attackAnimationTimer.Start();
        }
        private int meleeWalkingDelayCounter = 0;
        public override void Patrol()
        {
            if (isColliding) return;
            if (isIdling)
            {
                UpdateIdleAnimation();
                return;
            }
            int patrolSpeed = 2;

            if (isMovingRight)
            {
                this.Left += (int)(patrolSpeed * slowDownFactor);
                totalMoved += (int)(patrolSpeed * slowDownFactor);

                if (totalMoved >= PatrolDistance)
                {
                    isIdling = true;
                    totalMoved = 0;
                }
            }
            else
            {
                this.Left -= (int)(patrolSpeed * slowDownFactor);
                totalMoved += (int)(patrolSpeed * slowDownFactor);

                if (totalMoved >= PatrolDistance)
                {
                    isIdling = true;
                    totalMoved = 0;
                }
            }
            ++meleeWalkingDelayCounter;
            if(meleeWalkingDelayCounter >= (int)(2 / slowDownFactor))
            {
                meleeWalkingDelayCounter = 0;
                if (walkingFrame >= walkingMeleeRight.Count)
                {
                    walkingFrame = 0;
                }
                this.Image = Image.FromFile(isMovingRight ? walkingMeleeRight[walkingFrame++] : walkingMeleeLeft[walkingFrame++]);
            }
            
        }
        private int meleeRunningDelayCounter = 0;
        public override void UpdateEnemyRunningAnimation(bool isRight)
        {
            ++meleeRunningDelayCounter;
            if(meleeRunningDelayCounter >= (int)(2 / slowDownFactor))
            {
                meleeRunningDelayCounter = 0;
                if (runningFrame < runningMeleeLeft.Count)
                {
                    this.Image = Image.FromFile(isRight ? runningMeleeLeft[runningFrame++] : runningMeleeRight[runningFrame++]);
                }
                else
                {
                    runningFrame = 0;
                }
            }
        }
        private int meleeAttackDelayCounter = 0;
        private bool checkPunch = true;
        private void UpdateAttackAnimation()
        {
            ++meleeAttackDelayCounter;
            if(meleeAttackDelayCounter >= (int) (2 / slowDownFactor))
            {
                meleeAttackDelayCounter = 0;
                if (isAttacking)
                {
                    if (attackFrame < (isAttackRight ? attackRight.Count : attackLeft.Count))
                    {
                        if (checkPunch)
                        {
                            soundsfx[5].Play();
                        }
                        checkPunch = false;
                        this.Image = Image.FromFile(isAttackRight ? attackRight[attackFrame] : attackLeft[attackFrame]);
                        attackFrame++;
                    }
                    else
                    {
                        checkPunch = true;
                        attackAnimationTimer.Stop();
                        isAttacking = false;
                        attackFrame = 0;
                    }
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
                        UpdateIdleAnimation();
                    }
                }
            }
        }
        private void UpdateIdleAnimation()
        {
            if (isColliding) return;
            if (isIdling)
            {
                idleCounter++;
                if (idleCounter >= idleDuration)
                {
                    isIdling = false;
                    idleCounter = 0;
                    isMovingRight = !isMovingRight;
                }
                else
                {
                    ++enemyIdleAnimationDelayCounter;
                    if (enemyIdleAnimationDelayCounter >= (int)(5 / slowDownFactor))
                    {
                        bool isRight = isMovingRight;
                        if (!isAttacking)
                        {
                            if (idleFrame >= (isRight ? idleMeleeRight.Count : idleMeleeLeft.Count)) idleFrame = 0;
                            this.Image = Image.FromFile(isRight ? idleMeleeRight[idleFrame++] : idleMeleeLeft[idleFrame++]);
                        }
                        enemyIdleAnimationDelayCounter = 0;
                    }
                }
            }
        }
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
                this.hitBox.Dispose();
                this.Dispose();
            }
        }
    }

}
