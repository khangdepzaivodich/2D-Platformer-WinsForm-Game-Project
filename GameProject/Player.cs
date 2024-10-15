using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        private int idleFrame;
        private int runningFrame;
        private int attackFrame;
        private int deathFrame;
        public PictureBox HitBox;
        public Player()
        {
            LoadAnimation();
            InitializeProperties();
        }
        public void CreateHitBox()
        {
            HitBox = new PictureBox();
            HitBox.BackColor = Color.Transparent;
            HitBox.Size = new Size(50,70);
            HitBox.Visible = false;
            HitBox.BorderStyle = BorderStyle.FixedSingle;
            this.Parent.Controls.Add(HitBox);
            UpdateHitboxPosition();
        }

        private void UpdateHitboxPosition()
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
            if(slowCounter == 2)
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
            UpdateHitboxPosition();
        }

        private void UpdateAttackingAnimation()
        {
            if(attackFrame >= attackLeft.Count)
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

        public void Jump()
        {
            if (IsOnGround)
            {
                IsJumping = true;
                IsOnGround = false;
                IsFalling = false;
                JumpVelocity = 4;
                this.Image = Image.FromFile(IsFlipped ? jumpingLeft[0] : jumpingRight[0]);
            }
            UpdateHitboxPosition();
        }
        public void TakeDamage(int damage)
        {
            Health -= damage;
        }

        public void PlayerMove()
        {
            
            if (IsLeft && this.Left > 0 && !IsAttacking)
            {
                this.Left -= Speed;
                IsFlipped = true;
                IsRunning = true;
            }
            else if (IsRight && this.Right < this.Parent.ClientSize.Width && !IsAttacking)
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
            if(IsFalling || !IsOnGround)
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
    }
}
