using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using The_Game.Properties;

namespace The_Game
{
    class GameLogic
    {
        #region ПОЛЯ И СВОЙСТВА
        private static BufferedGraphicsContext _context;
        public static BufferedGraphics Buffer;

        public static Random rand;
        public static Timer timer;

        // static List<Celestial> _celestial_bodies; // нарушает порядок Draw()
        static Celestial[] _asteroids;
        static Celestial[] _comets;
        static Celestial[] _stars;
        static Planet planet;

        public static int Width { get; set; }
        public static int Height { get; set; }
        #endregion

        public static void Init(Form form)
        {
            _context = BufferedGraphicsManager.Current;
            Graphics g = form.CreateGraphics();
            rand = new Random();
            timer = new Timer();

            Width = form.ClientSize.Width;
            Height = form.ClientSize.Height;
            Buffer = _context.Allocate(g, new Rectangle(0, 0, Width, Height));

            Load();

            timer.Interval = 36;
            timer.Tick += Timer_Tick;
            timer.Start();
        }


        #region ГЛАВНЫЙ ЦИКЛ
        private static void Timer_Tick(object sender, EventArgs e)
        {
            Update();
            Draw();
        }

        public static void Draw()
        {
            Buffer.Graphics.Clear(Color.Black);

            try
            {
                Buffer.Graphics.DrawImage(Properties.Resources.background, 0, 0, Width, Height);
            }
            catch
            {
                // Buffer.Graphics.FillRectangle(Brushes.Blue, new Rectangle(0, 0, Width, Height));
            }

            Celestial.Draw(_stars);
            Celestial.Draw(planet);
            Celestial.Draw(_asteroids);
            Celestial.Draw(_comets);

            Buffer.Render();
        }

        public static void Update()
        {
            UpdateStars();
            // Celestial.Update(planet);  // не происходит изменений, TODO?
            Celestial.Update(_asteroids);
            Celestial.Update(_comets);
        }

        public static void UpdateStars()
        {
            var stars = new List<Celestial>();
            foreach (Star star in _stars)
            {
                star.Update();
                if (star.OutOfView() || star.StandsStill())
                    stars.Add(MakeStar());
                else
                    stars.Add(star);
            }
            _stars = stars.ToArray();
        }
        #endregion


        #region ИНИЦИАЛИЗАЦИЯ ОБЪЕКТОВ В ИГРЕ
        public static void Load()
        {
            planet = new Planet(100, 100, 200, 200);
            // _celestial_bodies = new List<Celestial>();
            LoadAsteroids(12);
            LoadStars(25);
            LoadComets(2); // сильно нагружают
        }

        private static void LoadAsteroids(int number)
        {
            _asteroids = new Celestial[number];
            for (int i = 0; i < _asteroids.Length; i++)
                _asteroids[i] = MakeAsteroid();
        }
        private static void LoadStars(int number)
        {
            _stars = new Celestial[number];
            for (int i = 0; i < _stars.Length; i++)
                _stars[i] = MakeStar();
        }
        private static void LoadComets(int number)
        {
            _comets = new Celestial[number];
            for (int i = 0; i < _comets.Length; i++)
            {
                _comets[i] = MakeComet();
            }
        }
        #endregion

        #region СОЗДАНИЕ ОБЪЕКТОВ В ИГРЕ, границы значений
        private static Asteroid MakeAsteroid()
        {
            int size = rand.Next(10, 40);
            return new Asteroid(
                new Point(rand.Next(10, Width - size - 10), rand.Next(10, Height - size - 10)),
                new Point(rand.Next(-15, 15), rand.Next(-15, 15)),
                new Size(size, size)
            );
        }

        private static Star MakeStar()
        {
            int size = rand.Next(1, 5),
                x = rand.Next(Width * 2 / 5, Width * 3 / 5),
                y = rand.Next(Height * 2 / 5, Height * 3 / 5);

            return new Star(
                new Point(x, y),
                new Point((x - Width / 2) / 3, (y - Height / 2) / 4),
                new Size(size, size)
            );
        }

        private static Comet MakeComet()
        {
            int dirX, dirY,
                size = rand.Next(10, 20),
                x = rand.Next(10, Width - size - 10),
                y = rand.Next(10, Height - size - 10);

            dirX = rand.Next(-2 * size, 2 * size);
            if (dirX < -size || dirX > size)
                dirY = rand.Next(-2 * size, 2 * size);
            else
            {
                dirY = rand.Next(size, size * 2);
                if (rand.Next(1) == 0)
                    dirY = -dirY;
            }

            return new Comet(new Point(x, y), new Point(dirX, dirY), new Size(size, size));
        }
        #endregion
    }
}
