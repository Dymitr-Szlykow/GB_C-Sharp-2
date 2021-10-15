using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Game
{
    class Comet : Celestial
    {
        private Queue<Celestial> tail;

        public Celestial Head { get { return tail.Last(); } }
        public override int ScoreCost { get { return 50; } }


        public Comet(Point pos, Point dir, Size size) : base(pos, dir, size)
        {
            tail = new Queue<Celestial>();
            tail.Enqueue(new Tailpiece(pos, size));
        }


        public override void DrawInLines(Graphics hostGraphics)
        {
            foreach (Tailpiece piece in tail)
            {
                piece.Draw(hostGraphics);
            }
        }

        public override void Update()
        {
            Update(tail);
            if (tail.Peek().SmallerThan(2)) tail.Dequeue();
            if (Dir != Point.Empty)
            {
                Move();
                Ricochet();
            }
        }

        public override void Hit()
        {
            dir = Point.Empty;
        }

        protected override void Move()
        {
            pos.X += dir.X;
            pos.Y += dir.Y;
            tail.Enqueue(new Tailpiece(pos, size));
        }

        public override bool IsEmpty()
        {
            if (tail.Count == 0)
                return true;
            else
                return false;
        }


        private class Tailpiece : Celestial
        {
            public override int ScoreCost { get { return 50; } }

            public Tailpiece(Point pos, Size size) : base(pos, Point.Empty, size)
            {
                pen = Pens.Red;
            }

            public override void DrawInLines(Graphics hostGraphics)
            {
                hostGraphics.FillEllipse(pen.Brush, pos.X, pos.Y, size.Width, size.Height);
            }

            public override void Update()
            {
                Reduce(2);
                ChangeBrush();
            }

            private void ChangeBrush()
            {
                //if (SmallerThan(6) && pen != Pens.Yellow) pen = Pens.Yellow;
                if (SmallerThan(8) && pen != Pens.Orange) pen = Pens.Orange;
                else if (pen != Pens.OrangeRed) pen = Pens.OrangeRed;
            }
        }
    }
}
