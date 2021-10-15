using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Game
{
    class EngiKit : Celestial
    {
        public int Restore { get; private set; }

        public EngiKit(Point pos, Size size, int restore) : base(pos, Point.Empty, size)
        {
            if (restore <= 0)
                throw new ArgumentOutOfRangeException("Значение востанавливаемой инженерским набором энергии должно быть больше нуля.");
            Restore = restore;
            pen = Pens.Red;
        }

        public override void DrawInLines(Graphics hostGraphics)
        {
            hostGraphics.FillRectangle(Brushes.Red, pos.X + size.Width /  3, pos.Y, size.Width / 3, size.Height);
            hostGraphics.FillRectangle(Brushes.Red, pos.X, pos.Y + size.Height / 3, size.Width, size.Height / 3);
            hostGraphics.DrawEllipse(Pens.White, pos.X, pos.Y, size.Width, size.Height);
        }

        public override void Hit() => throw new NotImplementedException();
        public override void Hit(int damage)
        {
            throw new NotImplementedException();
        }

        public override void Picked(Ship player)
        {
            if (player.Energy > 100 - Restore)
                player.Energy = 100;
            else
                player.Energy += Restore;
        }

        public override void Update() { }
    }
}
