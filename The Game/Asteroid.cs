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
        public static readonly Pen pen = Pens.White;
        public static readonly Brush brush = Brushes.White;

        private static readonly Bitmap[] images = {
            Properties.Resources.meteorBrown_big1,
            Properties.Resources.meteorBrown_big2,
            Properties.Resources.meteorBrown_big3,
            Properties.Resources.meteorBrown_big4
        };

        public Asteroid(Point pos, Point dir, Size size) : base(pos, dir, size)
        {
            image = images[GameLogic.rand.Next(0, images.Length)];
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
    }
}
