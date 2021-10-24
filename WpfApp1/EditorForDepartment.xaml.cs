using CompanyApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CompanyApp
{
    public partial class EditorForDepartment : Window
    {
        public Department NewDepartment { get; set; } = new Department();

        public EditorForDepartment()
        {
            InitializeComponent();
            EntryControl.DepartmentDetailed = NewDepartment;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e) => DialogResult = true;
        private void btnCancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}
