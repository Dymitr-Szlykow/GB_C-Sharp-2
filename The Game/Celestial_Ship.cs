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
        public int Energy { get; private set; }
        public int LastDamage { get; private set; }
        public override int ScoreCost { get; }


        public event EventHandler<DeathEventArgs> Death;


        public Ship(Point pos, Point dir, Size size) : base(pos, dir, size)
        {
            Energy = 100;
            LastDamage = 0;
            pen = Pens.White;
            image = Properties.Resources.ship;
        }

        public override void DrawInLines()
        {
            GameLogic.Buffer.Graphics.DrawLine(pen, pos.X, pos.Y, pos.X, pos.Y + size.Height);
            GameLogic.Buffer.Graphics.DrawLine(pen, pos.X, pos.Y + size.Height, pos.X + size.Width, pos.Y + size.Height / 2);
            GameLogic.Buffer.Graphics.DrawLine(pen, pos.X, pos.Y, pos.X + size.Width, pos.Y + size.Height / 2);
        }

        public override void Update() { }

        public override void Hit() { }
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
            if (pos.Y <= GameLogic.Height - size.Height)
            {
                if (GameLogic.Height - pos.Y < dir.Y)
                    pos.Y = GameLogic.Height - size.Height - 1;
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
            if (pos.X < GameLogic.Width / 3 - size.Width)
                pos.X += dir.X;
        }

        public void Dies()
        {
            if (Death != null)
            {
                Death.Invoke(this, new DeathEventArgs(LastDamage));
            }
        }
        #endregion
    }
}
