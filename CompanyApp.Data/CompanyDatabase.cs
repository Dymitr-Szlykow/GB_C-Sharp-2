using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyApp.Data
{
    public class CompanyDatabase
    {
        public const string access = @"Data Source=DESKTOP-8NDG9PK\MSSQLEXPRESS;Initial Catalog=CompanyDB;User ID=uczebny;Password=1";
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

        public int NextDepartmentID { get; set; }
        public int NextCrewID { get; set; }

        public CompanyDatabase()
        {
            NextDepartmentID = NextCrewID = 1;
            //_ = InSQL.ConnectTo(access).StartAnew().ExecuteNonQuery();

            ManualTruncate();
            _departments = GenerateDepartments(9);
            _crew = GenerateEmployees(42);
            foreach (Department one in Departments) one.Manager = Crew[random.Next(Crew.Count)];
            _ = InSQL.ConnectTo(access).CommandUploadDepartments(_departments).ExecuteNonQuery();
            _ = InSQL.ConnectTo(access).CommandUploadEmployees(_crew).ExecuteNonQuery();

            //if ((_crew = InSQL.ConnectTo(access).DownloadAllEmployees().FinishQuery()).Count == 0)
            //{
            //    _crew = GenerateEmployees(42);
            //    _ = InSQL.ConnectTo(access).CommandUploadEmployees(_crew).ExecuteNonQuery();
            //}
            //if ((_departments = InSQL.ConnectTo(access).DownloadAllDepartments().FinishQuery()).Count == 0)
            //{
            //    _departments = GenerateDepartments(9);
            //    foreach (Department one in Departments)
            //    {
            //        one.Manager = Crew[random.Next(Crew.Count)];
            //    }
            //    _ = InSQL.ConnectTo(access).CommandUploadDepartments(_departments).ExecuteNonQuery();
            //}
        }

        protected void ManualTruncate()
        {
            using (var connection = new SqlConnection(access))
            {
                string statement = @"DELETE FROM Departments; DELETE FROM Employees;";
                connection.Open();
                _ = new SqlCommand(statement, connection).ExecuteNonQuery();
            }
        }

        public int InsertInSQL(Employee entry) => InSQL.ConnectTo(access).CommandUploadEmployee(entry).ExecuteNonQuery();
        public int InsertInSQL(Department entry) => InSQL.ConnectTo(access).CommandUploadDepartment(entry).ExecuteNonQuery();
        public int UpdateInSQL(Employee entry) => InSQL.ConnectTo(access).CommandUpdateEmployee(entry).ExecuteNonQuery();
        public int UpdateInSQL(Department entry) => InSQL.ConnectTo(access).CommandUpdateDepartment(entry).ExecuteNonQuery();
        public int RemoveInSQL(IDatabaseEntity entry) => InSQL.ConnectTo(access).CommandRemove(entry).ExecuteNonQuery();


        #region СЛУЧАЙНАЯ ГЕНЕРАЦИЯ ДАННЫХ
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

            for (int i = 0; i < contactCount; i++, NextDepartmentID++)
            {
                title = GenerateSymbols(random.Next(6) + 5);
                location = GenerateSymbols(random.Next(6) + 5);

                res.Add(new Department(NextDepartmentID, title, location));
            }
            return res;
        }

        private ObservableCollection<Employee> GenerateEmployees(int contactCount)
        {
            var res = new ObservableCollection<Employee>();

            string name, surname, patronym, department;
            GenderType gender;
            int salary;

            for (int i = 0; i < contactCount; i++, NextCrewID++)
            {
                //if (random.Next(2) == 0)
                name = GenerateSymbols(random.Next(6) + 5);
                surname = GenerateSymbols(random.Next(6) + 5);
                patronym = GenerateSymbols(random.Next(6) + 5);
                gender = random.Next(2) == 0 ? GenderType.Male : GenderType.Female;
                salary = random.Next(20, 85) * 1000;
                department = Departments[random.Next(Departments.Count)].Title ?? string.Empty;

                res.Add(new Employee(NextCrewID, name, surname, patronym, string.Empty, gender, salary, department));
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
        #endregion
    }


    public interface IDatabaseEntity
    {
        int ID { get; set; }
        string Tablename { get; }
    }
}
