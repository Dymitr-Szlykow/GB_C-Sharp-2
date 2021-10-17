using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Game
{
    class MenuOption
    {
        #region ПОЛЯ И СВОЙСТВА
        protected Graphics _hostGraphics;
        public delegate void PushFunction();
        public PushFunction Push;

        public string Content { get; protected set; }
        public Font Font { get; protected set; }
        public Point PosContent { get; protected set; }
        public Point PosSelf { get; protected set; }
        public Size SizeContent { get; protected set; }
        public Size SizeSelf { get; protected set; }
        public Point[] SelectMarkerLeft => new Point[]
        {
            new Point(PosSelf.X - 20, PosSelf.Y),
            new Point(PosSelf.X - 10, PosSelf.Y + SizeSelf.Height / 2),
            new Point(PosSelf.X - 20, PosSelf.Y + SizeSelf.Height)
        };
        public Point[] SelectMarkerRight => new Point[]
        {
            new Point(PosSelf.X + SizeSelf.Width + 20, PosSelf.Y),
            new Point(PosSelf.X + SizeSelf.Width + 10, PosSelf.Y + SizeSelf.Height / 2),
            new Point(PosSelf.X + SizeSelf.Width + 20, PosSelf.Y + SizeSelf.Height)
        };
        #endregion

        #region КОНСТРУКТОРЫ
        protected MenuOption() { }
        public MenuOption(int windowTop, string content, Font font, Graphics hostGraphics, PushFunction func)
        {
            //if (pos.IsEmpty || size.IsEmpty)
            //    throw new GameObjectException("Необходимо задать объекту действительные значения начального положения и размера.");
            //if (size.Width < 0 || size.Height < 0)
            //    throw new GameObjectException("Попытка создать объект с недопустимым размером.");
            //if (pos.X < -2 * size.Width || pos.X > InGame.Width + 2 * size.Width || pos.Y < -2 * size.Height || pos.Y > InGame.Height + 2 * size.Height)
            //    throw new GameObjectException("Попытка создать объект слишком далеко за краем экрана.");
            //if (dir.X < -50 || 50 < dir.X || dir.Y < -50 || 50 < dir.Y)
            //    throw new GameObjectException("Попытка создать объект со слишком высокой скоростью.");

            _hostGraphics = hostGraphics;
            Push = func;
            Content = content;
            Font = font;
            SizeContent = Size.Round(hostGraphics.MeasureString(Content, Font, Menu.Width));
            PosContent = new Point((Menu.Width - SizeContent.Width) / 2, windowTop + 7);
            SizeSelf = new Size(SizeContent.Width + 26, SizeContent.Height + 14);
            PosSelf = new Point((Menu.Width - SizeSelf.Width) / 2, windowTop);
        }
        #endregion

        public void DrawButton()
        {
            _hostGraphics.FillRectangle(Brushes.Black, PosSelf.X, PosSelf.Y, SizeSelf.Width, SizeSelf.Height);
            _hostGraphics.DrawRectangle(Pens.OrangeRed, PosSelf.X, PosSelf.Y, SizeSelf.Width, SizeSelf.Height);
            _hostGraphics.DrawString(Content, Font, Brushes.OrangeRed, PosContent.X, PosContent.Y);
        }

        public void DrawFocused()
        {
            DrawButton();
            _hostGraphics.FillPolygon(Brushes.OrangeRed, SelectMarkerLeft);
            _hostGraphics.FillPolygon(Brushes.OrangeRed, SelectMarkerRight);
        }

        public void DrawContent()
        {
            _hostGraphics.FillRectangle(Brushes.Black, PosContent.X, PosContent.Y, SizeContent.Width, SizeContent.Height);
            _hostGraphics.DrawString(Content, Font, Brushes.Red, PosContent.X, PosContent.Y);
        }
    }
}
