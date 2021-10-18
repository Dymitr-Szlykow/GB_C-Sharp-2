using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using The_Game.Properties;

namespace The_Game
{
    class ArcadeMode : InGame
    {
        //public event EventHandler NewLevel;
        public static int level;

        public override PackedParams GetNumeralPack()
        {
            level = 1;
            return new PackedParams
            {
                asteroids = 9,
                comets = 1,
                stars = 25
            };
        }

        public override PackedObjects PopulateIngameObjectsPack()
        {
            var pack = new PackedObjects();
            pack.planet = new Planet(100, 100, 200, 200);

            pack.ship = new Ship(new Point(10, Height / 2 - 30), new Point(12, 15), new Size(45, 60));
            pack.ship.Death += InGame.OnDeath;

            pack.asteroids = InitObjList(_params.asteroids, MakeAsteroid_Randomplace);
            pack.comets = InitObjList(_params.comets, MakeComet_Randomplace);
            pack.stars = InitObjList(_params.stars, MakeStar_FromCenter);
            pack.missles = new List<Celestial>();

            return pack;
        }

        public void NewLevel()
        {
            _coel.ship.Replanish();
            level++;
            _params.asteroids += 3;
            _params.comets += 1;

            _coel.asteroids = InitObjList(_params.asteroids, MakeAsteroid_FromEdges);
            _coel.comets = InitObjList(_params.comets, MakeComet_FromEdges);
        }

        #region ГЛАВНЫЙ ЦИКЛ
        public override void Draw()
        {
            Buffer.Graphics.Clear(Color.Black);
            if (Resources.background != null)
                Buffer.Graphics.DrawImage(Resources.background, 0, 0, Width, Height);

            Celestial.Draw(_coel.stars, Buffer.Graphics);
            Celestial.Draw(_coel.planet, Buffer.Graphics);
            Celestial.Draw(_coel.asteroids, Buffer.Graphics);
            Celestial.Draw(_coel.comets, Buffer.Graphics);
            Celestial.Draw(_coel.missles, Buffer.Graphics);

            if (_coel.ship != null) Celestial.Draw(_coel.ship, Buffer.Graphics);
            Buffer.Graphics.DrawString($"Энергия: {_coel.ship.Energy}", SystemFonts.DefaultFont, Brushes.White, new Point(Width - 100, 1));
            Buffer.Graphics.DrawString($"Очки: {_stat.score_overall}", SystemFonts.DefaultFont, Brushes.White, new Point(Width - 100, 15));

            Buffer.Render();
        }

        public override void Update()
        {
            UpdateStars(MakeStar_Scroller);
            _coel.ship.Update();

            UpdateMissles();
            UpdateTargets();

            if (_coel.asteroids.Count == 0 && _coel.comets.Count == 0) NewLevel();
        }
        #endregion


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
                        _coel.asteroids.RemoveAt(i);  // ! вместо  _coel.asteroids[i] = MakeAsteroid_FromEdges();
                        _coel.missles.RemoveAt(j);
                        break;
                    }
                }
                if (IsShot) continue;

                // с т о л к н у л с я  с  к о р а б л е м ?
                if (_coel.asteroids[i].CollidesWith(_coel.ship))
                {
                    SystemSounds.Beep.Play();
                    // _coel.asteroids[i].Bump(ship);
                    _coel.ship.Hit(rand.Next(3, 5));
                    if (_coel.ship.Energy < 0) _coel.ship.Dies();
                    _coel.asteroids[i] = MakeAsteroid_FromEdges();
                }

                // с т о л к н у л с я  с  д р у г и м  а с т е р о и д о м ?
                for (int j = _coel.asteroids.Count - 1; j > i; j--)
                {
                    if (_coel.asteroids[i].CollidesWith(_coel.asteroids[j]))
                    {
                        //TODO Bump()
                    }
                }
            }

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

                // с т о л к н у л а с ь  с  к о р а б л е м ?
                if (_coel.comets[i].CollidesWith(_coel.ship))
                {
                    SystemSounds.Beep.Play();
                    if (_coel.comets[i].IsAlive()) _coel.ship.Hit(rand.Next(5, 10));
                    if (_coel.ship.Energy < 0) _coel.ship.Dies();
                    _coel.comets[i].Hit();
                }

                // с т о л к н у л а с ь  с  д р у г о й  к о м е т о й ?
                for (int j = _coel.comets.Count - 1; j > i; j--)
                {
                    if (_coel.comets[i].CollidesWith(_coel.asteroids[j]))
                    {
                        // TODO Bump()
                    }
                }

                // с т о л к н у л а с ь  с  а с т е р о и д о м ?
                //for (int j = 0; j < _coel.asteroids.Count; j++)
                //{
                //    if (_coel.comets[i].CollidesWith(_coel.asteroids[j]))
                //    {
                //        // TODO Bump()
                //    }
                //}
            }
        }
    }
}
