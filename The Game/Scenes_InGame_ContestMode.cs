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
    class ContestMode : InGame
    {
        public override PackedParams GetNumeralPack()
        {
            timer.Tick += RemakingEngikits;
            return new PackedParams
            {
                asteroids = 12,
                comets = 2,
                stars = 25
            };
        }

        public override PackedObjects PopulateIngameObjectsPack()
        {
            var pack = new PackedObjects();
            pack.planet = new Planet(Width - 150, 70, 200, 200);
            pack.planet.ReachingEdge += OnReachingEdge;

            pack.ship = new Ship(new Point(10, Height / 2 - 30), new Point(12, 15), new Size(45, 60));
            pack.ship.Death += OnDeath;

            pack.missle_pl = MakeMissle_FromPlanet(pack.planet);

            pack.asteroids = InitObjList(_params.asteroids, MakeAsteroid_Randomplace);
            pack.comets = InitObjList(_params.comets, MakeComet_Randomplace);
            pack.stars = InitObjList(_params.stars, MakeStar_FromCenter);
            pack.kits = new List<Celestial>();
            pack.missles = new List<Celestial>();

            return pack;
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
            Celestial.Draw(_coel.kits, Buffer.Graphics);
            Celestial.Draw(_coel.missles, Buffer.Graphics);
            if (_coel.missle_pl != null) Celestial.Draw(_coel.missle_pl, Buffer.Graphics);

            if (_coel.ship != null) Celestial.Draw(_coel.ship, Buffer.Graphics);
            Buffer.Graphics.DrawString($"Энергия: {_coel.ship.Energy}", SystemFonts.DefaultFont, Brushes.White, new Point(Width - 100, 1));
            Buffer.Graphics.DrawString($"Очки ваши: {_stat.score_overall}", SystemFonts.DefaultFont, Brushes.White, new Point(Width - 100, 15));
            Buffer.Graphics.DrawString($"Очки ПВО: {_stat.score_overall_pl}", SystemFonts.DefaultFont, Brushes.White, new Point(Width - 100, 29));

            Buffer.Render();
        }

        public override void Update()
        {
            UpdateStars(MakeStar_Scroller);
            _coel.planet.Update();
            _coel.ship.Update();

            UpdateMissles();
            UpdateMissle_pl();
            UpdateTargets();

            CheckKits();

            if (_coel.planet.Pos.X < -_coel.planet.Size.Width / 2) _coel.planet.ReachesEdge();
        }

        public void RemakingEngikits(object sender, EventArgs e)
        {
            if (_countdown == 0)
            {
                _coel.kits.Add(MakeEngikit());
                _countdown = rand.Next(90, 150);
            }
            else
                _countdown--;
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
                    _coel.ship.Hit(rand.Next(3, 5));
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
                //for (int j = 0; j < _coel.asteroids.Count; j++)
                //{
                //    if (_coel.comets[i].CollidesWith(_coel.asteroids[j]))
                //    {
                //        // TODO
                //    }
                //}
            }
            while (_coel.comets.Count < 2) _coel.comets.Add(MakeComet_FromEdges());
        }
    }
}
