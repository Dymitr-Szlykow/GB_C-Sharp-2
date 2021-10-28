using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CompanyApp.Data
{
    public class Employee : IDatabaseEntity, INotifyPropertyChanged, ICloneable
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
            get
            {
                if (_name.Length == 0) return "John";
                else return _name;
            }
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }
        public string Surname
        {
            get
            {
                if (_surname.Length == 0) return "Doe";
                else return _surname;
            }
            set
            {
                _surname = value;
                NotifyPropertyChanged();
            }
        }
        public string Patronym
        {
            get
            {
                if (_patronym.Length == 0) return " ";
                else return _patronym;
            }
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
            get => $"{Name} {Patronym} {Surname}";
            set { }
        }
        public string ShortName
        {
            get => $"{Name[0]}.{Patronym[0]}. {Surname}";
            set { }
        }

        public int ID { get; set; }
        public string Tablename { get => "Employees"; }
        #endregion

        public Employee() { }
        public Employee(int id, string name, string surname, string patronym, string birthdate, GenderType gender, int salary, string department)
        {
            ID = id;
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
