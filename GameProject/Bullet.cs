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
    public class Bullet : PictureBox
    {
        public int Speed { get; set; }
        public bool IsMovingRight { get; set; }
        private List<string> bulletFrames;
        private int currentFrame = 0;
        private Timer bulletAnimator;
        private int BulletAnimationDelayCounter;
        public Bullet(bool shootRight)
        {
            InitializeBullet(shootRight);
        }
        private void InitializeBullet(bool shootRight)
        {
            this.Size = new Size(50, 5);
            this.BackColor = Color.Transparent;
            bulletFrames = shootRight ?
                Directory.GetFiles("Bullet_Right", "*.png").ToList() :
                Directory.GetFiles("Bullet_Left", "*.png").ToList();

            this.Image = Image.FromFile(bulletFrames[currentFrame]);
            this.SizeMode = PictureBoxSizeMode.StretchImage;
            BulletAnimationDelayCounter = 0;
            bulletAnimator = new Timer();
            bulletAnimator.Interval = 20;
            bulletAnimator.Tick += AnimateBullet;
            bulletAnimator.Start();
        }
        public void Shoot()
        {
            if (IsMovingRight)
            {
                this.Left += Speed;
            }
            else
            {
                this.Left -= Speed;
            }
        }
        private void AnimateBullet(object sender, EventArgs e)
        {
            if(BulletAnimationDelayCounter == 0)
            {
                currentFrame++;
                if (currentFrame >= bulletFrames.Count)
                {
                    currentFrame = 0;
                }

                this.Image = Image.FromFile(bulletFrames[currentFrame]);
                BulletAnimationDelayCounter = 0;
            }
            
        }
        
    }
}
