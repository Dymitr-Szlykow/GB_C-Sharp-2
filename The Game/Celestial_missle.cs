using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Game
{
    class Missle : Celestial
    {
        private Size Startpoint
        {
            get
            {
                return new Size((size.Width - dir.X) / 2, (size.Height - dir.Y) / 2);
            }
        }
        public override int ScoreCost { get; }

        public Missle(Point pos, Point dir, Size size) : base(pos, dir, size)
        {
            pen = Pens.MediumVioletRed;
            //image = Properties.Resources.laserRed011;
            //pen.Width = 2.0F;
        }

        public override void DrawInLines(Graphics hostGraphics)
        {
            hostGraphics.DrawLine(pen, pos + Startpoint, pos + Startpoint + new Size(dir));
        }

        public override void Update()
        {
            Move();
        }
    }
}
