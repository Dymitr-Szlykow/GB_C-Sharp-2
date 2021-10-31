using Addendum.DerivingClasses;
using Addendum.MSdocsExamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addendum
{
    class Program
    {
        public static ConsoleKey GetInputKey(ConsoleKey[] validKeys)
        {
            ConsoleKeyInfo inputKey;
            bool valid = false;

            do
            {
                inputKey = Console.ReadKey(true);
                if (Array.Exists(validKeys, key => key.Equals(inputKey.Key)))
                    valid = true;
            }
            while (!valid);
            return inputKey.Key;
        }

        static void Main(string[] args)
        {
            Console.Title = "Дополнительные практические задания";
            Lesson4.Demonstrate();
        }

        static void MainMenu()
        {
            ConsoleKey inputKey;
            do
            {
                Console.Title = "Дополнительные практические задания";
                Console.Clear();

                Console.WriteLine("\tДополнительные практические задания второго курса C#.");
                Console.Write("Занятия:\n" +
                    "\t(1) занятие 2: наследование.\n" +
                    "\t(2) занятие 4: работа с коллекциями.\n" +
                    "Укажите номер занятия для просмотра (или Q для выхода).");

                inputKey = GetInputKey(new ConsoleKey[]
                {
                    ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.D4, ConsoleKey.D0,
                    ConsoleKey.NumPad1, ConsoleKey.NumPad2, ConsoleKey.NumPad3, ConsoleKey.NumPad4, ConsoleKey.NumPad0, ConsoleKey.Q
                });

                switch (inputKey)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        Console.WriteLine(" - занятие 2\n\n");
                        Lesson2.Demonstrate();
                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        Console.WriteLine(" - занятие 4\n\n");
                        Lesson4.Demonstrate();
                        break;

                    case ConsoleKey.D0:
                    case ConsoleKey.NumPad0:
                        Console.WriteLine(" - скрытые примеры\n\n");
                        MSdocsExamples.LINQ.RunExample();
                        break;
                }
            }
            while (inputKey != ConsoleKey.Q);
        }
    }
}