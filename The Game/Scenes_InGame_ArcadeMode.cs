using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Game.Properties;

namespace The_Game
{
    class ArcadeMode : InGame
    {
        public override PackedObjects PopulateIngameObjectsPack()
        {
            var pack = new PackedObjects();
            pack.planet = new Planet(Width - 150, 70, 200, 200);
            pack.planet.ReachingEdge += OnReachingEdge;

            pack.ship = new Ship(new Point(10, Height / 2 - 30), new Point(12, 15), new Size(45, 60));
            pack.ship.Death += OnDeath;

            pack.missle_pl = MakeMissle_FromPlanet(pack.planet);

            pack.asteroids = InitObjList(12, MakeAsteroid_Randomplace);
            pack.comets = InitObjList(2, MakeComet_Randomplace);
            pack.stars = InitObjList(25, MakeStar_FromCenter);
            pack.kits = new List<Celestial>();
            pack.missles = new List<Celestial>();

            return pack;
        }

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

            if (_coel.ship != null)
            {
                Celestial.Draw(_coel.ship, Buffer.Graphics);
                Buffer.Graphics.DrawString($"Энергия: {_coel.ship.Energy}", SystemFonts.DefaultFont, Brushes.White, new Point(Width - 100, 1));
                Buffer.Graphics.DrawString($"Очки ваши: {_stat.score_overall}", SystemFonts.DefaultFont, Brushes.White, new Point(Width - 100, 15));
                Buffer.Graphics.DrawString($"Очки ПВО: {_stat.score_overall_pl}", SystemFonts.DefaultFont, Brushes.White, new Point(Width - 100, 29));
            }

            Buffer.Render();
        }

        public override void Update()
        {
            UpdateStars(MakeStar_Scroller);
            _coel.planet.Update();
            _coel.ship.Update();

            UpdateMissles();
            UpdateTargets();

            CheckKits();

            if (_coel.planet.Pos.X < -_coel.planet.Size.Width / 2) _coel.planet.ReachesEdge();
        }
    }
}
