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
        protected GenderType _gender;
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
        public GenderType Gender
        {
            get => _gender;
            set
            {
                _gender = value;
                NotifyPropertyChanged();
            }
        }
        public char GenderAsChar
        {
            get => _gender == GenderType.Male ? 'М' : 'Ж';
            set
            {
                if (value == 'м' || value == 'М') _gender = GenderType.Male;
                else if (value == 'ж' || value == 'Ж') _gender = GenderType.Female;
            }
        }
        public bool IsMale
        {
            get => _gender == GenderType.Male;
            set
            {
                if (value) _gender = GenderType.Male;
            }
        }
        public bool IsFemale
        {
            get => _gender == GenderType.Female;
            set
            {
                if (value) Gender = GenderType.Female;
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
            set { }
        }
        public string ShortName
        {
            get => $"{_name[0]}.{_patronym[0]}. {_surname}";
            set { }
        }
        #endregion

        public Employee() { }
        public Employee(string name, string surname, string patronym, string birthdate, GenderType gender, int salary, string department)
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

    public enum GenderType { Male, Female }
}
