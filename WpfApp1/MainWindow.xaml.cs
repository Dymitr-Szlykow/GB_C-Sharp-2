using CompanyApp.Controls;
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

namespace CompanyApp
{
    public partial class MainWindow : Window
    {
        private CompanyDatabase CompanyDB = new CompanyDatabase();  // наполнение случайными записями в конструкторе CompanyDatabase()
        private ObservableCollection<string> _deparmentList;
        //private CrewControl _employeeDetails;


        #region СВОЙСТВА
        public ObservableCollection<Employee> Crew
        {
            get => CompanyDB.Crew;
            set => CompanyDB.Crew = value;
        }
        public ObservableCollection<Department> Departments
        {
            get => CompanyDB.Departments;
            set
            {
                CompanyDB.Departments = value;
            }
        }
        public Employee SelectedEmployee { get; set; }
        public Department SelectedDepartment { get; set; }
        public StateManager ChoosenSchema { get; set; }
        #endregion


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            ChoosenSchema = new StateManager(this);
            _deparmentList = new ObservableCollection<string>(CompanyDB.Departments.OrderBy(dep => dep.Title).Select(dep => dep.Title));
            //     from dep in _departments
            //     orderby dep.Title
            //     select dep.Title;
            EmployeeDetails.DeparmentList = _deparmentList;
        }


        private void OnEmployeeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0) EmployeeDetails.EmployeeDetailed = (Employee)SelectedEmployee.Clone();
        }

        private void OnDepartmentSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0) DepartmentDetails.DepartmentDetailed = (Department)SelectedDepartment.Clone();
        }


        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            if (ChoosenSchema.ShowSchema == PresentSchema.Employees)
            {
                if (CrewListView.SelectedItems.Count < 1) return;
                Crew[Crew.IndexOf(SelectedEmployee)] = EmployeeDetails.EmployeeDetailed;
            }
            else if (ChoosenSchema.ShowSchema == PresentSchema.Departments)
            {
                if (DepartmentsListView.SelectedItems.Count < 1) return;
                {
                    foreach (var one in Crew)
                    {
                        if (one.Department == SelectedDepartment.Title) one.Department = DepartmentDetails.DepartmentDetailed.Title;
                    }
                    _deparmentList[_deparmentList.IndexOf(SelectedDepartment.Title)] = DepartmentDetails.DepartmentDetailed.Title;
                    Departments[Departments.IndexOf(SelectedDepartment)] = DepartmentDetails.DepartmentDetailed;
                    EmployeeDetails.DeparmentList = _deparmentList;
                }
            }
            else return;
        }


        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (ChoosenSchema.ShowSchema == PresentSchema.Employees)
            {
                if (SelectedEmployee == null) return;
                if (MessageBox.Show("Вы действительно желаете удалить запись о сотруднике?", "Удаление записи", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _ = CompanyDB.Crew.Remove(SelectedEmployee);
                    EmployeeDetails.EmployeeDetailed = null;
                    //SelectedEmployee = null;
                }
            }
            else if (ChoosenSchema.ShowSchema == PresentSchema.Departments)
            {
                if (SelectedDepartment == null) return;
                if (MessageBox.Show("Внимание! Будут удалены все записи, связанные с данным отделением.\nПродолжить?", "Каскадное удаление записей", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    for (int i = Crew.Count - 1; i >= 0; i--)
                    {
                        if (Crew[i].Department == SelectedDepartment.Title) Crew.RemoveAt(i);
                    }
                    _ = _deparmentList.Remove(SelectedDepartment.Title);
                    _ = CompanyDB.Departments.Remove(SelectedDepartment);
                    DepartmentDetails.DepartmentDetailed = null;
                    //SelectedDepartment = null;
                }
            }
            else return;
        }


        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (ChoosenSchema.ShowSchema == PresentSchema.Employees)
            {
                var editorWindow = new EntryEditor();
                editorWindow.EntryControl.DeparmentList = EmployeeDetails.DeparmentList;
                if (editorWindow.ShowDialog() == true)
                {
                    CompanyDB.Crew.Add(editorWindow.NewEmployee);
                }
            }
            else if (ChoosenSchema.ShowSchema == PresentSchema.Departments)
            {
                var editorWindow = new EditorForDepartment();
                if (editorWindow.ShowDialog() == true)
                {
                    CompanyDB.Departments.Add(editorWindow.NewDepartment);
                    _deparmentList.Add(editorWindow.NewDepartment.Title);
                }
            }
            else return;
        }


        public class StateManager: INotifyPropertyChanged
        {
            private readonly MainWindow _host;
            private PresentSchema _showSchema;
            private List<UIElement> _stateA_employees;
            private List<UIElement> _stateB_departments;
            private Visibility _stateAVisibility;
            private Visibility _stateBVisibility;


            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }


            public ObservableCollection<PresentSchema> Schemas { get; set; } = new ObservableCollection<PresentSchema>();
            public PresentSchema ShowSchema
            {
                get => _showSchema;
                set
                {
                    _showSchema = value;
                    OnSchemaChoose_ShowStateElements1();
                    //OnSchemaChoose_ShowStateElements2(); // не работает
                    NotifyPropertyChanged();
                }
            }
            public string ShowSchemaAsString
            {
                get => _showSchema == PresentSchema.Employees ? "Сотрудники" : "Отделения";
                set
                {
                    if (value == "Сотрудники") _showSchema = PresentSchema.Employees;
                    else if (value == "Отделения") _showSchema = PresentSchema.Departments;
                }
            }
            public Visibility StateAVisibility
            {
                get => _stateAVisibility;
                set
                {
                    _stateAVisibility = value;
                    NotifyPropertyChanged();
                }
            }
            public Visibility StateBVisibility
            {
                get => _stateBVisibility;
                set
                {
                    _stateBVisibility = value;
                    NotifyPropertyChanged();
                }
            }


            public StateManager(MainWindow host)
            {
                _host = host;

                Schemas.Add(PresentSchema.Employees);
                Schemas.Add(PresentSchema.Departments);
                _showSchema = PresentSchema.Employees;

                _stateA_employees = new List<UIElement>
                {
                    _host.CrewListView,
                    _host.EmployeeDetails
                };
                _stateB_departments = new List<UIElement>
                {
                    _host.DepartmentsListView,
                    _host.DepartmentDetails
                };
                OnSchemaChoose_ShowStateElements1();
            }


            public void OnSchemaChoose_ShowStateElements1()
            {
                if (_showSchema == PresentSchema.Employees)
                {
                    foreach (var one in _stateA_employees) one.Visibility = Visibility.Visible;
                    foreach (var one in _stateB_departments) one.Visibility = Visibility.Hidden;
                }
                else if (_showSchema == PresentSchema.Departments)
                {
                    foreach (var one in _stateB_departments) one.Visibility = Visibility.Visible;
                    foreach (var one in _stateA_employees) one.Visibility = Visibility.Hidden;
                }
                else return;
            }

            public void OnSchemaChoose_ShowStateElements2()
            {
                if (_showSchema == PresentSchema.Employees)
                {
                    _stateAVisibility = Visibility.Visible;
                    _stateBVisibility = Visibility.Hidden;
                }
                else if (_showSchema == PresentSchema.Departments)
                {
                    _stateBVisibility = Visibility.Visible;
                    _stateAVisibility = Visibility.Hidden;
                }
                else return;
            }
        }

        public enum PresentSchema { Employees, Departments }
    }
}
