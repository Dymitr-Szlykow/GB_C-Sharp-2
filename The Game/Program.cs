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

            Form theForm = new Form();
            theForm.MinimumSize = new System.Drawing.Size(800, 500);
            theForm.MaximumSize = new System.Drawing.Size(800, 500);
            theForm.MinimizeBox = false;
            theForm.MaximizeBox = false;
            theForm.StartPosition = FormStartPosition.CenterScreen;
            theForm.Text = "Астероиды";

            GameLogic.Init(theForm);
            //theForm.Show();
            //GameLogic.Draw();
            Application.Run(theForm);
        }
    }
}
