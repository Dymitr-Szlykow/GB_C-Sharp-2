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
        //protected BaseScene host;
        protected Pen pen;
        protected Bitmap image;

        public int NickOfTime { get; protected set; }
        public Point Pos { get => pos; }
        public Point Dir { get => dir; }
        public Size Size { get => size; }
        #endregion

        #region СОЗДАНИЕ ЭКЗЕМПЛЯРОВ, КОНСТРУКТОРЫ
        protected Celestial() { }
        public Celestial(Point pos, Point dir, Size size)
        {
            if (pos.IsEmpty || size.IsEmpty)
                throw new GameObjectException("Необходимо задать объекту действительные значения начального положения и размера.");
            if (size.Width < 0 || size.Height < 0)
                throw new GameObjectException("Попытка создать объект с недопустимым размером.");
            //if (pos.X < -2 * size.Width || pos.X > InGame.Width + 2 * size.Width || pos.Y < -2 * size.Height || pos.Y > InGame.Height + 2 * size.Height)
            //    throw new GameObjectException("Попытка создать объект слишком далеко за краем экрана.");
            if (dir.X < -50 || 50 < dir.X || dir.Y < -50 || 50 < dir.Y)
                throw new GameObjectException("Попытка создать объект со слишком высокой скоростью.");

            this.pos = pos;
            this.dir = dir;
            this.size = size;
            NickOfTime = 0;
        }
        //public Celestial(Point pos, Point dir, Size size, BaseScene host) : this(pos, dir, size)
        //{
        //    this.host = host;
        //}
        #endregion

        #region ИНТЕРФЕЙС ICollision
        public virtual Rectangle Rect { get { return new Rectangle(pos, size); } }
        public virtual int ScoreCost { get; }

        public virtual bool CollidesWith(ICollision other)
        {
            if (Rect.IntersectsWith(other.Rect))
                return true;
            else
                return false;
        }
        #endregion

        #region МЕТОДЫ: ОТРИСОВКА
        public static void Draw(Celestial obj, Graphics hostGraphics) => obj.Draw(hostGraphics);
        public static void Draw(Celestial[] objset, Graphics hostGraphics)
        {
            foreach (Celestial obj in objset)
            {
                if (obj != null) obj.Draw(hostGraphics);
            }
        }
        public static void Draw(List<Celestial> objset, Graphics hostGraphics)
        {
            foreach (Celestial obj in objset)
            {
                obj.Draw(hostGraphics);
            }
        }
        public static void Draw(List<Celestial>[] objset, Graphics hostGraphics)
        {
            foreach (List<Celestial> objlist in objset)
            {
                Draw(objlist, hostGraphics);
            }
        }
        public void Draw(Graphics hostGraphics)
        {
            if (image != null) DrawImage(hostGraphics);
            else DrawInLines(hostGraphics);
        }

        public abstract void DrawInLines(Graphics hostGraphics);
        public virtual void DrawImage(Graphics hostGraphics) => hostGraphics.DrawImage(image, pos.X, pos.Y, size.Width, size.Height);
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
        public virtual void Hit() => throw new NotImplementedException();
        public virtual void Hit(int damage) => throw new NotImplementedException();
        public virtual void Picked(Ship player) => throw new NotImplementedException();
        #endregion

        #region МЕТОДЫ: ПОВЕДЕНИЕ
        protected virtual void Move()
        {
            pos.X += dir.X;
            pos.Y += dir.Y;
        }

        protected void Ricochet() //(Size hostWindow)
        {
            if ((pos.X < 0 && dir.X < 0) || (pos.X > InGame.Width - size.Width && dir.X > 0))
                dir.X = -dir.X;
            if ((pos.Y < 0 && dir.Y < 0) || (pos.Y > InGame.Height - size.Height && dir.Y > 0))
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
        public virtual bool IsEmpty() => throw new NotImplementedException();
        public bool OutOfView()
        {
            if (pos.X < -size.Width || pos.X > InGame.Width || pos.Y < -size.Height || pos.Y > InGame.Height)
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
