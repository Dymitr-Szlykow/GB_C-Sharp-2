using CompanyApp.Controls;
using CompanyApp.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CompanyApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CompanyDatabase CompanyDB = new CompanyDatabase();  // наполнение случайными записями в конструкторе CompanyDatabase()

        #region СВОЙСТВА
        public ObservableCollection<Employee> Crew
        {
            get => CompanyDB.Crew;
            set => CompanyDB.Crew = value;
        }
        public ObservableCollection<Department> Departments
        {
            get => CompanyDB.Departments;
            set => CompanyDB.Departments = value;
        }
        public Employee SelectedEmployee { get; set; }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            CrewControl.DeparmentList = new ObservableCollection<string>(CompanyDB.Departments.OrderBy(dep => dep.Title).Select(dep => dep.Title));
            //     from dep in _departments
            //     orderby dep.Title
            //     select dep.Title;
        }


        private void phonebookListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0) CrewControl.EmployeeDetailed = (Employee)SelectedEmployee.Clone();
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            if (DataListView.SelectedItems.Count < 1) return;
            Crew[Crew.IndexOf(SelectedEmployee)] = CrewControl.EmployeeDetailed;
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEmployee == null) return;
            if (MessageBox.Show("Вы действительно желаете удалить запись о сотруднике?", "Удаление записи", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _ = CompanyDB.Crew.Remove(SelectedEmployee);
                CrewControl.EmployeeDetailed = null;
                //SelectedEmployee = null;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var editorWindow = new EntryEditor();
            editorWindow.EntryControl.DeparmentList = CrewControl.DeparmentList;
            if (editorWindow.ShowDialog() == true)
            {
                CompanyDB.Crew.Add(editorWindow.NewEmployee);
            }

        }
    }
}
