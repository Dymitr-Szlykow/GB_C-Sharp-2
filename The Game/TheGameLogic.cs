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

        public static Random rand = new Random();
        public static Timer timer = new Timer();

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
        static Ship ship;
        static Missle missle;
        static Missle missle_pl;
        //static List<Celestial> _missles;

        static int score_overall = 0;
        static int score_overall_pl = 0;
        static int asteroidsDestroyed = 0;
        static int cometsDestroyed = 0;

        public static int Width { get; set; }
        public static int Height { get; set; }
        #endregion

        #region ЗАПУСК ЯДРА
        public static void Init(Form form)
        {
            SetWindow(form);
            Load();

            timer.Tick += Timer_Tick;
            form.KeyDown += OnKeyDown;

            timer.Interval = 60;
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

        private static void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up) ship.MoveUp();
            else if (e.KeyCode == Keys.Down) ship.MoveDown();
            else if (e.KeyCode == Keys.Left) ship.MoveLeft();
            else if (e.KeyCode == Keys.Right) ship.MoveRight();
            else if (e.KeyCode == Keys.ControlKey && missle == null) missle = ShootMissle(); //_missles.Add(ShootMissle());
        }
        #endregion


        #region ГЛАВНЫЙ ЦИКЛ
        private static void Timer_Tick(object sender, EventArgs e)
        {
            Update();
            Draw();
        }

        #region главный цикл: ПРОРИСОВКА
        public static void Draw()
        {
            Buffer.Graphics.Clear(Color.Black);
            if (Properties.Resources.background != null)
                Buffer.Graphics.DrawImage(Properties.Resources.background, 0, 0, Width, Height);

            Celestial.Draw(_stars);
            Celestial.Draw(planet);
            Celestial.Draw(_asteroids);
            Celestial.Draw(_comets);

            if (missle != null) Celestial.Draw(missle);
            if (missle_pl != null) Celestial.Draw(missle_pl);
            //if (_missles != null) Celestial.Draw(_missles);

            if (ship != null)
            {
                Celestial.Draw(ship);
                Buffer.Graphics.DrawString($"Энергия: {ship.Energy}", SystemFonts.DefaultFont, Brushes.White, new Point(Width - 100, 1));
                Buffer.Graphics.DrawString($"Очки ваши: {score_overall}", SystemFonts.DefaultFont, Brushes.White, new Point(Width - 100, 15));
                Buffer.Graphics.DrawString($"Очки ПВО: {score_overall_pl}", SystemFonts.DefaultFont, Brushes.White, new Point(Width - 100, 29));
            }

            Buffer.Render();
        }
        #endregion

        #region главный цикл: ОБНОВЛЕНИЕ
        public static void Update() // TODO, полна мусора
        {
            UpdateStars();
            planet.Update();
            if (planet.Pos.X < -planet.Size.Width / 2)
            {
                planet.ReachesEdge();
            }

            UpdateAsteroids_questionable(new Celestial[0], SystemSounds.Hand); // TODO

            //UpdateMissles();
            if (missle != null)
            {
                missle.Update();
                if (missle.OutOfView() || missle.StandsStill())
                    missle = null;
            }

            if (missle_pl != null)
            {
                missle_pl.Update();
                if (missle_pl.OutOfView() || missle_pl.StandsStill())
                    missle_pl = MakeMissle_FromPlanet();
            }

            //UpdateWithHit(_asteroids, SystemSounds.Hand, ref asteroidsDestroyed);
            //CheckAsteroidsOnCollisions();

            UpdateWithHit(_comets, SystemSounds.Exclamation, ref cometsDestroyed);
            UpdateCometList();
        }

        public static void UpdateWithHit(Celestial[] objset, SystemSound sound, ref int count)
        {
            foreach (var one in objset)
            {
                one.Update();
                if (missle != null && one.CollidesWith(missle))
                {
                    sound.Play();
                    one.Hit();
                    missle = null;
                    count++;
                    score_overall += one.ScoreCost;
                }
            }
        }

        public static void UpdateStars() // +
        {
            var stars = new List<Celestial>();
            foreach (Star one in _stars)
            {
                one.Update();
                if (one.OutOfView() || one.StandsStill())
                    stars.Add(MakeStar_Scroller());
                //stars.Add(MakeStar());
                else
                    stars.Add(one);
            }
            _stars = stars.ToArray();
        }

        public static void UpdateAsteroids_questionable(Celestial[] objset, SystemSound sound)  // TODO, полна мусора
        {
            for (int i = 0; i < _asteroids.Length; i++)
            {
                if (_asteroids[i] == null) continue;

                _asteroids[i].Update();

                if (missle != null && _asteroids[i].CollidesWith(missle))
                {
                    sound.Play();
                    missle = null;
                    _asteroids[i] = MakeAsteroid_FromEdges();
                    asteroidsDestroyed++;
                    score_overall += _asteroids[i].ScoreCost;
                    continue;
                }

                if (missle_pl != null && _asteroids[i].CollidesWith(missle_pl))
                {
                    sound.Play();
                    missle_pl = MakeMissle_FromPlanet();
                    _asteroids[i] = MakeAsteroid_FromEdges();
                    score_overall_pl += _asteroids[i].ScoreCost;
                    continue;
                }

                if (ship != null && _asteroids[i].CollidesWith(ship))
                {
                    SystemSounds.Beep.Play(); //sound.Play();
                    // _asteroids[i].Bump(ship);
                    ship.Hit(rand.Next(1, 3));
                    if (ship.Energy < 0) ship.Dies();
                }
            }
        }

        public static void UpdateCometList()  // +
        {
            var comets = new List<Celestial>();
            foreach (Comet one in _comets)
            {
                if (one.Empty())
                    comets.Add(MakeComet_randomplace());
                else
                    comets.Add(one);
            }
            _comets = comets.ToArray();
        }

        //public static void UpdateMissles()
        //{
        //    if (_missles != null && _missles.Count != 0)
        //    {
        //        var missles = new List<Celestial>();
        //        foreach (Missle one in _missles)
        //        {
        //            one.Update();
        //            if (!one.OutOfView() && !one.StandsStill())
        //                missles.Add(one);
        //        }
        //        _missles = missles;
        //    }
        //}

        public static void CheckAsteroidsOnCollisions()
        {
            for (int i = 0;  i < _asteroids.Length - 1;  i++)
            {
                for (int j = i + 1;  j < _asteroids.Length;  j++)
                {
                    if (_asteroids[i].CollidesWith(_asteroids[j]))
                    {
                        //System.Media.SystemSounds.Hand.Play(); // не при данном применении звука // TODO
                        _asteroids[i].Bump(_asteroids[j]);
                    }
                }
            }
        }
        #endregion
        #endregion

        #region МАКРОСОБЫТИЯ
        private static void OnDeath(object sender, DeathEventArgs e)
        {
            timer.Stop();
            Buffer.Graphics.DrawString("Game Over!", new Font(FontFamily.GenericSansSerif, 60, FontStyle.Underline), Brushes.Red, 100, 70);
            Buffer.Graphics.DrawString
            (
                $"Ваши очки: {score_overall}" +
                $"\n(сбито: астероидов {asteroidsDestroyed}, комет {cometsDestroyed})" +
                $"\nОчки Противо-космической обороны: {score_overall_pl}",
                new Font(FontFamily.GenericSansSerif, 40, FontStyle.Underline), Brushes.Red, 50, 150
            );
            Buffer.Render();
        }

        private static void OnReachingEdge(object sender, EventArgs e)
        {
            timer.Stop();
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

            planet = new Planet(Width - 150, 70, 200, 200);
            planet.ReachingEdge += OnReachingEdge;

            ship = new Ship(new Point(10, Height / 2 - 30), new Point(9, 15), new Size(45, 60));
            ship.Death += OnDeath;

            missle_pl = MakeMissle_FromPlanet();
            LoadAsteroids(12);
            LoadStars(25);
            LoadComets(2);
        }

        private static void LoadAsteroids(int number)
        {
            _asteroids = new Celestial[number];
            for (int i = 0; i < _asteroids.Length; i++)
                _asteroids[i] = MakeAsteroid_Randomplace();
        }
        private static void LoadStars(int number)
        {
            _stars = new Celestial[number];
            for (int i = 0; i < _stars.Length; i++)
            {
                _stars[i] = MakeStar_FromCenter();
                // _stars[i] = MakeStar_Scroller();
            }
        }
        private static void LoadComets(int number)
        {
            _comets = new Celestial[number];
            for (int i = 0; i < _comets.Length; i++)
            {
                _comets[i] = MakeComet_randomplace();
            }
        }
        #endregion

        #region СОЗДАНИЕ ОБЪЕКТОВ В ИГРЕ, границы значений
        private static Asteroid MakeAsteroid_Randomplace()
        {
            int size = rand.Next(10, 40);
            return new Asteroid(
                new Point(rand.Next(10, Width - size - 10), rand.Next(10, Height - size - 10)),
                new Point(rand.Next(-8, 8), rand.Next(-8, 8)),
                new Size(size, size)
            );
        }

        private static Asteroid MakeAsteroid_FromEdges()
        {
            int x, y, size = rand.Next(10, 40);
            switch (rand.Next(3))
            {
                case 0:
                    x = -size;
                    y = rand.Next(-size, Height);
                    break;
                case 1:
                    x = Width;
                    y = rand.Next(-size, Height);
                    break;
                case 2:
                    x = rand.Next(-size, Width);
                    y = -size;
                    break;
                case 3:
                    x = rand.Next(-size, Width);
                    y = Height;
                    break;
                default:
                    x = 0;
                    y = 0;
                    break;
            }
            return new Asteroid(
                new Point(x, y),
                new Point(rand.Next(-8, 8), rand.Next(-8, 8)),
                new Size(size, size)
            );
        }

        private static Star MakeStar_FromCenter()
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
        private static Star MakeStar_Scroller()
        {
            int astray, size = rand.Next(1, 5);
            if (rand.Next(3) == 0)
            {
                astray = rand.Next(-3, 3);
            }
            else
                astray = 0;

            return new Star(
                new Point(Width, rand.Next(5, Height - size - 5)),
                new Point(rand.Next(-10, -5), astray),
                new Size(size, size)
            );
        }

        private static Comet MakeComet_randomplace()
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

        private static Missle MakeMissle_FromPlanet()
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

        private static Missle ShootMissle()
        {
            return new Missle(new Point(ship.Pos.X + ship.Size.Width, ship.Pos.Y + ship.Size.Height / 2), new Point(20, 0), new Size(27, 5));
        }
        #endregion
    }
}
