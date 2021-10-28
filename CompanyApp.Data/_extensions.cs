using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyApp.Data
{
    public static class InSQL
    {
        public static SqlConnection ConnectTo(string connectionString) => new SqlConnection(connectionString);

        public static int ExecuteNonQuery(this (SqlConnection, SqlCommand) bundle)
        {
            bundle.Item1.Open();
            var temp = bundle.Item2.ExecuteNonQuery();
            bundle.Item1.Dispose();
            return temp;
        }

        public static ObservableCollection<T> FinishQuery<T>(this (SqlConnection, ObservableCollection<T>) bundle) where T : IDatabaseEntity
        {
            bundle.Item1.Dispose();
            return bundle.Item2;
        }


        #region DATA STRUCTURE
        public static (SqlConnection, SqlCommand) StartAnew(this SqlConnection connection)
        {
            string statement =
                @"TRUNCATE TABLE Employees; " +
                @"TRUNCATE TABLE Departments;";
            return (connection, new SqlCommand(statement, connection));
        }
        #endregion


        #region DATA QUERY: SELECT clause
        public static (SqlConnection, ObservableCollection<Employee>) DownloadAllEmployees(this SqlConnection connection)
        {
            var command = new SqlCommand($"SELECT * FROM Employees LEFT OUTER JOIN Departments ON (Employees.Department_id = Departments.id)", connection);
            var list = new ObservableCollection<Employee>();

            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read()) list.LoadEmployee(reader);
            }
            return (connection, list);
        }

        public static (SqlConnection, ObservableCollection<Department>) DownloadAllDepartments(this SqlConnection connection)
        {
            var command = new SqlCommand($"SELECT * FROM Departments LEFT OUTER JOIN Employees ON (Departments.Manager_id = Employees.id)", connection);
            var list = new ObservableCollection<Department>();

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read()) list.LoadDepartment(reader);
            reader.Dispose();

            return (connection, list);
        }

        public static void LoadEmployee(this ObservableCollection<Employee> list, SqlDataReader reader)
        {
            list.Add(new Employee()
            {
                ID = (int)reader["id"],
                Name = reader["Name"].ToString(),
                Surname = reader["Surname"]?.ToString() ?? string.Empty,
                Patronym = reader["Patronym"]?.ToString() ?? string.Empty,
                BirthDate = reader["BirthDate"]?.ToString() ?? string.Empty,
                GenderAsChar = reader["Gender"].ToString()[0],
                Salary = (int)reader["Salary"],
                Department = reader["Title"]?.ToString() ?? string.Empty
            });
        }

        public static void LoadDepartment(this ObservableCollection<Department> list, SqlDataReader reader)
        {
            list.Add(new Department()
            {
                ID = (int)reader["id"],
                Title = reader["Title"].ToString(),
                Location = reader["Location"]?.ToString() ?? string.Empty
            });
        }
        #endregion


        #region DATA MANIPULATION: INSERT clause
        public static (SqlConnection, SqlCommand) CommandUploadEmployees(this SqlConnection connection, ObservableCollection<Employee> list)
        {
            var statement = new StringBuilder("INSERT INTO Employees (id, Name, Surname, Patronym, BirthDate, Gender, Salary) VALUES");
            for (int i = 0; i < list.Count; i++)
            {
                statement.Append($" ({list[i].ID}, '{list[i].Name}', '{list[i].Surname}', '{list[i].Patronym}', '{list[i].BirthDate}', '{list[i].GenderAsChar}', {list[i].Salary}),");
            }
            statement[statement.Length - 1] = ';';
            return (connection, new SqlCommand(statement.ToString(), connection));
        }
        public static (SqlConnection, SqlCommand) CommandUploadEmployee(this SqlConnection connection, Employee entry)
        {
            string statement = "INSERT INTO Employees (id, Name, Surname, Patronym, BirthDate, Gender, Salary) VALUES" +
                $" ({entry.ID}, '{entry.Name}', '{entry.Surname}', '{entry.Patronym}', '{entry.BirthDate}', '{entry.GenderAsChar}', {entry.Salary});";
            return (connection, new SqlCommand(statement.ToString(), connection));
        }

        public static (SqlConnection, SqlCommand) CommandUploadEmployees_withAUTOINCREMENT(this SqlConnection connection, ObservableCollection<Employee> list)
        {
            var statement = new StringBuilder("INSERT INTO Employees (Name, Surname, Patronym, BirthDate, Gender, Salary) VALUES");
            foreach (var one in list)
            {
                statement.Append($" ('{one.Name}', '{one.Surname}', '{one.Patronym}', '{one.BirthDate}', '{one.GenderAsChar}', {one.Salary}),");
                //statement.Append(list.GetEnumerator().Current == list.Last() ? ";" : ",");
            }
            statement[statement.Length - 1] = ';';
            return (connection, new SqlCommand(statement.ToString(), connection));
        }

        public static (SqlConnection, SqlCommand) CommandUploadDepartments(this SqlConnection connection, ObservableCollection<Department> list)
        {
            var statement = new StringBuilder("INSERT INTO Departments (id, Title, Location) VALUES");
            for (int i = 0; i < list.Count; i++)
            {
                statement.Append($" ({list[i].ID}, '{list[i].Title}', '{list[i].Location}'),");
            }
            statement[statement.Length - 1] = ';';
            return (connection, new SqlCommand(statement.ToString(), connection));
        }
        public static (SqlConnection, SqlCommand) CommandUploadDepartment(this SqlConnection connection, Department entry)
        {
            string statement = $"INSERT INTO Departments (id, Title, Location) VALUES ({entry.ID}, '{entry.Title}', '{entry.Location}');";
            return (connection, new SqlCommand(statement.ToString(), connection));
        }

        public static (SqlConnection, SqlCommand) CommandUploadDepartments_withAUTOINCREMENT(this SqlConnection connection, ObservableCollection<Department> list)
        {
            var statement = new StringBuilder("INSERT INTO Departments (Title, Location) VALUES");
            foreach (var one in list)
            {
                statement.Append($" ('{one.Title}', '{one.Location}'),");
                //statement.Append(list.GetEnumerator().Current == list.Last() ? ";" : ",");
            }
            statement[statement.Length - 1] = ';';
            return (connection, new SqlCommand(statement.ToString(), connection));
        }
        #endregion


        #region DATA MANIPULATION: UPDATE clause
        public static (SqlConnection, SqlCommand) CommandUpdateEmployee(this SqlConnection connection, Employee entry)
        {
            string statement =
                @"UPDATE Employees SET " +
                $@"Name = '{entry.Name}', " +
                $@"Surname = '{entry.Surname}', " +
                $@"Patronym = '{entry.Patronym}', " +
                $@"BirthDate = '{entry.BirthDate}', " +
                $@"Gender = '{entry.GenderAsChar}', " +
                $@"Salary = {entry.Salary} " +
                $@"WHERE id = {entry.ID};";
            return (connection, new SqlCommand(statement, connection));
        }

        public static (SqlConnection, SqlCommand) CommandUpdateDepartment(this SqlConnection connection, Department entry)
        {
            string statement =
                @"UPDATE Departments SET " +
                $@"Title = '{entry.Title}', " +
                $@"Location = '{entry.Location}' " +
                $@"WHERE id = {entry.ID};";
            return (connection, new SqlCommand(statement, connection));
        }
        #endregion


        #region DATA MANIPULATION: DELETE clause
        public static (SqlConnection, SqlCommand) CommandRemove<T>(this SqlConnection connection, T entry) where T : IDatabaseEntity
        {
            string statement = $@"DELETE FROM {entry.Tablename} WHERE id = {entry.ID};";
            return (connection, new SqlCommand(statement, connection));
        }

        //public static (SqlConnection, SqlCommand) CommandRemoveDepartment(this SqlConnection connection, Department entry)
        //{
        //    string statement = $@"DELETE FROM Departments WHERE id = {entry.ID}";
        //    return (connection, new SqlCommand(statement, connection));
        //}
        #endregion

        //Predicate<int> del_if;              // delegate bool System.Predicate<in T>(T obj)
        //Action<int> del21;                  // delegate void System.Action<in T>(T obj)
        //Action<,,,,,,,,,,,,,,,> del22;      // delegate void System.Action<in T1, in T2...>(T1 arg1, T2 arg2...) до 16
        //Func<int> del31;                    // delegate TResult System.Func<out TResult>()
        //Func<,,,,,,,,,,,,,,,,> del32;       // delegate TResult System.Func<in T1, in T2...out TResult>(T1 arg1, T2 arg2...) до 16 входных
    }
}
