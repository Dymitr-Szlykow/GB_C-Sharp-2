using CompanyApp.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml.Serialization;

namespace CompanyApp.WebService
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [XmlInclude(typeof(Employee)), XmlInclude(typeof(Department))] //, XmlInclude(typeof(CompanyDatabase))]
    // [System.Web.Script.Services.ScriptService] // Чтобы разрешить вызывать веб-службу из скрипта с помощью ASP.NET AJAX - раскомментируйте
    public class CompService : System.Web.Services.WebService
    {
        public string access = ConfigurationManager.ConnectionStrings["MSSQLaccess"].ConnectionString;

        [WebMethod]
        public void ManualTruncate()
        {
            using (var connection = new SqlConnection(access))
            {
                string statement = @"DELETE FROM Departments; DELETE FROM Employees;";
                connection.Open();
                _ = new SqlCommand(statement, connection).ExecuteNonQuery();
            }
        }

        [WebMethod(MessageName = "Download_all_employees")]
        public ObservableCollection<Employee> DownloadAllEmployees() => InSQL.ConnectTo(access).DownloadAllEmployees().FinishQuery();

        [WebMethod(MessageName = "Upload_employees")]
        public int UploadEmployees(ObservableCollection<Employee> list) => InSQL.ConnectTo(access).CommandUploadEmployees(list).ExecuteNonQuery();

        [WebMethod(MessageName = "Insert_employee")]
        public int InsertEmployee(Employee entry) => InSQL.ConnectTo(access).CommandUploadEmployee(entry).ExecuteNonQuery();

        [WebMethod(MessageName = "Update_employee")]
        public int UpdateEmployee(Employee entry) => InSQL.ConnectTo(access).CommandUpdateEmployee(entry).ExecuteNonQuery();


        [WebMethod(MessageName = "Download_all_departments")]
        public ObservableCollection<Department> DownloadAllDepartments() => InSQL.ConnectTo(access).DownloadAllDepartments().FinishQuery();

        [WebMethod(MessageName = "Upload_departments")]
        public int UploadDepartments(ObservableCollection<Department> list) => InSQL.ConnectTo(access).CommandUploadDepartments(list).ExecuteNonQuery();

        [WebMethod(MessageName = "Insert_department")]
        public int InsertDepartment(Department entry) => InSQL.ConnectTo(access).CommandUploadDepartment(entry).ExecuteNonQuery();

        [WebMethod(MessageName = "Update_department")]
        public int UpdateDepartment(Department entry) => InSQL.ConnectTo(access).CommandUpdateDepartment(entry).ExecuteNonQuery();


        [WebMethod(MessageName = "Remove_entry")]
        public int Remove(DatabaseEntity entry) => InSQL.ConnectTo(access).CommandRemove(entry).ExecuteNonQuery();
    }
}
