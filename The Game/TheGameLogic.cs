using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
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

        /*
        // static List<Celestial> _celestial_bodies; // нарушает порядок Draw()
        static List<Celestial>[] allobjects;
        static List<Celestial> _asteroids;
        static List<Celestial> _comets;
        static List<Celestial> _stars;
        static List<Celestial> planet;
        static List<Celestial> missle;
        */
        static Celestial[] _asteroids;
        static Celestial[] _comets;
        static Celestial[] _stars;
        static Planet planet;
        static Missle missle;

        public static int Width { get; set; }
        public static int Height { get; set; }
        #endregion

        #region ЗАПУСК ЯДРА
        public static void Init(Form form)
        {
            SetWindow(form);
            rand = new Random();
            timer = new Timer();

            //Missle.pen.Width = 2.0F;

            Load();

            timer.Interval = 36;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public static void SetWindow(Form form)
        {
            if (form.ClientSize.Width < 0 || 1000 < form.ClientSize.Width || form.ClientSize.Height < 0 || 1000 < form.ClientSize.Height)
            {
                throw new ArgumentOutOfRangeException();
            }

            _context = BufferedGraphicsManager.Current;
            Graphics g = form.CreateGraphics();
            Width = form.ClientSize.Width;
            Height = form.ClientSize.Height;
            Buffer = _context.Allocate(g, new Rectangle(0, 0, Width, Height));
            Buffer.Graphics.PageUnit = GraphicsUnit.Pixel;
        }
        #endregion


        #region ГЛАВНЫЙ ЦИКЛ
        private static void Timer_Tick(object sender, EventArgs e)
        {
            Update();
            Draw();
        }

        public static void Draw()
        {
            Buffer.Graphics.Clear(Color.Black);

            if (Properties.Resources.background != null)
                Buffer.Graphics.DrawImage(Properties.Resources.background, 0, 0, Width, Height);

            Celestial.Draw(_stars);
            Celestial.Draw(planet);
            Celestial.Draw(_asteroids);
            Celestial.Draw(_comets);
            Celestial.Draw(missle);

            Buffer.Render();
        }

        public static void Update()
        {
            UpdateStars();

            Celestial.Update(missle);
            if (missle.OutOfView() || missle.StandsStill())
                missle = MakeMissle();

            UpdateWithHit(_asteroids, SystemSounds.Hand);
            //CheckAsteroidsOnCollisions();

            UpdateWithHit(_comets, SystemSounds.Exclamation);
            UpdateComets();
        }

        public static void UpdateWithHit(Celestial[] objset, SystemSound sound)
        {
            foreach (var one in objset)
            {
                one.Update();
                if (one.CollidesWith(missle))
                {
                    sound.Play();
                    one.Hit();
                }
            }
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

        public static void UpdateComets()
        {
            var comets = new List<Celestial>();
            foreach (Comet comet in _comets)
            {
                if (comet.Empty())
                    comets.Add(MakeComet());
                else
                    comets.Add(comet);
            }
            _comets = comets.ToArray();
        }

        public static void CheckAsteroidsOnCollisions()
        {
            for (int i = 0;  i < _asteroids.Length - 1;  i++)
            {
                for (int j = i + 1;  j < _asteroids.Length;  j++)
                {
                    if (_asteroids[i].CollidesWith(_asteroids[j]))
                    {
                        //System.Media.SystemSounds.Hand.Play(); // не при данном применении звука // TODO
                        _asteroids[i].Bump();
                        _asteroids[j].Bump();
                    }
                }
            }
        }
        #endregion


        #region ИНИЦИАЛИЗАЦИЯ ОБЪЕКТОВ В ИГРЕ
        public static void Load()
        {
            /*
            allobjects = new List<Celestial>[5];
            _stars = allobjects[0] = new List<Celestial>();
            planet = allobjects[1] = new List<Celestial>();
            _asteroids = allobjects[2] = new List<Celestial>();
            _comets = allobjects[3] = new List<Celestial>();
            missle = allobjects[4] = new List<Celestial>();
            // так Celestial.Draw(allobjects) должна сохранять порядок прорисовки
            */

            planet = new Planet(100, 100, 200, 200);
            missle = MakeMissle();
            LoadAsteroids(12);
            LoadStars(25);
            LoadComets(2);
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

        private static Missle MakeMissle()
        {
            return new Missle(
                new Point(planet.Pos.X + planet.Size.Width / 2, planet.Pos.Y + planet.Size.Height / 2),
                new Point(rand.Next(-20, 20), rand.Next(-20, 20)),
                new Size(rand.Next(9, 12), rand.Next(9, 12))
            );
            /*
            return new Missle(
                new Point(0, rand.Next(50, Height - 50)),
                new Point(rand.Next(5, 15), rand.Next(-30, 30)),
                new Size(rand.Next(6, 12), rand.Next(6, 12))
            ); */
        }
        #endregion
    }
}
