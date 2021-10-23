using CompanyApp.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyApp
{
    public class CompanyDatabase
    {
        private static string[] PHONE_PREFIX = { "906", "495", "499" }; // Префексы телефонных номеров
        private static int CHAR_BOUND_L = 65; // Номер начального символа (для генерации последовательности символов)
        private static int CHAR_BOUND_H = 90; // Номер конечного  символа (для генерации последовательности символов)

        private Random random = new Random();

        private ObservableCollection<Department> _departments;
        private ObservableCollection<Employee> _crew;

        public ObservableCollection<Department> Departments
        {
            get => _departments;
            set => _departments = value;
        }
        public ObservableCollection<Employee> Crew
        {
            get => _crew;
            set => _crew = value;
        }

        public CompanyDatabase() => GenerateDB();

        private string GenerateSymbols(int amount)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < amount; i++)
                stringBuilder.Append((char)(CHAR_BOUND_L + random.Next(CHAR_BOUND_H - CHAR_BOUND_L)));
            return stringBuilder.ToString();
        }

        private string GeneratePhone()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("+7").Append(PHONE_PREFIX[random.Next(3)]);
            for (int i = 0; i < 6; i++)
                stringBuilder.Append(random.Next(10));
            return stringBuilder.ToString();
        }

        private ObservableCollection<Department> GenerateDepartments(int contactCount)
        {
            var res = new ObservableCollection<Department>();

            string title, location;

            for (int i = 0; i < contactCount; i++)
            {
                title = GenerateSymbols(random.Next(6) + 5);
                location = GenerateSymbols(random.Next(6) + 5);

                res.Add(new Department(title, location));
            }
            return res;
        }

        private ObservableCollection<Employee> GenerateEmployees(int contactCount)
        {
            var res = new ObservableCollection<Employee>();

            string name, surname, patronym, department;
            char gender;
            int salary;

            for (int i = 0; i < contactCount; i++)
            {
                //if (random.Next(2) == 0)
                name = GenerateSymbols(random.Next(6) + 5);
                surname = GenerateSymbols(random.Next(6) + 5);
                patronym = GenerateSymbols(random.Next(6) + 5);
                gender = random.Next(2) == 0 ? 'м' : 'ж';
                salary = random.Next(20, 85) * 1000;
                department = Departments[random.Next(Departments.Count)].Title;

                res.Add(new Employee(name, surname, patronym, string.Empty, gender, salary, department));
            }
            return res;
        }

        private void GenerateDB()
        {
            Departments = GenerateDepartments(9);
            Crew = GenerateEmployees(42);

            foreach (Department one in Departments)
            {
                one.Manager = Crew[random.Next(Crew.Count)];
            }
        }
    }
}
