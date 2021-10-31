using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace The_Game
{
    static class Program
    {
        /// <summary>
        /// Практическая часть курса "C# уровень 2"
        /// студент Дмитрий Шлыков
        /// </summary>
        [STAThread]
        static void Main()
        {
            /*
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            */

            Form theForm = new Form()
            {
                MinimumSize = new System.Drawing.Size(800, 500),
                MaximumSize = new System.Drawing.Size(800, 500),
                MinimizeBox = false,
                MaximizeBox = false,
                StartPosition = FormStartPosition.CenterScreen,
                Text = "Астероиды"
            };
            theForm.Show();

            SceneManager
                .Boot()                                         // убедиться, что есть SceneManager (возвращает SceneManager)
                .PrepareScene<Menu>(new SceneArgs(theForm))     // установить в SceneManager новую сцену (возвращает IScene)
                .Draw();                                        // отрисовать ее

            Application.Run(theForm);
        }
    }
}
