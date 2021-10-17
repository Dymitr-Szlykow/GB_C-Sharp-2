using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace The_Game
{
    class BaseScene : IScene
    {
        protected BufferedGraphicsContext _context;
        protected Form _form;
        public static BufferedGraphics Buffer;
        public static Timer timer;

        public static int Width { get; set; }
        public static int Height { get; set; }

        public virtual void Init(Form form)
        {
            if (form.ClientSize.Width < 0 || 1000 < form.ClientSize.Width || form.ClientSize.Height < 0 || 1000 < form.ClientSize.Height)
            {
                throw new ArgumentOutOfRangeException();
            }
            _form = form;
            Graphics g = _form.CreateGraphics();
            Width = _form.ClientSize.Width;
            Height = _form.ClientSize.Height;
            _context = BufferedGraphicsManager.Current;
            Buffer = _context.Allocate(g, new Rectangle(0, 0, Width, Height));

            _form.KeyDown += SceneKeyDown;
        }

        public virtual void SceneKeyDown(object sender, KeyEventArgs e) { }

        public virtual void Draw() { }

        public virtual void Dispose()
        {
            timer?.Stop();
            _form.KeyDown -= SceneKeyDown;
            Buffer = null;
            _context = null;
        }
    }

    public interface IScene
    {
        void Init(Form form);
        void Draw();
    }
}
