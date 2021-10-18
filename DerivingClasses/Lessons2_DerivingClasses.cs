using Adendum.DerivingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addendum.DerivingClasses
{
    /// <summary>
    /// Дполонительное задание ко второму занятию
    /// </summary>
    class Lesson2
    {
        //static CrewList1 employees1 = new CrewList1();
        //static CrewList2 employees2 = new CrewList2();
        //    или
        static CrewList1<Employee> employees1 = new CrewList1<Employee>();
        static CrewList2<Employee> employees2 = new CrewList2<Employee>();

        static void Report(CrewList list)
        {
            Console.WriteLine("\n   В массиве {0}", list.GetType() == typeof(CrewList1) ? "статическом (Employee[]):" : "динамическом (List<Employee>):");
            if (list.Count == 0)
                Console.WriteLine("\t- массив пуст -");
            else
            {
                foreach (Employee one in list)
                {
                    Console.Write("\t{0}, {1}:", one.Name, one.GetType() == typeof(HourPaid) ? "почасовая оплата" : "фиксированная ставка");
                    Console.CursorLeft = 52;
                    Console.Write("ставка {0} руб{1}", one.Salary, one.GetType() == typeof(HourPaid) ? "/ч" : "");
                    Console.CursorLeft = 72;
                    Console.WriteLine("среднемесячная опалата {0} руб", one.GetMonthAverageSalary());
                }
            }
        }

        static void Report(CrewList<Employee> list)
        {
            Console.WriteLine("\n   В массиве {0}", list.GetType() == typeof(CrewList1<Employee>) ? "статическом (Employee[]):" : "динамическом (List<Employee>):");
            if (list.Count == 0)
                Console.WriteLine("\t- массив пуст -");
            else
            {
                foreach (Employee one in list)
                {
                    Console.Write("\t{0}, {1}:", one.Name, one.GetType() == typeof(HourPaid) ? "почасовая оплата" : "фиксированная ставка");
                    Console.CursorLeft = 52;
                    Console.Write("ставка {0} руб{1}", one.Salary, one.GetType() == typeof(HourPaid) ? "/ч" : "");
                    Console.CursorLeft = 72;
                    Console.WriteLine("среднемесячная опалата {0} руб", one.GetMonthAverageSalary());
                }
            }
        }

        static void ShowAndWait(bool separate)
        {
            Report(employees1);
            Report(employees2);
            if (separate) Console.WriteLine("\n" + new string('-', 100) + "\n");
            Console.ReadKey(true);
        }

        static void Demonstrate(string method, string answer1, string answer2)
        {
            Console.Write("- " + method);
            Console.CursorLeft = 50;
            Console.Write(answer1);
            Console.CursorLeft = 70;
            Console.WriteLine(answer2);
        }


        public static void Demonstrate()
        {
            var emp1 = new HourPaid("Петя", 125);
            var emp2 = new FixPaid("Дима", 17620);
            var emp3 = new FixPaid("Маша", 25431);
            var emp4 = new HourPaid("Света", 210);
            var emp5 = new HourPaid("Ваня", 160);
            var emp6 = new FixPaid("Андрей", 21590);
            var emp7 = new FixPaid("Лариса Ивановна", 26624);

            employees1.Add(emp1);
            employees1.Add(emp2);
            employees1.Add(emp3);
            employees1.Add(emp4);
            employees1.Add(emp5);
            employees1.Add(emp6);
            employees1.Add(emp7);

            employees2.Add(emp1);
            employees2.Add(emp2);
            employees2.Add(emp3);
            employees2.Add(emp4);
            employees2.Add(emp5);
            employees2.Add(emp6);
            employees2.Add(emp7);

            Console.WriteLine("Список в порядке добавления: метод Add().");
            ShowAndWait(true);

            Console.WriteLine("Сортировка по имени: метод Sort() с помощью IComparable.");
            employees1.Sort();
            employees2.Sort();
            ShowAndWait(true);

            Console.WriteLine("Сортировка по средней месячной зарплате: метод Sort() с помощью вспомогательного класса и IComparer.");
            employees1.Sort(new Employee.SalaryComparer());
            employees2.Sort(new Employee.SalaryComparer());
            ShowAndWait(true);

            Console.WriteLine("Некоторые методы из ICollection.\n");
            Demonstrate("Constains(new HourPaid()):", employees1.Contains(new HourPaid("", 0)).ToString(), employees2.Contains(new HourPaid("", 0)).ToString());
            Demonstrate("Constains(var3):", employees1.Contains(emp3).ToString(), employees2.Contains(emp3).ToString());
            Console.ReadKey();

            Console.WriteLine();
            Demonstrate("Remove(new FixPaid()):", employees1.Remove(new FixPaid("", 0)).ToString(), employees2.Remove(new FixPaid("", 0)).ToString());
            ShowAndWait(true);

            Console.WriteLine();
            Demonstrate("Constains(var1):", employees1.Contains(emp1).ToString(), employees2.Contains(emp1).ToString());
            Demonstrate("Remove(var1):", employees1.Remove(emp1).ToString(), employees2.Remove(emp1).ToString());
            Demonstrate("Constains(var1):", employees1.Contains(emp1).ToString(), employees2.Contains(emp1).ToString());
            ShowAndWait(true);

            Console.WriteLine();
            Demonstrate("Constains(var3):", employees1.Contains(emp3).ToString(), employees2.Contains(emp3).ToString());
            Demonstrate("Remove(var3):", employees1.Remove(emp3).ToString(), employees2.Remove(emp3).ToString());
            Demonstrate("Constains(var3):", employees1.Contains(emp3).ToString(), employees2.Contains(emp3).ToString());
            ShowAndWait(true);

            Console.WriteLine();
            Demonstrate("Constains(var5):", employees1.Contains(emp5).ToString(), employees2.Contains(emp5).ToString());
            Demonstrate("Remove(var5):", employees1.Remove(emp5).ToString(), employees2.Remove(emp5).ToString());
            Demonstrate("Constains(var5):", employees1.Contains(emp5).ToString(), employees2.Contains(emp5).ToString());
            ShowAndWait(true);

            Console.WriteLine("\n- Clear():");
            employees1.Clear();
            employees2.Clear();
            ShowAndWait(true);

            Console.WriteLine("Демонстрация окончена. А вывод каждый раз осуществлялся с помощью инструкции foreach.");
            Console.ReadKey(true);
        }
    }
}
