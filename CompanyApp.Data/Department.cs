using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyApp.Data
{
    public class Department : DatabaseEntity, IEqualityComparer<Department>
    {
        protected string _title;
        protected string _location;
        protected Employee _manager;

        public static readonly string tablename = "Departments";

        #region СВОЙСТВА
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                NotifyPropertyChanged();
            }
        }
        public string Location
        {
            get => _location;
            set
            {
                _location = value;
                NotifyPropertyChanged();
            }
        }
        public Employee Manager
        {
            get => _manager;
            set
            {
                _manager = value;
                NotifyPropertyChanged();
            }
        }

        public override int ID { get; set; }
        public override string Tablename { get; } = tablename;
        #endregion

        public Department() { }
        public Department(int id, string title, string location)
        {
            ID = id;
            Title = title;
            Location = location;
        }

        public bool Equals(Department x, Department y) => x.Equals(y);
        public int GetHashCode(Department obj) => obj.GetHashCode();
    }
}
