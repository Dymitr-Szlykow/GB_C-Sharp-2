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
        public static Random rand = new Random();

        public static int Width { get; set; }
        public static int Height { get; set; }

        public virtual void Init(SceneArgs instructions)
        {
            if (instructions._form.ClientSize.Width < 0 || 1000 < instructions._form.ClientSize.Width ||
                instructions._form.ClientSize.Height < 0 || 1000 < instructions._form.ClientSize.Height)
            {
                throw new ArgumentOutOfRangeException();
            }
            _form = instructions._form;
            Graphics g = _form.CreateGraphics();
            Width = _form.ClientSize.Width;
            Height = _form.ClientSize.Height;
            _context = BufferedGraphicsManager.Current;
            Buffer = _context.Allocate(g, new Rectangle(0, 0, Width, Height));

            _form.KeyDown += SceneKeyDown;
        }

        public virtual void SceneKeyDown(object sender, KeyEventArgs e) { }
        public virtual void Draw() { }
        public virtual void Update() { }

        public virtual void Dispose()
        {
            timer?.Stop();
            _form.KeyDown -= SceneKeyDown;
            Buffer = null;
            _context = null;
        }
    }


    public class SceneArgs
    {
        internal Form _form;
        internal InGame _mode;

        internal SceneArgs(Form form) => _form = form;
        internal SceneArgs(Form form, InGame mode) : this(form) => _mode = mode;
    }


    public interface IScene
    {
        void Init(SceneArgs e);
        void Draw();
    }
}
