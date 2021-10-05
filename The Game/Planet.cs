using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Game
{
    class Planet : Celestial
    {
        public Planet(Point pos, Size size) : base(pos, Point.Empty, size)
        {
            pen = Pens.SandyBrown;
            image = Properties.Resources.planet;
        }
        public Planet(int x1, int y1, int x2, int y2) : base(new Point(x1, y1), Point.Empty, new Size(x2, y2))
        {
            pen = Pens.SandyBrown;
            image = Properties.Resources.planet;
        }

        public override void DrawInLines()
        {
            GameLogic.Buffer.Graphics.FillEllipse(pen.Brush, pos.X, pos.Y, size.Width, size.Height);
        }

        public override void Update() { }
        public override void Hit() { }
    }
}
