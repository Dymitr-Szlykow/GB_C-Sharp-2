using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace The_Game
{
    class InGame : BaseScene
    {
        #region ПОЛЯ И СВОЙСТВА
        public static Random rand = new Random();

        protected static PackedObjects _coel;
        protected static PackedStatistics _stat;
        protected static int _countdown;
        private static InGame _mode;

        public class PackedObjects
        {
            public List<Celestial> asteroids;
            public List<Celestial> comets;
            public List<Celestial> kits;
            public List<Celestial> missles;
            public List<Celestial> stars;
            public Missle missle_pl;
            public Planet planet;
            public Ship ship;
        }
        public class PackedStatistics
        {
            public int score_overall = 0;
            public int score_overall_pl = 0;
            public int asteroidsDestroyed = 0;
            public int cometsDestroyed = 0;
        }
        #endregion

        #region ЗАПУСК ЯДРА
        public override void Init(SceneArgs instructions)
        {
            if (instructions._mode == null) throw new Exception("Не указан режим при запуске игры.");
            base.Init(instructions);
            _mode = instructions._mode;

            _countdown = 0;
            _stat = new PackedStatistics();
            _coel = _mode.PopulateIngameObjectsPack();

            timer = new Timer();
            timer.Interval = 60;
            timer.Tick += Cycle_OnTimerTick;
            timer.Start();
        }

        public override void SceneKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up) _coel.ship.MoveUp();
            else if (e.KeyCode == Keys.Down) _coel.ship.MoveDown();
            else if (e.KeyCode == Keys.Left) _coel.ship.MoveLeft();
            else if (e.KeyCode == Keys.Right) _coel.ship.MoveRight();
            else if (e.KeyCode == Keys.ControlKey && _coel.ship.MayShoot)
            {
                _coel.missles.Add(ShootMissle());
                _coel.ship.Shot();
            }
            else if (e.KeyCode == Keys.Back)
            {
                //Dispose();
                SceneManager.Boot().PrepareScene<Menu>(new SceneArgs(_form)).Draw();
            }
        }
        #endregion

        #region ЯДРО: УПРАВЛЕНИЕ ЦИКЛОМ
        public override void Draw() => _mode.Draw();
        public override void Update() => _mode.Update();
        public virtual PackedObjects PopulateIngameObjectsPack() => new PackedObjects();
        protected static void Cycle_OnTimerTick(object sender, EventArgs e)
        {
            if (_countdown == 0)
            {
                _coel.kits.Add(MakeEngikit());
                _countdown = rand.Next(70, 150);
            }
            else
                _countdown--;

            _mode.Draw();
            _mode.Update();
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
            _mode = new Endgame();
        }
        #endregion

        #region главный цикл: ДЕТАЛИ ОБНОВЛЕНИЯ  -- TODO
        public static void UpdateStars(Func<Celestial> creationMethod)
        {
            for (int i = _coel.stars.Count - 1; i >= 0; i--)
            {
                _coel.stars[i].Update();
                if (_coel.stars[i].OutOfView() || _coel.stars[i].StandsStill())
                {
                    _coel.stars[i] = creationMethod.Invoke();
                }
            }
        }
        public static void UpdateStars(ref List<Celestial> these, Func<Celestial> creationMethod)
        {
            for (int i = these.Count - 1; i >= 0; i--)
            {
                these[i].Update();
                if (these[i].OutOfView() || these[i].StandsStill())
                {
                    these[i] = creationMethod.Invoke();
                }
            }
        }

        public static void UpdateMissles()
        {
            if (_coel.missle_pl != null)
            {
                _coel.missle_pl.Update();
                if (_coel.missle_pl.OutOfView() || _coel.missle_pl.StandsStill())
                    _coel.missle_pl = null;
            }
            else
                _coel.missle_pl = MakeMissle_FromPlanet(_coel.planet);

            for (int i = _coel.missles.Count - 1; i >= 0; i--)
            {
                _coel.missles[i].Update();
                if (_coel.missles[i].OutOfView())
                {
                    _coel.missles.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Огромный неловкий метод, выполняющий обновление астероидов и комет и проверки на их столкновения
        /// </summary>
        public static void UpdateTargets()
        {
            for (int i = _coel.asteroids.Count - 1; i >= 0; i--)
            {
                // о б н о в и т ь
                _coel.asteroids[i].Update();

                // с б и т  и г р о к о м ?
                bool IsShot = false;
                for (int j = _coel.missles.Count - 1; j >= 0; j--)
                {
                    if (_coel.asteroids[i].CollidesWith(_coel.missles[j]))
                    {
                        SystemSounds.Hand.Play();
                        IsShot = true;
                        _stat.asteroidsDestroyed++;
                        _stat.score_overall += _coel.asteroids[i].ScoreCost;
                        _coel.asteroids[i] = MakeAsteroid_FromEdges();
                        _coel.missles.RemoveAt(j);
                        break;
                    }
                }
                if (IsShot) continue;

                // с б и т  с  п л а н е т ы ?
                if (_coel.missle_pl != null && _coel.asteroids[i].CollidesWith(_coel.missle_pl))
                {
                    SystemSounds.Hand.Play();
                    _stat.score_overall_pl += _coel.asteroids[i].ScoreCost;
                    _coel.asteroids[i] = MakeAsteroid_FromEdges();
                    _coel.missle_pl = null;
                    continue;
                }

                // с т о л к н у л с я  с  к о р а б л е м ?
                if (_coel.asteroids[i].CollidesWith(_coel.ship))
                {
                    SystemSounds.Beep.Play();
                    // _coel.asteroids[i].Bump(ship);
                    _coel.ship.Hit(rand.Next(1, 3));
                    if (_coel.ship.Energy < 0) _coel.ship.Dies();
                    _coel.asteroids[i] = MakeAsteroid_FromEdges();
                }

                // с т о л к н у л с я  с  д р у г и м  а с т е р о и д о м ?
                for (int j = _coel.asteroids.Count - 1; j > i; j--)
                {
                    if (_coel.asteroids[i].CollidesWith(_coel.asteroids[j]))
                    {
                        //TODO
                    }
                }
            }
            while (_coel.asteroids.Count < 12) _coel.asteroids.Add(MakeAsteroid_FromEdges());

            for (int i = 0; i < _coel.comets.Count; i++)
            {
                // о б н о в и т ь
                _coel.comets[i].Update();
                if (_coel.comets[i].IsEmpty())
                {
                    _coel.comets.RemoveAt(i);
                    continue;
                }

                // с б и т а  и г р о к о м ?
                bool IsShot = false;
                for (int j = _coel.missles.Count - 1; j >= 0; j--)
                {
                    if (_coel.comets[i].CollidesWith(_coel.missles[j]))
                    {
                        SystemSounds.Exclamation.Play();
                        IsShot = true;
                        _stat.cometsDestroyed++;
                        _stat.score_overall += _coel.comets[i].ScoreCost;
                        _coel.comets[i].Hit();
                        _coel.missles.RemoveAt(j);
                        break;
                    }
                }
                if (IsShot) continue;

                // с б и т а  с  п л а н е т ы ?
                if (_coel.missle_pl != null && _coel.comets[i].CollidesWith(_coel.missle_pl))
                {
                    SystemSounds.Hand.Play();
                    _stat.score_overall_pl += _coel.comets[i].ScoreCost;
                    _coel.comets[i].Hit();
                    _coel.missle_pl = null;
                    continue;
                }

                // с т о л к н у л а с ь  с  к о р а б л е м ?
                if (_coel.comets[i].CollidesWith(_coel.ship))
                {
                    SystemSounds.Beep.Play();
                    if (_coel.comets[i].Dir != Point.Empty) _coel.ship.Hit(rand.Next(5, 10));
                    if (_coel.ship.Energy < 0) _coel.ship.Dies();
                    _coel.comets[i].Hit();
                }

                // с т о л к н у л а с ь  с  д р у г о й  к о м е т о й ?
                for (int j = _coel.comets.Count - 1; j > i; j--)
                {
                    if (_coel.comets[i].CollidesWith(_coel.asteroids[j]))
                    {
                        // TODO
                    }
                }

                // с т о л к н у л а с ь  с  а с т е р о и д о м ?
                for (int j = 0; j < _coel.asteroids.Count; j++)
                {
                    if (_coel.comets[i].CollidesWith(_coel.asteroids[j]))
                    {
                        // TODO
                    }
                }
            }
            while (_coel.comets.Count < 2) _coel.comets.Add(MakeComet_FromEdges());
        }

        public static void CheckKits()
        {
            for (int i = _coel.kits.Count - 1; i >= 0; i--)
            {
                if (_coel.kits[i].CollidesWith(_coel.ship))
                {
                    _coel.kits[i].Picked(_coel.ship);
                    _coel.kits.RemoveAt(i);
                }
            }
        }

        public static void CheckAsteroidsOnCollisions()
        {
            for (int i = 0; i < _coel.asteroids.Count - 1; i++)
            {
                for (int j = i + 1; j < _coel.asteroids.Count; j++)
                {
                    if (_coel.asteroids[i].CollidesWith(_coel.asteroids[j]))
                    {
                        //System.Media.SystemSounds.Hand.Play(); // не при данном применении звука // TODO
                        _coel.asteroids[i].Bump(_coel.asteroids[j]);
                    }
                }
            }
        }
        #endregion

        #region СТАТИЧЕСКИЙ ИНСТРУМЕНТ ДЛЯ СОЗДАНИЯ ОБЪЕКТОВ В ИГРЕ, границы значений
        public static Celestial[] InitObjArray(int number, Func<Celestial> creationMethod)
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
            switch (rand.Next(1, 3))
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
            switch (rand.Next(1, 3))
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

        public static Missle MakeMissle_FromPlanet(Planet thatone)
        {
            return new Missle(
                new Point(thatone.Pos.X + thatone.Size.Width / 2, thatone.Pos.Y + thatone.Size.Height / 2),
                new Point(rand.Next(-20, 20), rand.Next(-20, 20)),
                new Size(rand.Next(9, 12), rand.Next(9, 12))
            );
        }

        public static Missle ShootMissle()
        {
            return new Missle(new Point(_coel.ship.Pos.X + _coel.ship.Size.Width, _coel.ship.Pos.Y + _coel.ship.Size.Height / 2), new Point(20, 0), new Size(27, 5));
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
