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
            image = images[InGame.rand.Next(images.Length)];
        }

        public override void DrawInLines(Graphics hostGraphics)
        {
            hostGraphics.DrawEllipse(pen, pos.X, pos.Y, size.Width, size.Height);
        }

        public override void Update()
        {
            Move();
            Ricochet();
        }
    }
}
