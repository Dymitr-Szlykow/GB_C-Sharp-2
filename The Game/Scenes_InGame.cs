using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using The_Game.Properties;

namespace The_Game
{
    class InGame : BaseScene
    {
        #region ПОЛЯ И СВОЙСТВА
        public static Random rand = new Random();

        protected delegate void MainCycle();
        protected static MainCycle mainCycle;

        static PackedObjects _o;
        static List<Celestial> _asteroids;
        static List<Celestial> _comets;
        static List<Celestial> _kits;
        static List<Celestial> _missles;
        static List<Celestial> _stars;
        static Missle missle_pl;
        static Planet planet;
        static Ship ship;

        static PackedStatistics _stat;
        static int _countdown;
        static int score_overall;
        static int score_overall_pl;
        static int asteroidsDestroyed;
        static int cometsDestroyed;

        class PackedObjects
        {
            static List<Celestial> asteroids;
            static List<Celestial> comets;
            static List<Celestial> kits;
            static List<Celestial> missles;
            static List<Celestial> stars;
            static Missle missle_pl;
            static Planet planet;
            static Ship ship;
        }

        class PackedStatistics
        {
            static int score_overall;
            static int score_overall_pl;
            static int asteroidsDestroyed;
            static int cometsDestroyed;
        }
        #endregion

        #region ЗАПУСК ЯДРА
        public override void Init(Form form)
        {
            base.Init(form);

            _countdown = 0;
            score_overall = 0;
            score_overall_pl = 0;
            asteroidsDestroyed = 0;
            cometsDestroyed = 0;

            Load();
            mainCycle = CycleUpdate;
            mainCycle += CycleDraw;

            timer = new Timer();
            timer.Interval = 60;
            timer.Tick += OnTimerTick;
            timer.Start();
        }

