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
        public event EventHandler<EndGameEventArgs> ReachingEdge;

        public Planet(Point pos, Size size) : base(pos, new Point(-1, 0), size)
        {
            pen = Pens.SandyBrown;
            image = Properties.Resources.planet;
        }
        public Planet(int x1, int y1, int x2, int y2) : base(new Point(x1, y1), new Point(-1, 0), new Size(x2, y2))
        {
            pen = Pens.SandyBrown;
            image = Properties.Resources.planet;
        }

        public override void DrawInLines(Graphics hostGraphics)
        {
            hostGraphics.FillEllipse(pen.Brush, pos.X, pos.Y, size.Width, size.Height);
        }

        public override void Update()
        {
            if (++NickOfTime == 3)
            {
                NickOfTime = 0;
                Move();
            }
        }

        public void ReachesEdge()
        {
            if (ReachingEdge != null)
            {
                ReachingEdge.Invoke(this, new EndGameEventArgs("Конец!", "путь пройден (планета достигла края экрана)"));
            }
        }
    }
}
