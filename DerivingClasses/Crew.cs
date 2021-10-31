using Adendum.DerivingClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addendum.DerivingClasses
{   abstract class CrewList : ICollection, ICollection<Employee>, IEnumerable
    {
        public abstract int Count { get; }
        public abstract object SyncRoot { get; }
        public abstract bool IsSynchronized { get; }
        public abstract bool IsReadOnly { get; }

        public abstract void Add(Employee item);
        public abstract void Clear();
        public abstract bool Contains(Employee item);
        public abstract void CopyTo(Array array, int index);
        public abstract void CopyTo(Employee[] array, int arrayIndex);
        public abstract bool Remove(Employee item);

        public abstract IEnumerator GetEnumerator();
        IEnumerator<Employee> IEnumerable<Employee>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }


    class CrewList1 : CrewList, IEnumerable
    {
        private Employee[] _list;

        public CrewList1()
        {
            _list = new Employee[0];
        }

        public Employee this[int index] => _list[index];
        public override int Count => _list.Length;
        public override bool IsReadOnly => _list.IsReadOnly;
        public override object SyncRoot => _list.SyncRoot;
        public override bool IsSynchronized => _list.IsSynchronized;

        public override void Add(Employee item)
        {
            Employee[] buffer = new Employee[_list.Length + 1];
            _list.CopyTo(buffer, 0);
            buffer[buffer.Length - 1] = item;
            _list = new Employee[buffer.Length];
            buffer.CopyTo(_list, 0);
        }

        public override void Clear() =>_list = new Employee[0];
        public override bool Contains(Employee item) => Array.IndexOf(_list, item) >= 0;
        public override void CopyTo(Employee[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        public override void CopyTo(Array array, int index) => _list.CopyTo(array, index);

        public override bool Remove(Employee item)
        {
            int toRemove = Array.IndexOf(_list, item);
            if (toRemove < 0) return false;

            Employee[] buffer = new Employee[_list.Length - 1];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = _list[i < toRemove ? i : i + 1];
            }
            _list = new Employee[buffer.Length];
            buffer.CopyTo(_list, 0);
            return true;
        }

        public void Sort() => Array.Sort(_list);
        public void Sort(IComparer<Employee> comparer) => Array.Sort(_list, comparer);

        public override IEnumerator GetEnumerator() => ((IEnumerable<Employee>)_list).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }



    class CrewList2 : CrewList, IReadOnlyList<Employee>, IReadOnlyCollection<Employee>
    {
        private List<Employee> _list;

        public CrewList2()
        {
            _list = new List<Employee>();
        }

        public Employee this[int index] => _list[index];
        public Employee[] AsArray() => _list.ToArray();

        public override int Count => _list.Count;
        public override bool IsReadOnly => _list.ToArray().IsReadOnly;
        public override object SyncRoot => _list.ToArray().SyncRoot;
        public override bool IsSynchronized => _list.ToArray().IsSynchronized;

        public override void Add(Employee item) => _list.Add(item);
        public override void Clear() => _list.Clear();
        public override bool Contains(Employee item) => _list.Contains(item);
        public override void CopyTo(Employee[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        public override void CopyTo(Array array, int index) => ((ICollection)_list).CopyTo(array, index);
        public override bool Remove(Employee item) => _list.Remove(item);
        public void Sort() => _list.Sort();
        public void Sort(IComparer<Employee> comparer) => _list.Sort(comparer);

        public override IEnumerator GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }
}
