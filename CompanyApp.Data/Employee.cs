using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CompanyApp.Data
{
    public class Employee : INotifyPropertyChanged, ICloneable
    {
        protected string _name;
        protected string _surname;
        protected string _patronym;
        protected string _birthDate;
        protected char _gender;
        protected int _salary;
        protected string _department;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region СВОЙСТВА
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }
        public string Surname
        {
            get => _surname;
            set
            {
                _surname = value;
                NotifyPropertyChanged();
            }
        }
        public string Patronym
        {
            get => _patronym;
            set
            {
                _patronym = value;
                NotifyPropertyChanged();
            }
        }
        public string BirthDate
        {
            get => _birthDate;
            set
            {
                _birthDate = value;
                NotifyPropertyChanged();
            }
        }
        public char Gender
        {
            get => _gender;
            set
            {
                if (value == 'м' || value == 'ж')
                    _gender = value;
                else
                    _gender = ' ';
                NotifyPropertyChanged();
            }
        }
        public bool IsMale
        {
            get => _gender == 'м';
            set
            {
                if (value) Gender = 'м';
            }
        }
        public bool IsFemale
        {
            get => _gender == 'ж';
            set
            {
                if (value) Gender = 'ж';
            }
        }
        public int Salary
        {
            get => _salary;
            set
            {
                _salary = value;
                NotifyPropertyChanged();
            }
        }
        public string Department
        {
            get => _department;
            set
            {
                _department = value;
                NotifyPropertyChanged();
            }
        }
        public string FullName
        {
            get => $"{_name} {_patronym} {_surname}";
        }
        public string ShortName
        {
            get => $"{_name[0]}.{_patronym[0]}. {_surname}";
        }
        #endregion

        public Employee() { }
        public Employee(string name, string surname, string patronym, string birthdate, char gender, int salary, string department)
        {
            Name = name;
            Surname = surname;
            Patronym = patronym;
            BirthDate = birthdate;
            Gender = gender;
            Salary = salary;
            Department = department;
        }

        public object Clone() => this.MemberwiseClone();
    }
}
