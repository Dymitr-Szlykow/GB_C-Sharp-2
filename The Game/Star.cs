using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Game
{
    class Star : Celestial
    {
        private static readonly Bitmap[] images = {
            Properties.Resources.star1,
            Properties.Resources.star2,
            Properties.Resources.star3
        };

        public Star(Point pos, Point dir, Size size) : base(pos, dir, size)
        {
            pen = Pens.Orange;
            image = images[GameLogic.rand.Next(0, images.Length)];
        }

        public override void DrawInLines()
        {
            GameLogic.Buffer.Graphics.DrawLine(pen, pos.X + size.Width / 2, pos.Y, pos.X + size.Width / 2, pos.Y + size.Height);
            GameLogic.Buffer.Graphics.DrawLine(pen, pos.X, pos.Y + size.Height / 2, pos.X + size.Width, pos.Y + size.Height / 2);
        }

        public override void Update()
        {
            Move();
        }

        public override void Hit() { }
    }
}
