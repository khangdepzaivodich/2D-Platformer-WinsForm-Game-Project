using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameProject
{
    public class Player : PictureBox
    {
        public bool IsLeft { get; set; }
        public bool IsRight { get; set; }
        public bool IsFlipped { get; set; }
        public bool IsRunning { get; set; }
        public bool IsOnGround { get; set; }
        public bool IsJumping { get; set; }
        public bool IsFalling = false;
        public bool startFalling = true;
        private bool isDashing = false;
        private bool canDash = true;
        private int dashDist = 10;
        private int dashDuration = 100;
        private int dashStep = 10;
        private int dashDir = 1;
        private int dashStepDist;
        private int dashCooldown = 3000;
        private Timer dashTimer;
        private Timer dashCooldownTimer;
        public int Speed { get; set; }
        public int Gravity { get; set; }
        public int JumpSpeed { get; set; }      
        public int Health { get; set; } = 100;
        public int AttackDamage { get; private set; } = 20;
        public bool isDead { get; set; } = false;

        public List<PictureBox> HeartBoxes { get; set; }

        public bool IsAttacking = false;
        private Timer attackCooldownTimer;

        private List<string> idleRight;
        private List<string> idleLeft;
        private List<string> runningRight;
        private List<string> runningLeft;
        private List<string> attackRight;
        private List<string> attackLeft;
        private List<string> jumpingRight;
        private List<string> jumpingLeft;
        private List<string> deathLeft;
        private List<string> deathRight;
        private List<string> takeHitLeft;
        private List<string> takeHitRight;
        private List<string> bloodEffect;
        private List<string> fallRight;
        private List<string> fallLeft;
        private List<SoundPlayer> soundsfx;

        private int idleFrame;
        private int runningFrame;
        private int attackFrame;
        private int deathFrame;
        private int takeHitFrame;
        private Main mainForm;
        private int bloodEffectFrame = 0;

        public double slowDownFactor = 1;

        private bool isBloodEffectRunning = false;
        private int deathFrameIndex = 0;
        private Timer deathAnimationTimer;
        private Timer bloodEffectTimer;
        private Timer runAnimationTimer;

        public PictureBox HitBox;
        private PictureBox bloodEffectBox;


        public Player()
        {
            LoadAnimation();
            InitializeProperties();
            LoadSoundEffect();

            dashStepDist = dashDist / dashStep;
            dashTimer = new Timer();
            dashTimer.Interval = dashDuration / dashStep;
            dashTimer.Tick += PerformDash;


            dashCooldownTimer = new Timer();
            dashCooldownTimer.Interval = dashCooldown;
            dashCooldownTimer.Tick += EndDashCooldown;


            deathAnimationTimer = new Timer();
            deathAnimationTimer.Interval = 200;
            deathAnimationTimer.Tick += UpdateDeathAnimation;

            

            bloodEffectTimer = new Timer();
            bloodEffectTimer.Interval = 1; //(ms)
            bloodEffectTimer.Tick += UpdateBloodEffect;

            bloodEffectBox = new PictureBox
            {
                Size = new Size(50, 50),
                SizeMode = PictureBoxSizeMode.CenterImage,
                BackColor = Color.Transparent
            };
            this.Controls.Add(bloodEffectBox);
        }


        public void CreateHitBox()
        {
            HitBox = new PictureBox();
            HitBox.BackColor = Color.Transparent;
            HitBox.Size = new Size(50, 70);
            HitBox.Visible = true;
            HitBox.BorderStyle = BorderStyle.FixedSingle;
            if (this.Parent != null)
            {
                this.Parent.Controls.Add(HitBox);
            }
        }

        public void UpdateHitboxPosition()
        {
            HitBox.Left = this.Left + (this.Width / 2) - (HitBox.Width / 2);
            HitBox.Top = this.Top + (this.Height / 2) - (HitBox.Height / 2);
        }

        private void LoadAnimation()
        {
            
            idleRight = Directory.GetFiles("Idle_Right", "*png").ToList();
            idleLeft = Directory.GetFiles("Idle_Left", "*.png").ToList();
            runningRight = Directory.GetFiles("Running_Right", "*.png").ToList();
            runningLeft = Directory.GetFiles("Running_Left", "*.png").ToList();
            attackRight = Directory.GetFiles("Attack_Right", "*.png").ToList();
            attackLeft = Directory.GetFiles("Attack_Left", "*.png").ToList();
            jumpingLeft = Directory.GetFiles("Jumping_Left", "*.png").ToList();
            jumpingRight = Directory.GetFiles("Jumping_Right", "*.png").ToList();
            deathLeft = Directory.GetFiles("PlayerDeath_Left", "*.png").ToList();
            deathRight = Directory.GetFiles("PlayerDeath_Right", "*.png").ToList();
            takeHitLeft = Directory.GetFiles("TakeHit_Left", "*.png").ToList();
            takeHitRight = Directory.GetFiles("TakeHit_Right", "*.png").ToList();
            bloodEffect = Directory.GetFiles("Blood", "*.png").ToList();
            fallLeft = Directory.GetFiles("Fall_Left","*.png").ToList();
            fallRight = Directory.GetFiles("Fall_Right", "*.png").ToList();
        }
        private void InitializeProperties()
        {
            Speed = 15;
            Gravity = 10;
            JumpSpeed = 30;
            IsOnGround = false;
            IsJumping = false;
            IsFlipped = false;
            this.Size = new Size(150, 60);
            this.BackColor = Color.Transparent;
        }
        private int slowCounter = 0;

        public void UpdateAnimation()
        {
            ++slowCounter;
            if (isDead) return;
            if (isTakingHit)
            {
                UpdateTakeHitAnimation();
            }
            if (slowCounter >= (int)(2 / slowDownFactor))
            {
                slowCounter = 0;
                if (IsAttacking)
                {
                    UpdateAttackingAnimation();
                }
                else if (!IsRunning && IsOnGround)
                {
                    UpdateIdleAnimation();
                }
                else if (IsRunning)
                {
                    UpdateRunningAnimation();
                }
            }
        }
        private bool flag = true;
        private void UpdateAttackingAnimation()
        {
            if(flag)
            {
                soundsfx[0].Play();
                flag = false;
            }
            if (attackFrame >= attackLeft.Count)
            {
                flag = true;
                attackFrame = 0;
                IsAttacking = false;
            }
            this.Image = Image.FromFile(IsFlipped ? attackLeft[attackFrame++] : attackRight[attackFrame++]);
        }
        private void UpdateIdleAnimation()
        {
            if (idleFrame > 6) idleFrame = 0;
            this.Image = Image.FromFile(IsFlipped ? idleLeft[idleFrame++] : idleRight[idleFrame++]);
        }
        private void UpdateRunningAnimation()
        {
            if (runningFrame >= runningRight.Count) runningFrame = 0;
            this.Image = Image.FromFile(IsFlipped ? runningLeft[runningFrame++] : runningRight[runningFrame++]);
        }

        private double JumpVelocity;
        
        private int MaxJumpHeight = 100;
        private bool isTakingHit = false;
        public void Jump()
        {
            if (isDead) return;
            if (IsOnGround)
            {
                IsJumping = true;
                IsOnGround = false;
                IsFalling = false;
                JumpVelocity = 4;
                this.Image = Image.FromFile(IsFlipped ? jumpingLeft[0] : jumpingRight[0]);
                soundsfx[1].Play();
            }
        }
        public void Dash()
        {
            if (isDead || isDashing || !canDash) return;
            isDashing = true;
            canDash = false;
            dashDir = IsFlipped ? -1 : 1;  
            dashTimer.Start();
        }
        private void PerformDash(object sender, EventArgs e)
        {
            if(dashStep <= 0)
            {
                EndDash();
                return;
            }
            this.Left += dashDist * dashDir;
            this.Image = Image.FromFile(IsFlipped ? runningLeft[3] : runningRight[3]);
            --dashStep;
        }
        private void EndDash()
        {
            isDashing = false;
            dashTimer.Stop();
            dashStep = 10;
            dashCooldownTimer.Start();
        }
        private void EndDashCooldown(object sender, EventArgs e)
        {
            canDash = true;
            dashCooldownTimer.Stop(); 
        }
        public void StartToFall()
        {
            if (startFalling) 
            { 
                this.Image = Image.FromFile(IsFlipped ? fallLeft[0] : fallRight[0]);
            }
        }

        public void ApplyGravity()
        {
            if (IsJumping && !IsFalling)
            {
                this.Top -= (int)(JumpSpeed * slowDownFactor);
                JumpVelocity -= slowDownFactor;
                if (JumpVelocity <= 0 || this.Top <= MaxJumpHeight)
                {
                    IsFalling = true;
                }
            }
            if (IsFalling || !IsOnGround)
            {
                this.Top += (int)(Gravity * slowDownFactor);
                //if (this.Parent != null)
                //{
                //    if (this.Top + this.Height >= this.Parent.ClientSize.Height)
                //    {
                //        IsOnGround = true;
                //        IsJumping = false;
                //        this.Top = this.Parent.ClientSize.Height - this.Height;
                //    }
                //}
            }
            UpdateHitboxPosition();
        }
        public void TakeDamage(int damage)
        {
            if (isDead) return;
            Health -= damage;
            takeHitFrame = 0;
            isTakingHit = true;
            bloodEffectFrame = 0;
            isBloodEffectRunning = true;
            soundsfx[6].Play();
            
            UpdateBloodEffect(this, EventArgs.Empty);
            bloodEffectTimer.Start();
            if (Health <= 0 && !isDead)
            {
                Die();
            }
        }
        public void Die()
        {
            isDead = true;
            deathAnimationTimer.Start();
        }
        public void Respawn()
        {
            isDead = false;
            Health = 100;
            isTakingHit = false;
            isBloodEffectRunning = false;
        }
        private void UpdateDeathAnimation(object sender, EventArgs e)
        {
            if (isDead)
            {
                List<string> currentDeathFrames = IsFlipped ? deathLeft : deathRight;

                if (deathFrameIndex < currentDeathFrames.Count)
                {
                    this.Image = Image.FromFile(currentDeathFrames[deathFrameIndex++]);
                }
                else
                {
                    deathAnimationTimer.Stop();
                }
            }
        }
        private void UpdateTakeHitAnimation()
        {
            if (isDead) return;
            if (takeHitFrame < takeHitLeft.Count || takeHitFrame < takeHitRight.Count)
            {
                if (IsFlipped)
                {
                    this.Image = Image.FromFile(takeHitLeft[takeHitFrame]);
                }
                else
                {
                    this.Image = Image.FromFile(takeHitRight[takeHitFrame]);
                }
                takeHitFrame++;
            }
            if (takeHitFrame >= takeHitLeft.Count || takeHitFrame >= takeHitRight.Count)
            {
                takeHitFrame = 0;
                isTakingHit = false;
            }
        }
        public void PlayerMove()
        {
            if (isDead) return;
            bool flag = true;
            if(this.Parent != null)
            {
                foreach (Control x in this.Parent.Controls)
                {
                    if (x is MeleeEnemy || x is RangedEnemy)
                    {
                        if (x is MeleeEnemy meleeEnemy)
                        {
                            if (this.HitBox.Bounds.IntersectsWith(meleeEnemy.hitBox.Bounds) && ((IsLeft && this.HitBox.Left > meleeEnemy.hitBox.Left) || (IsRight && this.HitBox.Left < meleeEnemy.hitBox.Left)))
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (x is RangedEnemy rangedEnemy)
                        {
                            if (this.HitBox.Bounds.IntersectsWith(rangedEnemy.Bounds) && ((IsLeft && this.HitBox.Left > rangedEnemy.Left) || (IsRight && this.HitBox.Left < rangedEnemy.Left)))
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                    if (x is PictureBox && (string)x.Tag == "BlockedWall")
                    {
                        if (this.HitBox.Bounds.IntersectsWith(x.Bounds) && ((IsLeft && this.HitBox.Left > x.Left) || (IsRight && this.HitBox.Left < x.Left)))
                        {
                            flag = false;
                            break;
                        }
                    }

                }
            }
            

            if (IsLeft && this.Left > 0 && !IsAttacking && flag)
            {
                this.Left -= (int)(Speed * slowDownFactor);
                IsFlipped = true;
                IsRunning = true;
            }
            else if (IsRight && !IsAttacking && flag)
            {
                this.Left += (int)(Speed * slowDownFactor);
                IsFlipped = false;
                IsRunning = true;
            }
            else
            {
                IsRunning = false;
            }
        }
        private void UpdateBloodEffect(object sender, EventArgs e)
        {
            if (bloodEffectFrame >= bloodEffect.Count)
            {
                bloodEffectFrame = 0;
                bloodEffectTimer.Stop();
                bloodEffectBox.Visible = false;
                isTakingHit = false;
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
        private void LoadSoundEffect()
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
        
    }

}