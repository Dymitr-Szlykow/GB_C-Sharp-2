using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CompanyApp.ServiceCommunication.CompanyService
{
    public partial class Employee : ICloneable
    {
        [XmlElement(Order = 20)]
        public virtual string Name_client
        {
            get
            {
                if (Name.Length == 0) return "John";
                else return Name;
            }
            set => Name = value;
        }

        [XmlElement(Order = 21)]
        public virtual string Surname_client
        {
            get
            {
                if (Surname.Length == 0) return "Doe";
                else return Surname;
            }
            set => Surname = value;
        }

        [XmlElement(Order = 22)]
        public virtual string Patronym_client
        {
            get
            {
                if (Patronym.Length == 0) return " ";
                else return Patronym;
            }
            set => Patronym = value;
        }

        [XmlElement(Order = 23)]
        public char GenderAsChar_client
        {
            get => GenderAsChar;
            set => GenderAsChar = value;
            //{
            //    if (value == 'м' || value == 'М') GenderField = GenderType.Male;
            //    else if (value == 'ж' || value == 'Ж') GenderField = GenderType.Female;
            //}
        }

        [XmlElement(Order = 24)]
        public bool IsMale_client
        {
            get => GenderAsChar == 'м' || GenderAsChar == 'М';
            set
            {
                if (value) GenderAsChar = 'М';
            }
        }

        [XmlElement(Order = 25)]
        public bool IsFemale_client
        {
            get => GenderAsChar == 'ж' || GenderAsChar == 'Ж';
            set
            {
                if (value) GenderAsChar = 'Ж';
            }
        }

        [XmlElement(Order = 26)]
        public string FullName_client
        {
            get => $"{Name_client} {Patronym_client} {Surname_client}";
            set { }
        }

        [XmlElement(Order = 27)]
        public string ShortName_client
        {
            get => $"{Name_client[0]}.{Patronym_client[0]}. {Surname_client}";
            set { }
        }


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

        public object Clone() => MemberwiseClone();
    }
}
