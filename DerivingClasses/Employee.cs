using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adendum.DerivingClasses
{
    abstract class Employee : IComparable<Employee>
    {
        public string Name { get; private set; }
        public int Salary { get; private set; }

        public Employee(string name, int salary)
        {
            Name = name;
            Salary = salary;
        }

        public abstract int GetMonthAverageSalary();


        public int CompareTo(Employee other) => Name.CompareTo(other.Name);

        public static int CompareByAverageSalary(Employee one, Employee another)
        {
            var comparer = new SalaryComparer();
            return comparer.Compare(one, another);
        }

        public int CompareByAverageSalaryTo(Employee other)
        {
            var comparer = new SalaryComparer();
            return comparer.Compare(this, other);
        }

        public static bool operator >(Employee one, Employee another) => one.GetMonthAverageSalary() > another.GetMonthAverageSalary();
        public static bool operator <(Employee one, Employee another) => one.GetMonthAverageSalary() < another.GetMonthAverageSalary();
        public static bool operator ==(Employee one, Employee another) => one.GetMonthAverageSalary() == another.GetMonthAverageSalary();
        public static bool operator !=(Employee one, Employee another) => one.GetMonthAverageSalary() != another.GetMonthAverageSalary();


        public class SalaryComparer : IComparer<Employee>
        {
            public int Compare(Employee one, Employee another)
            {
                return one.GetMonthAverageSalary() - another.GetMonthAverageSalary();
            }
        }
    }



    class HourPaid : Employee
    {
        public HourPaid(string name, int salary) : base(name, salary) { }

        public override int GetMonthAverageSalary()
        {
            return Convert.ToInt32(20.8 * 8 * Salary);
        }
    }


    class FixPaid : Employee
    {
        public FixPaid(string name, int salary) : base(name, salary) { }

        public override int GetMonthAverageSalary()
        {
            return Salary;
        }
    }
}
