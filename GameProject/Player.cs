using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.IO;
using System.Linq;
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

        public int Speed { get; set; }
        public int Gravity { get; set; }
        public int JumpSpeed { get; set; }
        public int Health { get; private set; } = 100;
        public int AttackDamage { get; private set; } = 20;
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


        private int idleFrame;
        private int runningFrame;
        private int attackFrame;
        private int deathFrame;
        private int takeHitFrame;
        private Main mainForm;

        private bool isDead = false;
        private int deathFrameIndex = 0;
        private Timer deathAnimationTimer;

        public PictureBox HitBox;
        public Player(Main main)
        {
            LoadAnimation();
            InitializeProperties();
            mainForm = main;

            deathAnimationTimer = new Timer();
            deathAnimationTimer.Interval = 200;
            deathAnimationTimer.Tick += UpdateDeathAnimation;
        }
        public void CreateHitBox()
        {
            HitBox = new PictureBox();
            HitBox.BackColor = Color.Transparent;
            HitBox.Size = new Size(76, 70);
            HitBox.Visible = true;
            HitBox.BorderStyle = BorderStyle.FixedSingle;
            this.Parent.Controls.Add(HitBox);
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
            if (slowCounter == 2)
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

        private void UpdateAttackingAnimation()
        {
            if (attackFrame >= attackLeft.Count)
            {
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

        private int JumpVelocity;
        private bool IsFalling = false;
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
            }
        }

        public void ApplyGravity()
        {
            if (IsJumping && !IsFalling)
            {
                this.Top -= JumpSpeed;
                JumpVelocity--;
                if (JumpVelocity <= 0 || this.Top <= MaxJumpHeight)
                {
                    IsFalling = true;
                }
            }
            if (IsFalling || !IsOnGround)
            {
                this.Top += Gravity;

                if (this.Top + this.Height >= this.Parent.ClientSize.Height)
                {
                    IsOnGround = true;
                    IsJumping = false;
                    this.Top = this.Parent.ClientSize.Height - this.Height;
                }
            }
            UpdateHitboxPosition();
        }
        public void TakeDamage(int damage)
        {
            Health -= damage;
            takeHitFrame = 0;
            isTakingHit = true;
            mainForm.RemoveHeart();

            if (Health <= 0 && !isDead)
            {
                Die();
            }
        }
        private void Die()
        {
            isDead = true;
            deathAnimationTimer.Start();
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
            foreach (Control x in this.Parent.Controls)
            {
                if ((x is MeleeEnemy || x is RangedEnemy) && this.HitBox.Bounds.IntersectsWith(x.Bounds))
                {
                    if((IsLeft && x.Left < this.HitBox.Left) || (IsRight && x.Left > this.HitBox.Left))
                    {
                        flag = false;
                        break;
                    }
                }
            }
            
            if (IsLeft && this.Left > 0 && !IsAttacking && flag)
            { 
                this.Left -= Speed;
                IsFlipped = true;
                IsRunning = true;
            }
            else if (IsRight && this.Right < this.Parent.ClientSize.Width && !IsAttacking && flag)
            {
                this.Left += Speed;
                IsFlipped = false;
                IsRunning = true;
            }
            else
            {
                IsRunning = false;
            }
        }
    }
}
