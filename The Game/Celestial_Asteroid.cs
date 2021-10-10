using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Game
{
    class Asteroid : Celestial
    {
        private static readonly Bitmap[] images = {
            Properties.Resources.meteorBrown_big1,
            Properties.Resources.meteorBrown_big2,
            Properties.Resources.meteorBrown_big3,
            Properties.Resources.meteorBrown_big4
        };
        public override int ScoreCost { get { return 10; } }

        public Asteroid(Point pos, Point dir, Size size) : base(pos, dir, size)
        {
            pen = Pens.White;
            image = images[GameLogic.rand.Next(images.Length)];
        }

        public override void DrawInLines()
        {
            GameLogic.Buffer.Graphics.DrawEllipse(pen, pos.X, pos.Y, size.Width, size.Height);
        }

        public override void Update()
        {
            Move();
            Ricochet();
        }

        public override void Hit()
        {
            switch(GameLogic.rand.Next(3))
            {
                case 0:
                    pos.X = -this.size.Width;
                    pos.Y = GameLogic.rand.Next(-this.size.Height, GameLogic.Height);
                    break;
                case 1:
                    pos.X = GameLogic.Width;
                    pos.Y = GameLogic.rand.Next(-this.size.Height, GameLogic.Height);
                    break;
                case 2:
                    pos.X = GameLogic.rand.Next(-this.size.Width, GameLogic.Width);
                    pos.Y = -this.size.Height;
                    break;
                case 3:
                    pos.X = GameLogic.rand.Next(-this.size.Width, GameLogic.Width);
                    pos.Y = GameLogic.Height;
                    break;
            }
        }
        public override void Hit(int damage) { }
    }
}
