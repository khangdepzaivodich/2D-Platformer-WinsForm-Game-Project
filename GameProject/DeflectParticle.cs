using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GameProject
{
    public class DeflectParticle : PictureBox
    {
        private List<string> particleFrames;
        private int currentFrame;
        private Timer particleAnimator;
        private int particleAnimationDelayCounter;
        private int totalFrames;

        public DeflectParticle(Point deflectPosition)
        {
            InitializeParticle(deflectPosition);
        }

        private void InitializeParticle(Point deflectPosition)
        {
            this.Size = new Size(30, 15);
            this.BackColor = Color.Transparent;
            this.Location = deflectPosition;
            particleFrames = Directory.GetFiles("DeflectFrame", "*.png").ToList();
            totalFrames = particleFrames.Count;

            this.Image = Image.FromFile(particleFrames[currentFrame]);
            this.SizeMode = PictureBoxSizeMode.StretchImage;
            particleAnimationDelayCounter = 0;
            particleAnimator = new Timer();
            particleAnimator.Interval = 10; 
            particleAnimator.Tick += AnimateParticle;
            particleAnimator.Start();
        }

        private void AnimateParticle(object sender, EventArgs e)
        {
            particleAnimationDelayCounter++;
            if (particleAnimationDelayCounter >= 1) 
            {
                currentFrame++;
                if (currentFrame >= totalFrames)
                {
                    particleAnimator.Stop();
                    this.Dispose();
                    currentFrame = 0;
                }
                else
                {
                    this.Image = Image.FromFile(particleFrames[currentFrame]);
                }
                particleAnimationDelayCounter = 0;
            }
        }
    }
}
