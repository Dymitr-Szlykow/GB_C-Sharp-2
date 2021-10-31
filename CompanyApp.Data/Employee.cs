using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyApp.Data
{
    public class Employee : DatabaseEntity
    {
        protected string _name;
        protected string _surname;
        protected string _patronym;
        protected string _birthDate;
        protected GenderType _gender;
        protected int _salary;
        protected string _department;

        public static readonly string tablename = "Employees";

        #region СВОЙСТВА
        public virtual string Name
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
        public virtual string Surname
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
        public virtual string Patronym
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
        public virtual string BirthDate
        {
            get => _birthDate;
            set
            {
                _birthDate = value;
                NotifyPropertyChanged();
            }
        }
        public virtual GenderType Gender
        {
            get => _gender;
            set
            {
                _gender = value;
                NotifyPropertyChanged();
            }
        }
        public virtual char GenderAsChar
        {
            get => Gender == GenderType.Male ? 'М' : 'Ж';
            set
            {
                if (value == 'м' || value == 'М') Gender = GenderType.Male;
                else if (value == 'ж' || value == 'Ж') Gender = GenderType.Female;
            }
        }
        public virtual bool IsMale
        {
            get => Gender == GenderType.Male;
            set
            {
                if (value) Gender = GenderType.Male;
            }
        }
        public virtual bool IsFemale
        {
            get => Gender == GenderType.Female;
            set
            {
                if (value) Gender = GenderType.Female;
            }
        }
        public virtual int Salary
        {
            get => _salary;
            set
            {
                _salary = value;
                NotifyPropertyChanged();
            }
        }
        public virtual string Department
        {
            get => _department;
            set
            {
                _department = value;
                NotifyPropertyChanged();
            }
        }
        public virtual string FullName
        {
            get => $"{Name} {Patronym} {Surname}";
            set { }
        }
        public virtual string ShortName
        {
            get => $"{Name[0]}.{Patronym[0]}. {Surname}";
            set { }
        }

        public override int ID { get; set; }
        public override string Tablename { get; } = tablename;
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
    }

    public enum GenderType { Male, Female }
}
