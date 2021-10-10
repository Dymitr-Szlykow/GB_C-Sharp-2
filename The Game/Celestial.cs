using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Game
{
    abstract class Celestial : ICollision
    {
        #region ПОЛЯ И СВОЙСТВА
        protected Point pos;
        protected Point dir;
        protected Size size;
        protected Pen pen;
        protected Bitmap image;

        public Point Pos { get { return pos; } }
        public Point Dir { get { return dir; } }
        public Size Size { get { return size; } }
        #endregion

        #region СОЗДАНИЕ ЭКЗЕМПЛЯРОВ, КОНСТРУКТОРЫ
        protected Celestial() { }
        public Celestial(Point pos, Point dir, Size size)
        {
            if (pos.IsEmpty || size.IsEmpty)
                throw new GameObjectException("Необходимо задать объекту действительные значения начального положения и размера.");
            if (size.Width < 0 || size.Height < 0)
                throw new GameObjectException("Попытка создать объект с недопустимым размером.");
            if (pos.X < -2 * size.Width || pos.X > GameLogic.Width + 2 * size.Width || pos.Y < -2 * size.Height || pos.Y > GameLogic.Height + 2 * size.Height)
                throw new GameObjectException("Попытка создать объект слишком далеко за краем экрана.");
            if (dir.X < -50 || 50 < dir.X || dir.Y < -50 || 50 < dir.Y)
                throw new GameObjectException("Попытка создать объект со слишком высокой скоростью.");

            this.pos = pos;
            this.dir = dir;
            this.size = size;
        }
        #endregion

        #region ИНТЕРФЕЙС ICollision
        public Rectangle Rect { get { return new Rectangle(pos, size); } }
        public abstract int ScoreCost { get; }

        public bool CollidesWith(ICollision other)
        {
            if (Rect.IntersectsWith(other.Rect))
                return true;
            else
                return false;
        }
        #endregion

        #region МЕТОДЫ: ОТРИСОВКА
        public static void Draw(Celestial obj) => obj.Draw();
        public static void Draw(Celestial[] objset)
        {
            foreach (Celestial obj in objset)
            {
                if (obj != null) obj.Draw();
            }
        }
        public static void Draw(List<Celestial> objset)
        {
            foreach (Celestial obj in objset)
            {
                obj.Draw();
            }
        }
        public static void Draw(List<Celestial>[] objset)
        {
            foreach (List<Celestial> objlist in objset)
            {
                Draw(objlist);
            }
        }
        public void Draw()
        {
            if (image != null) DrawImage();
            else DrawInLines();
        }

        public abstract void DrawInLines();
        public virtual void DrawImage() => GameLogic.Buffer.Graphics.DrawImage(image, pos.X, pos.Y, size.Width, size.Height);
        #endregion

        #region МЕТОДЫ: В ЦИКЛЕ
        public static void Update(Celestial obj) => obj.Update();
        public static void Update(Celestial[] objset)
        {
            foreach (Celestial obj in objset)
            {
                if (obj != null) obj.Update();
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
        public abstract void Hit();
        public abstract void Hit(int damage);
        #endregion

        #region МЕТОДЫ: ПОВЕДЕНИЕ
        protected virtual void Move()
        {
            pos.X += dir.X;
            pos.Y += dir.Y;
        }

        protected void Ricochet()
        {
            if ((pos.X < 0 && dir.X < 0) || (pos.X > GameLogic.Width - size.Width && dir.X > 0))
                dir.X = -dir.X;
            if ((pos.Y < 0 && dir.Y < 0) || (pos.Y > GameLogic.Height - size.Height && dir.Y > 0))
                dir.Y = -dir.Y;
        }

        public void Bump(ICollision other)
        {
            // TODO
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
