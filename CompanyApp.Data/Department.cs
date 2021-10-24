using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CompanyApp.Data
{
    public class Department : INotifyPropertyChanged, ICloneable, IEqualityComparer<Department>
    {
        protected string _title;
        protected string _location;
        protected Employee _manager;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
        #endregion

        public Department() { }
        public Department(string title, string location)
        {
            Title = title;
            Location = location;
        }

        public object Clone() => MemberwiseClone();
        public bool Equals(Department x, Department y) => x.Equals(y);
        public int GetHashCode(Department obj) => obj.GetHashCode();
    }
}
