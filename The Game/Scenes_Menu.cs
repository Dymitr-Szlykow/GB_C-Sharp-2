using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace The_Game
{
    class Menu : BaseScene
    {
        #region ПОЛЯ И СВОЙСТВА
        static MenuOption _title;
        static List<MenuOption> _menu;
        static List<Celestial> _stars;
        protected static int focus;

        public static int FocusedOption
        {
            get { return focus; }
            set
            {
                if (_menu != null)
                {
                    if (value < 0 || _menu.Count <= value)
                        focus = 0;
                    else
                        focus = value;
                }
            }
        }
        #endregion

        #region ЯДРО
        public override void Init(Form form)
        {
            base.Init(form);
            _stars = InGame.InitObjList(32, InGame.MakeStar_FromCenter);
            _title = new MenuOption(Height * 3 / 20, "А С Т Е Р О И Д Ы", new Font(FontFamily.GenericSansSerif, 24, FontStyle.Bold), Buffer.Graphics, delegate () { });
            _menu = SetMainMenu();
            focus = 0;

            timer = new Timer();
            timer.Interval = 60;
            timer.Tick += OnTimerTick;
            timer.Start();
        }

        public override void SceneKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                if (FocusedOption > 0) FocusedOption--;
            }
            if (e.KeyCode == Keys.Down)
            {
                if (FocusedOption < _menu.Count - 1) FocusedOption++;
            }
            if (e.KeyCode == Keys.Enter) _menu[FocusedOption].Push.Invoke();
        }
        #endregion

        #region НА СЦЕНЕ
        protected void OnTimerTick(object sender, EventArgs e)
        {
            UpdateStars(InGame.MakeStar_FromCenter);
            Draw();
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

        public override void Draw()
        {
            Buffer.Graphics.Clear(Color.Black);
            Celestial.Draw(_stars, Buffer.Graphics);
            DrawOptions();
            _title.DrawContent();
            Buffer.Render();
        }

        public void DrawOptions()
        {
            for (int i = 0; i < _menu.Count; i++)
            {
                if (i == FocusedOption)
                    _menu[i].DrawFocused();
                else
                    _menu[i].DrawButton();
            }
        }
        #endregion

        #region РАЗКЛАДКИ
        protected List<MenuOption> SetMainMenu()
        {
            var res = new List<MenuOption>();
            res.Add(new MenuOption(Height * 8 / 20, "Новая игра в аркадном режиме", new Font(FontFamily.GenericSansSerif, 15, FontStyle.Regular), Buffer.Graphics, LounchSingleMode));
            res.Add(new MenuOption(Height * 11 / 20, "Новая игра в соревновательном режиме", new Font(FontFamily.GenericSansSerif, 15, FontStyle.Regular), Buffer.Graphics, LounchContestMode));
            res.Add(new MenuOption(Height * 14 / 20, "Выход", new Font(FontFamily.GenericSansSerif, 14, FontStyle.Regular), Buffer.Graphics, ExitApp));
            return res;
        }
        #endregion

        #region ДЕЙСТВИЯ
        public void LounchSingleMode()
        {
            //TODO
        }

        public void LounchContestMode()
        {
            SceneManager
                .Boot()                       // обновить SceneManager, сбросить управляемую сцену (возвращает SceneManager)
                .PrepareScene<InGame>(_form)  // установить в обновленный SceneManager новую сцену (возвращает IScene)
                .Draw();                      // отрисовать ее
        }

        public void ExitApp() => _form.Close();
        #endregion
    }
}
