using CompanyApp.Controls;
using CompanyApp.ServiceCommunication.CompanyService;
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
    /// <summary>
    /// Interaction logic for ContactEditor.xaml
    /// </summary>
    public partial class EntryEditor : Window
    {
        public Employee NewEmployee { get; set; } = new Employee();

        public EntryEditor()
        {
            InitializeComponent();
            EntryControl.EmployeeDetailed = NewEmployee;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e) => DialogResult = true;
        private void btnCancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}
