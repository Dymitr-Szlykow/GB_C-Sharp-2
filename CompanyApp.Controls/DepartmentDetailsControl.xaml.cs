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
    public partial class DepartmentDetailsControl : UserControl, INotifyPropertyChanged
    {
        private Department _departmentDetailed;

        public Department DepartmentDetailed
        {
            get => _departmentDetailed;
            set
            {
                _departmentDetailed = value;
                NotifyPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public DepartmentDetailsControl()
        {
            InitializeComponent();
            DataContext = this;
            //CategoryList.Add(ContactCategory.General);
            //CategoryList.Add(ContactCategory.Personal);
            //CategoryList.Add(ContactCategory.Working);
            //cbCategory.ItemsSource = Enum.GetValues(typeof(ContactCategory)).Cast<ContactCategory>();
        }
    }
}