        public override void SceneKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up) ship.MoveUp();
            else if (e.KeyCode == Keys.Down) ship.MoveDown();
            else if (e.KeyCode == Keys.Left) ship.MoveLeft();
            else if (e.KeyCode == Keys.Right) ship.MoveRight();
            else if (e.KeyCode == Keys.ControlKey && ship.MayShoot)
            {
                _missles.Add(ShootMissle());
                ship.Shot();
            }
            else if (e.KeyCode == Keys.Back) SceneManager.Boot().PrepareScene<Menu>(_form).Draw();
        }
        #endregion

        #region ЯДРО: УПРАВЛЕНИЕ ЦИКЛОМ
        protected static void OnTimerTick(object sender, EventArgs e)
        {
            if (_countdown == 0)
            {
                _kits.Add(MakeEngikit());
                _countdown = rand.Next(70, 150);
            }
            else
                _countdown--;

            mainCycle.Invoke();
        }

        protected static void OnDeath(object sender, DeathEventArgs e)
        {
            using (FileStream stream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "log.txt", FileMode.Append, FileAccess.Write))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine($"Игра окончена: {e._logMessage}. Послежний полученный ущерб: {e.lastDamage}.");
                }
            }

            // TODO
            Endgame();
        }

        protected static void OnReachingEdge(object sender, EndGameEventArgs e)
        {
            using (FileStream stream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "log.txt", FileMode.Append, FileAccess.Write))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine($"Игра окончена: {e._logMessage}.");
                }
            }

            // TODO
            Endgame();
        }

        protected static void Endgame()
        {
            mainCycle = CycleUpdate_endgame;
            mainCycle += CycleDraw_endgame;
        }
        #endregion

        #region ГЛАВНЫЙ ЦИКЛ: В ИГРЕ
        public static void CycleDraw()
        {
            Buffer.Graphics.Clear(Color.Black);
            if (Resources.background != null)
                Buffer.Graphics.DrawImage(Resources.background, 0, 0, Width, Height);

            Celestial.Draw(_stars, Buffer.Graphics);
            Celestial.Draw(planet, Buffer.Graphics);
            Celestial.Draw(_asteroids, Buffer.Graphics);
            Celestial.Draw(_comets, Buffer.Graphics);
            Celestial.Draw(_kits, Buffer.Graphics);
            Celestial.Draw(_missles, Buffer.Graphics);
            if (missle_pl != null) Celestial.Draw(missle_pl, Buffer.Graphics);

            if (ship != null)
            {
                Celestial.Draw(ship, Buffer.Graphics);
                Buffer.Graphics.DrawString($"Энергия: {ship.Energy}", SystemFonts.DefaultFont, Brushes.White, new Point(Width - 100, 1));
                Buffer.Graphics.DrawString($"Очки ваши: {score_overall}", SystemFonts.DefaultFont, Brushes.White, new Point(Width - 100, 15));
                Buffer.Graphics.DrawString($"Очки ПВО: {score_overall_pl}", SystemFonts.DefaultFont, Brushes.White, new Point(Width - 100, 29));
            }

            Buffer.Render();
        }

        public static void CycleUpdate()
        {
            UpdateStars(MakeStar_Scroller);
            planet.Update();
            ship.Update();

            UpdateMissles();
            UpdateTargets();

            CheckKits();

            if (planet.Pos.X < -planet.Size.Width / 2) planet.ReachesEdge();
        }

        public static void UpdateStars(Func<Celestial> creationMethod)
        {
            for (int i = _stars.Count - 1; i >= 0; i--)
            {
                _stars[i].Update();
                if (_stars[i].OutOfView() || _stars[i].StandsStill())
                {
                    _stars[i] = creationMethod.Invoke();
                }
            }
        }

        public static void UpdateMissles()
        {
            if (missle_pl != null)
            {
                missle_pl.Update();
                if (missle_pl.OutOfView() || missle_pl.StandsStill())
                    missle_pl = null;
            }
            else
                missle_pl = MakeMissle_FromPlanet();

            for (int i = _missles.Count - 1; i >= 0; i--)
            {
                _missles[i].Update();
                if (_missles[i].OutOfView())
                {
                    _missles.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Огромный неловкий метод, выполняющий обновление астероидов и комет и проверки на их столкновения
        /// </summary>
        public static void UpdateTargets()
        {
            for (int i = _asteroids.Count - 1; i >= 0; i--)
            {
                // о б н о в и т ь
                _asteroids[i].Update();

                // с б и т  и г р о к о м ?
                bool IsShot = false;
                for (int j = _missles.Count - 1; j >= 0; j--)
                {
                    if (_asteroids[i].CollidesWith(_missles[j]))
                    {
                        SystemSounds.Hand.Play();
                        IsShot = true;
                        asteroidsDestroyed++;
                        score_overall += _asteroids[i].ScoreCost;
                        _asteroids[i] = MakeAsteroid_FromEdges();
                        _missles.RemoveAt(j);
                        break;
                    }
                }
                if (IsShot) continue;

                // с б и т  с  п л а н е т ы ?
                if (missle_pl != null && _asteroids[i].CollidesWith(missle_pl))
                {
                    SystemSounds.Hand.Play();
                    score_overall_pl += _asteroids[i].ScoreCost;
                    _asteroids[i] = MakeAsteroid_FromEdges();
                    missle_pl = null;
                    continue;
                }

                // с т о л к н у л с я  с  к о р а б л е м ?
                if (_asteroids[i].CollidesWith(ship))
                {
                    SystemSounds.Beep.Play();
                    // _asteroids[i].Bump(ship);
                    ship.Hit(rand.Next(1, 3));
                    if (ship.Energy < 0) ship.Dies();
                    _asteroids[i] = MakeAsteroid_FromEdges();
                }

                // с т о л к н у л с я  с  д р у г и м  а с т е р о и д о м ?
                for (int j = _asteroids.Count - 1; j > i; j--)
                {
                    if (_asteroids[i].CollidesWith(_asteroids[j]))
                    {
                        //TODO
                    }
                }
            }
            while (_asteroids.Count < 12) _asteroids.Add(MakeAsteroid_FromEdges());

            for (int i = 0; i < _comets.Count; i++)
            {
                // о б н о в и т ь
                _comets[i].Update();
                if (_comets[i].IsEmpty())
                {
                    _comets.RemoveAt(i);
                    continue;
                }

                // с б и т а  и г р о к о м ?
                bool IsShot = false;
                for (int j = _missles.Count - 1; j >= 0; j--)
                {
                    if (_comets[i].CollidesWith(_missles[j]))
                    {
                        SystemSounds.Exclamation.Play();
                        IsShot = true;
                        cometsDestroyed++;
                        score_overall += _comets[i].ScoreCost;
                        _comets[i].Hit();
                        _missles.RemoveAt(j);
                        break;
                    }
                }
                if (IsShot) continue;

                // с б и т а  с  п л а н е т ы ?
                if (missle_pl != null && _comets[i].CollidesWith(missle_pl))
                {
                    SystemSounds.Hand.Play();
                    score_overall_pl += _comets[i].ScoreCost;
                    _comets[i].Hit();
                    missle_pl = null;
                    continue;
                }

                // с т о л к н у л а с ь  с  к о р а б л е м ?
                if (_comets[i].CollidesWith(ship))
                {
                    SystemSounds.Beep.Play(); //sound.Play();
                    if (_comets[i].Dir != Point.Empty) ship.Hit(rand.Next(5, 10));
                    if (ship.Energy < 0) ship.Dies();
                    _comets[i].Hit();
                }

                // с т о л к н у л а с ь  с  д р у г о й  к о м е т о й ?
                //for (int j = _comets.Count - 1; j > i; j--)
                //{
                //    if (_comets[i].CollidesWith(_asteroids[j]))
                //    {
                //        // TODO
                //    }
                //}

                // с т о л к н у л а с ь  с  а с т е р о и д о м ?
                for (int j = 0; j < _asteroids.Count; j++)
                {
                    if (_comets[i].CollidesWith(_asteroids[j]))
                    {
                        // TODO
                    }
                }
            }
            while (_comets.Count < 2) _comets.Add(MakeComet_FromEdges());
        }

        public static void CheckKits()
        {
            for (int i = _kits.Count -1; i >= 0; i--)
            {
                if (_kits[i].CollidesWith(ship))
                {
                    _kits[i].Picked(ship);
                    _kits.RemoveAt(i);
                }
            }
        }

        public static void CheckAsteroidsOnCollisions()
        {
            for (int i = 0;  i < _asteroids.Count - 1;  i++)
            {
                for (int j = i + 1;  j < _asteroids.Count;  j++)
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
        
        #region ГЛАВНЫЙ ЦИКЛ: КОНЕЦ ИГРЫ
        public static void CycleDraw_endgame()
        {
            Buffer.Graphics.Clear(Color.Black);
            if (Properties.Resources.background != null)
                Buffer.Graphics.DrawImage(Properties.Resources.background, 0, 0, Width, Height);

            Celestial.Draw(_stars, Buffer.Graphics);
            Celestial.Draw(planet, Buffer.Graphics);

            if (ship != null)
            {
                if (ship.Energy > 0) Celestial.Draw(ship, Buffer.Graphics);
                Buffer.Graphics.DrawString($"Энергия: {ship.Energy}", SystemFonts.DefaultFont, Brushes.White, new Point(Width - 100, 1));
                Buffer.Graphics.DrawString($"Очки ваши: {score_overall}", SystemFonts.DefaultFont, Brushes.White, new Point(Width - 100, 15));
                Buffer.Graphics.DrawString($"Очки ПКПО: {score_overall_pl}", SystemFonts.DefaultFont, Brushes.White, new Point(Width - 100, 29));
            }

            // Buffer.Graphics.MeasureString("Конец!", new Font(FontFamily.GenericSansSerif, 60, FontStyle.Underline)).ToSize(); // TODO
            Buffer.Graphics.DrawString("Конец!", new Font(FontFamily.GenericSansSerif, 60, FontStyle.Underline), Brushes.OrangeRed, Width / 2 - 160, 100);
            Buffer.Graphics.DrawString
            (
                $"Ваши очки: {score_overall}" +
                $"\t\tсбито астероидов: {asteroidsDestroyed}\n\t\t\tсбито комет: {cometsDestroyed}" +
                $"\nОчки Противо-космической планетарной обороны: {score_overall_pl}",
                new Font(FontFamily.GenericSansSerif, 15, FontStyle.Regular), Brushes.MediumTurquoise, 120, 260
            );
            Buffer.Render();
        }

        public static void CycleUpdate_endgame()
        {
            UpdateStars(MakeStar_FromCenter);
        }
        #endregion

        #region ИНИЦИАЛИЗАЦИЯ ОБЪЕКТОВ В ИГРЕ
        public static void Load()
        {
            planet = new Planet(Width - 150, 70, 200, 200);
            planet.ReachingEdge += OnReachingEdge;

            ship = new Ship(new Point(10, Height / 2 - 30), new Point(12, 15), new Size(45, 60));
            ship.Death += OnDeath;

            missle_pl = MakeMissle_FromPlanet();

            _asteroids = InitObjList(12, MakeAsteroid_Randomplace);
            _comets = InitObjList(2, MakeComet_Randomplace);
            _stars = InitObjList(25, MakeStar_FromCenter);
            _kits = new List<Celestial>();
            _missles = new List<Celestial>();
        }

        //public delegate TResult MakeObj<TResult>();

        public static Celestial[] InitObjArray (int number, Func<Celestial> creationMethod)
        {
            var res = new Celestial[number];
            for (int i = 0; i < res.Length; i++)
                res[i] = creationMethod.Invoke();
            return res;
        }

        public static List<Celestial> InitObjList(int number, Func<Celestial> creationMethod)
        {
            var res = new List<Celestial>();
            for (int i = 0; i < number; i++)
                res.Add(creationMethod.Invoke());
            return res;
        }
        #endregion

        #region СОЗДАНИЕ ОБЪЕКТОВ В ИГРЕ, границы значений
        public static Asteroid MakeAsteroid_Randomplace()
        {
            int size = rand.Next(10, 40);
            return new Asteroid(
                new Point(rand.Next(10, Width - size - 10), rand.Next(10, Height - size - 10)),
                new Point(rand.Next(-8, 8), rand.Next(-8, 8)),
                new Size(size, size)
            );
        }

        public static Asteroid MakeAsteroid_FromEdges()
        {
            int x, y, dirX, dirY, size = rand.Next(10, 40);
            switch (rand.Next(1,3))
            {
                //case 0:
                //    x = -size;
                //    y = rand.Next(-size, Height);
                //    dirX = rand.Next(3, 8);
                //    dirY = rand.Next(-8, 8);
                //    break;
                case 1:
                    x = Width;
                    y = rand.Next(-size, Height);
                    dirX = rand.Next(-8, -3);
                    dirY = rand.Next(-8, 8);
                    break;
                case 2:
                    x = rand.Next(-size, Width);
                    y = -size;
                    dirX = rand.Next(-8, 8);
                    dirY = rand.Next(3, 8);
                    break;
                case 3:
                    x = rand.Next(-size, Width);
                    y = Height;
                    dirX = rand.Next(-8, -8);
                    dirY = rand.Next(-8, -3);
                    break;
                default:
                    x = 0;
                    y = 0;
                    dirX = rand.Next(-8, 8);
                    dirY = rand.Next(-8, 8);
                    break;
            }
            return new Asteroid(new Point(x, y), new Point(dirX, dirY), new Size(size, size));
        }

        public static Comet MakeComet_Randomplace()
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

        public static Comet MakeComet_FromEdges()
        {
            int x, y, dirX, dirY,
                size = rand.Next(10, 20);
            switch (rand.Next(1,3))
            {
                //case 0:
                //    x = -size;
                //    y = rand.Next(-size, Height);
                //    dirX = rand.Next(size, 2 * size);
                //    dirY = rand.Next(-2 * size, 2 * size);
                //    break;
                case 1:
                    x = Width;
                    y = rand.Next(-size, Height);
                    dirX = rand.Next(-2 * size, -size);
                    dirY = rand.Next(-2 * size, 2 * size);
                    break;
                case 2:
                    x = rand.Next(-size, Width);
                    y = -size;
                    dirX = rand.Next(-2 * size, 2 * size);
                    dirY = rand.Next(size, 2 * size);
                    break;
                case 3:
                    x = rand.Next(-size, Width);
                    y = Height;
                    dirX = rand.Next(-2 * size, 2 * size);
                    dirY = rand.Next(-size, -2 * size);
                    break;
                default:
                    x = 0;
                    y = 0;
                    break;
            }
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

        public static Star MakeStar_FromCenter()
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

        public static Star MakeStar_Scroller()
        {
            int astray,
                size = rand.Next(1, 5);
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

        public static Missle MakeMissle_FromPlanet()
        {
            return new Missle(
                new Point(planet.Pos.X + planet.Size.Width / 2, planet.Pos.Y + planet.Size.Height / 2),
                new Point(rand.Next(-20, 20), rand.Next(-20, 20)),
                new Size(rand.Next(9, 12), rand.Next(9, 12))
            );
        }

        public static Missle ShootMissle()
        {
            return new Missle(new Point(ship.Pos.X + ship.Size.Width, ship.Pos.Y + ship.Size.Height / 2), new Point(20, 0), new Size(27, 5));
        }

        public static EngiKit MakeEngikit()
        {
            if (rand.Next(4) == 0)
                return MakeEngikit_big();
            else
                return MakeEngikit_small();
        }

        public static EngiKit MakeEngikit_small()
        {
            return new EngiKit(new Point(rand.Next(5, Width / 3 - 15), rand.Next(5, Height - 5)), new Size(15, 15), 3);
        }

        public static EngiKit MakeEngikit_big()
        {
            return new EngiKit(new Point(rand.Next(5, Width / 3 - 15), rand.Next(5, Height / 3 - 15)), new Size(18, 18), 10);
        }
        #endregion
    }
}
