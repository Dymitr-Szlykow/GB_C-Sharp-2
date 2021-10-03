using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace The_Game
{
    static class Program
    {
        /*
         * Практическое задание к первому уроку курса "C# уровень 2", занятие проходило 30.09.2021
         * 
         * 1. Добавить свои объекты в иерархию объектов, чтобы получился красивый задний фон, похожий на полёт в звёздном пространстве.
         * 2. *Заменить кружочки картинками, используя метод DrawImage.
         */

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
