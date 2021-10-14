using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Game
{
    class Ship : Celestial
    {
        public int Energy { get; set; }
        public int LastDamage { get; private set; }
        public bool MayShoot { get; private set; }
        public override int ScoreCost { get; }
        public override Rectangle Rect { get { return new Rectangle(pos.X + 4, pos.Y +4, size.Width - 4, size.Height - 4); } }


        public event EventHandler<DeathEventArgs> Death;


        public Ship(Point pos, Point dir, Size size) : base(pos, dir, size)
        {
            Energy = 100;
            LastDamage = 0;
            MayShoot = true;
            pen = Pens.White;
            image = Properties.Resources.ship;
        }

        public override void DrawInLines(Graphics hostGraphics)
        {
            hostGraphics.DrawLine(pen, pos.X, pos.Y, pos.X, pos.Y + size.Height);
            hostGraphics.DrawLine(pen, pos.X, pos.Y + size.Height, pos.X + size.Width, pos.Y + size.Height / 2);
            hostGraphics.DrawLine(pen, pos.X, pos.Y, pos.X + size.Width, pos.Y + size.Height / 2);
        }

        public override void Update()
        {
            CountdownShootImpedance();
        }

        public override void Hit(int damage)
        {
            LastDamage = damage;
            Energy -= damage;
        }

        #region СВОЕ ПОВЕДЕНИЕ
        public void MoveUp()
        {
            if (pos.Y > 0)
            {
                if (pos.Y <= dir.Y)
                    pos.Y = 1;
                else
                    pos.Y -= dir.Y;
            }   
        }
        public void MoveDown()
        {
            if (pos.Y <= InGame.Height - size.Height)
            {
                if (InGame.Height - pos.Y < dir.Y)
                    pos.Y = InGame.Height - size.Height - 1;
                else
                    pos.Y += dir.Y;
            }
        }
        public void MoveLeft()
        {
            if (pos.X > 0)
            {
                if (pos.X <= dir.X)
                    pos.X = 1;
                else
                    pos.X -= dir.X;
            }
        }
        public void MoveRight()
        {
            if (pos.X < InGame.Width / 3 - size.Width)
                pos.X += dir.X;
        }

        public void Shot()
        {
            NickOfTime++;
            MayShoot = false;
        }

        private void CountdownShootImpedance()
        {
            if (NickOfTime > 0)
            {
                NickOfTime++;
                if (NickOfTime > 10)
                {
                    NickOfTime = 0;
                    MayShoot = true;
                }
            }
        }

        public void Dies() => Death?.Invoke(this, new DeathEventArgs(LastDamage, "Game over!", "корабль уничтожен"));
        #endregion
    }
}
