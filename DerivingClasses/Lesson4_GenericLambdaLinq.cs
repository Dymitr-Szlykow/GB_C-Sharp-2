using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addendum.DerivingClasses
{
    class Lesson4
    {
        static int _start;
        static int _end;

        public static void Demonstrate()
        {
            Part1();
            Vacate();

            Part2();
            Vacate();

            Part3();
        }

        static void Vacate()
        {
            Console.SetCursorPosition(0, _start);
            var emptyLine = new string(' ', Console.WindowWidth);
            while (Console.CursorTop <= _end)
            {
                Console.Write(emptyLine);
            }
            Console.SetCursorPosition(0, _start);
        }


        /// <summary>
        /// подсчитать частоту вхождения каждого элемента в List<T> а) для целых чисел
        /// </summary>
        static void Part1()
        {
            _start = Console.CursorTop;
            Console.WriteLine("Подсчет различных вхождений в обощенной коллекции.\n");

            int line = 15,
                startY = Console.CursorTop,
                startX = 3 * line + 10,
                quantity = 100;

            // сформировать и вывести случайный неупорядоченный список
            var list = new List<int>(quantity);
            var rand = new Random();
            Console.WriteLine("Случайный список:");
            for (int i = 0; i < quantity; i += line)
            {
                for (int j = 0; j < line && i + j < quantity; j++)
                {
                    list.Add(rand.Next(10));
                    Console.Write($"{list[i + j],3}");
                }
                Console.WriteLine();
            }

            // сформировать неупорядоченный Dictionary<,>
            var dict = new Dictionary<int, int>();
            foreach (int item in list)
            {
                if (dict.ContainsKey(item))
                    dict[item]++;
                else
                    dict.Add(item, 1);
            }

            // упорядочить и вывести список
            list.Sort();
            Console.SetCursorPosition(startX, startY);
            Console.WriteLine("Упорядоченный список:");
            for (int i = 0; i < list.Count; i += line)
            {
                Console.CursorLeft = startX;
                for (int j = 0; j < line && i + j < list.Count; j++)
                {
                    Console.Write($"{list[i + j],3}");
                }
                Console.WriteLine();
            }

            // вывести неупорядоченный Dictionary<,>
            startY = ++Console.CursorTop;
            foreach (KeyValuePair<int, int> entry in dict)
            {
                Console.Write($"{entry.Key,5}  -  {entry.Value} раз\n");
            }

            // упорядочить и вывести Dictionary<,>
            int[] buffer = new int[dict.Count];
            new Dictionary<int, int>.KeyCollection(dict).CopyTo(buffer, 0);
            Array.Sort(buffer);
            Console.CursorTop = startY;
            foreach (int key in buffer)
            {
                Console.CursorLeft = startX;
                Console.WriteLine($"{key,5}  -  {dict[key]} раз");
            }

            _end = Console.CursorTop;
            Console.ReadLine();
        }


        /// <summary>
        /// подсчитать частоту вхождения каждого элемента б) для необощенной коллекции
        /// </summary>
        static void Part2()
        {
            _start = Console.CursorTop;
            Console.WriteLine("Подсчет различных вхождений в необощенной коллекции.\n");

            // случайный неупорядоченный список
            int quantity = 100;
            var list = new ArrayList(quantity);
            var rand = new Random();
            var pool = new object[]
            {
                0, 1, 2, 5, 10, 15, 42, 81, 144,
                0.0, 0.999, 3.14, 5.28,
                '_', 'ы', 'Z',
                "ура", "какие-то слова", "целая строка текста",
                new Random(), new Program(),
                //new List<int>(), new Dictionary<int, string>(), new Queue<byte>(),
                new Predicate<List<string>>(a => true), new Action<int>(b => b++)
            };

            Console.WriteLine("Случайные объекты:");
            for (int i = 0; i < quantity; i++)
            {
                list.Add(pool[rand.Next(pool.Length - 1)]);
                Console.WriteLine($"    {list[i]}");
            }


            // Dictionary<,>
            var dict = new Dictionary<object, int>();
            foreach (object item in list)
            {
                if (dict.ContainsKey(item))
                    dict[item]++;
                else
                    dict.Add(item, 1);
            }
            Console.Write("\n\n");

            foreach (var key in dict)
            {
                Console.WriteLine($"    {dict[key.Key]} раз(а):  {key.Key}");
            }

            _end = Console.CursorTop;
            Console.ReadLine();
        }


        /// <summary>
        /// подсчитать частоту вхождения каждого элемента в List<T> в) с помощью LinQ
        /// </summary>
        static void Part3()
        {
            _start = Console.CursorTop;
            Console.WriteLine("Подсчет различных вхождений в обощенной коллекции с помощью LinQ.\n");

            int startY = Console.CursorTop,
                line = 15,
                startX = 3 * line + 10,
                quantity = 100;

            // сформировать и вывести случайный неупорядоченный список
            var list = new List<int>(quantity);
            var rand = new Random();
            Console.WriteLine("Случайный список:");
            for (int i = 0; i < quantity; i += line)
            {
                for (int j = 0; j < line && i + j < quantity; j++)
                {
                    list.Add(rand.Next(10));
                    Console.Write($"{list[i + j],3}");
                }
                Console.WriteLine();
            }

            // LINQ, синтаксис запросов
            var res1 =
                (from element in list
                 orderby element
                 group element by element into continuation
                 select new { Element = continuation.Key, frequency = continuation.Count() }).ToArray();
            // LINQ, синтаксис методов
            var res = list.GroupBy(el => el).OrderBy(distinct => distinct.Key).Select(distinct => new { Element = distinct.Key, frequency = distinct.Count() }).ToArray();


            // упорядочить и вывести список
            list.Sort();
            Console.SetCursorPosition(startX, startY);
            Console.WriteLine("Упорядоченный список:");
            for (int i = 0; i < list.Count; i += line)
            {
                Console.CursorLeft = startX;
                for (int j = 0; j < line && i + j < list.Count; j++)
                {
                    Console.Write($"{list[i + j],3}");
                }
                Console.WriteLine();
            }

            // вывод результата
            startY = ++Console.CursorTop;
            for (int i =0; i < res.Length; i++)
            {
                Console.Write($"{res[i].Element,5}  -  {res[i].frequency} раз\n");
            }
            Console.WriteLine();

            Console.CursorTop = startY;
            foreach (var entry in res)
            {
                Console.CursorLeft = startX;
                Console.WriteLine($"{entry.Element,5}  -  {entry.frequency} раз");
            }

            _end = Console.CursorTop;
            Console.ReadLine();
        }


        static void Task3()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>()
            {
                { "four", 4 },
                { "two", 2 },
                { "one", 1 },
                { "three", 3 },
            };

            foreach (var pair in dict) Console.WriteLine("{0} - {1}", pair.Key, pair.Value);
            Console.WriteLine();


            //var d = dict.OrderBy(delegate (KeyValuePair<string, int> pair) { return pair.Value; });
            var d = dict.OrderBy(pair => pair.Value);

            foreach (var pair in d) Console.WriteLine("{0} - {1}", pair.Key, pair.Value);
            Console.WriteLine();


            Console.WriteLine("Демонстрация окончена.");
            Console.ReadKey(true);
        }
    }
}
