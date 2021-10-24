using CompanyApp;
using CompanyApp.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CompanyApp.Controls
{
    public partial class CrewControl : UserControl, INotifyPropertyChanged
    {
        private Employee _employeeDetailed;
        private ObservableCollection<string> _deparmentList;

        public Employee EmployeeDetailed
        {
            get => _employeeDetailed;
            set
            {
                _employeeDetailed = value;
                NotifyPropertyChanged();
            }
        }
        public ObservableCollection<string> DeparmentList
        {
            get => _deparmentList;
            set
            {
                _deparmentList = value;
                NotifyPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public CrewControl()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
