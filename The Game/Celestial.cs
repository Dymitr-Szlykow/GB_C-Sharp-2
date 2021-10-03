using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Game
{
    abstract class Celestial
    {
        #region ПОЛЯ И СВОЙСТВА
        //protected static Pen pen;

        protected Point pos;
        protected Point dir;
        protected Size size;
        protected Bitmap image;

        public Point Pos { get; }
        public Point Dir { get; }
        public Size Size { get; }
        //public abstract Pen Pen { get; }
        #endregion

        #region СОЗДАНИЕ ЭКЗЕМПЛЯРОВ, КОНСТРУКТОРЫ
        protected Celestial() { }
        public Celestial(Point pos, Point dir, Size size)
        {
            this.pos = pos;
            this.dir = dir;
            this.size = size;
        }
        #endregion

        #region МЕТОДЫ: ОТРИСОВКА
        public static void Draw(Celestial obj)
        {
            obj.Draw();
        }
        public static void Draw(Celestial[] objset)
        {
            foreach (Celestial obj in objset)
            {
                obj.Draw();
            }
        }
        public static void Draw(List<Celestial> objset)
        {
            foreach (Celestial obj in objset)
            {
                obj.Draw();
            }
        }
        public void Draw()
        {
            //DrawInLines();
            try { DrawImage(); }
            catch { DrawInLines(); }
        }

        public abstract void DrawInLines();
        public void DrawImage()
        {
            GameLogic.Buffer.Graphics.DrawImage(image, pos.X, pos.Y, size.Width, size.Height);
        }
        #endregion

        #region МЕТОДЫ: В ЦИКЛЕ
        public static void Update(Celestial obj)
        {
            obj.Update();
        }
        public static void Update(Celestial[] objset)
        {
            foreach (Celestial obj in objset)
            {
                obj.Update();
            }
        }
        public static void Update(List<Celestial> objset)
        {
            foreach (Celestial obj in objset)
            {
                obj.Update();
            }
        }
        public static void Update(Queue<Celestial> objset)
        {
            foreach (Celestial obj in objset)
            {
                obj.Update();
            }
        }
        public abstract void Update();
        #endregion

        #region МЕТОДЫ: ПОВЕДЕНИЕ
        protected virtual void Move()
        {
            pos.X += dir.X;
            pos.Y += dir.Y;
        }

        protected void Ricochet()
        {
            if (pos.X < 0 || pos.X > GameLogic.Width - size.Width)
                dir.X = -dir.X;
            if (pos.Y < 0 || pos.Y > GameLogic.Height - size.Height)
                dir.Y = -dir.Y;
        }

        protected void Reduce(int reduction)
        {
            size -= new Size(reduction, reduction);
            pos += new Size(reduction / 2, reduction / 2);
        }
        #endregion

        #region ПРОВЕРКИ
        public bool OutOfView()
        {
            if (pos.X < -size.Width || pos.X > GameLogic.Width || pos.Y < -size.Height || pos.Y > GameLogic.Height)
                return true;
            else
                return false;
        }

        public bool StandsStill()
        {
            if (dir.X == 0 && dir.Y == 0)
                return true;
            else
                return false;
        }

        public bool SmallerThan(int cap)
        {
            if (size.Width < cap || size.Height < cap)
                return true;
            else
                return false;
        }
        #endregion
    }
}
