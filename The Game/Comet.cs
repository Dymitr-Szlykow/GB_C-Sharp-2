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

        public Comet(Point pos, Point dir, Size size) : base(pos, dir, size)
        {
            tail = new Queue<Celestial>();
            tail.Enqueue(new Tailpiece(pos, size));
        }

        public override void DrawInLines()
        {
            foreach (Tailpiece piece in tail)
            {
                piece.Draw();
            }
        }

        public override void Update()
        {
            Update(tail);
            if (tail.Peek().SmallerThan(2)) tail.Dequeue();
            Move();
            Ricochet();
        }

        protected override void Move()
        {
            pos.X += dir.X;
            pos.Y += dir.Y;
            tail.Enqueue(new Tailpiece(pos, size));
        }


        private class Tailpiece : Celestial
        {
            public Brush brush;

            public Tailpiece(Point pos, Size size) : base(pos, Point.Empty, size)
            {
                brush = Brushes.Red;
            }

            public override void DrawInLines()
            {
                GameLogic.Buffer.Graphics.FillEllipse(brush, pos.X, pos.Y, size.Width, size.Height);
            }

            public override void Update()
            {
                Reduce(2);
                ChangeBrush();
            }

            private void ChangeBrush()
            {
                //if (SmallerThan(6)) brush = Brushes.Yellow;
                if (SmallerThan(8)) brush = Brushes.Orange;
                else brush = Brushes.OrangeRed;
            }
        }
    }
}
