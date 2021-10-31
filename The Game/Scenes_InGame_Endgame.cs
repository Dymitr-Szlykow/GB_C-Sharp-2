using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Game
{
    class Endgame : InGame
    {
        public override PackedObjects PopulateIngameObjectsPack()
        {
            throw new NotImplementedException();
        }

        public override void Draw()
        {
            Buffer.Graphics.Clear(Color.Black);
            if (Properties.Resources.background != null)
                Buffer.Graphics.DrawImage(Properties.Resources.background, 0, 0, Width, Height);

            Celestial.Draw(_coel.stars, Buffer.Graphics);
            Celestial.Draw(_coel.planet, Buffer.Graphics);

            if (_coel.ship != null)
            {
                if (_coel.ship.Energy > 0) Celestial.Draw(_coel.ship, Buffer.Graphics);
                Buffer.Graphics.DrawString($"Энергия: {_coel.ship.Energy}", SystemFonts.DefaultFont, Brushes.White, new Point(Width - 100, 1));
                Buffer.Graphics.DrawString($"Очки ваши: {_stat.score_overall}", SystemFonts.DefaultFont, Brushes.White, new Point(Width - 100, 15));
                Buffer.Graphics.DrawString($"Очки ПКПО: {_stat.score_overall_pl}", SystemFonts.DefaultFont, Brushes.White, new Point(Width - 100, 29));
            }

            // Buffer.Graphics.MeasureString("Конец!", new Font(FontFamily.GenericSansSerif, 60, FontStyle.Underline)).ToSize(); // TODO
            Buffer.Graphics.DrawString("Конец!", new Font(FontFamily.GenericSansSerif, 60, FontStyle.Underline), Brushes.OrangeRed, Width / 2 - 160, 100);
            Buffer.Graphics.DrawString
            (
                $"Ваши очки: {_stat.score_overall}" +
                $"\t\tсбито астероидов: {_stat.asteroidsDestroyed}\n\t\t\tсбито комет: {_stat.cometsDestroyed}" +
                $"\nОчки Противо-космической планетарной обороны: {_stat.score_overall_pl}",
                new Font(FontFamily.GenericSansSerif, 15, FontStyle.Regular), Brushes.MediumTurquoise, 120, 260
            );
            Buffer.Graphics.DrawString("<backspace> - назад в меню", new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular), Brushes.White, Width / 2 - 120, 340);
            Buffer.Render();
        }

        public override void Update()
        {
            UpdateStars(MakeStar_FromCenter);
        }
    }
}
