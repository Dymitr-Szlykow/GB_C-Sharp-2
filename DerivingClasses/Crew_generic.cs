using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DerivingClasses
{
    abstract class CrewList<T> : ICollection, ICollection<T>, IEnumerable, IEnumerable<T>
    {
        public abstract int Count { get; }
        public abstract object SyncRoot { get; }
        public abstract bool IsSynchronized { get; }
        public abstract bool IsReadOnly { get; }

        public abstract void Add(T item);
        public abstract void Clear();
        public abstract bool Contains(T item);
        public abstract void CopyTo(Array array, int index);
        public abstract void CopyTo(T[] array, int arrayIndex);
        public abstract bool Remove(T item);

        public abstract IEnumerator GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }


    class CrewList1<T> : CrewList<T>, IEnumerable
    {
        private T[] _list;

        public CrewList1()
        {
            _list = new T[0];
        }

        public T this[int index] => _list[index];
        public override int Count => _list.Length;
        public override bool IsReadOnly => _list.IsReadOnly;
        public override object SyncRoot => _list.SyncRoot;
        public override bool IsSynchronized => _list.IsSynchronized;

        public override void Add(T item)
        {
            T[] buffer = new T[_list.Length + 1];
            _list.CopyTo(buffer, 0);
            buffer[buffer.Length - 1] = item;
            _list = new T[buffer.Length];
            buffer.CopyTo(_list, 0);
        }

        public override void Clear() => _list = new T[0];
        public override bool Contains(T item) => Array.IndexOf(_list, item) >= 0;
        public override void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        public override void CopyTo(Array array, int index) => _list.CopyTo(array, index);

        public override bool Remove(T item)
        {
            int toRemove = Array.IndexOf(_list, item);
            if (toRemove < 0) return false;

            T[] buffer = new T[_list.Length - 1];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = _list[i < toRemove ? i : i + 1];
            }
            _list = new T[buffer.Length];
            buffer.CopyTo(_list, 0);
            return true;
        }

        public void Sort() => Array.Sort(_list);
        public void Sort(IComparer<T> comparer) => Array.Sort(_list, comparer);

        public override IEnumerator GetEnumerator() => ((IEnumerable<T>)_list).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }



    class CrewList2<T> : CrewList<T>, IReadOnlyList<T>, IReadOnlyCollection<T>
    {
        private List<T> _list;

        public CrewList2()
        {
            _list = new List<T>();
        }

        public T this[int index] => _list[index];
        public T[] AsArray() => _list.ToArray();

        public override int Count => _list.Count;
        public override bool IsReadOnly => _list.ToArray().IsReadOnly;
        public override object SyncRoot => _list.ToArray().SyncRoot;
        public override bool IsSynchronized => _list.ToArray().IsSynchronized;

        public override void Add(T item) => _list.Add(item);
        public override void Clear() => _list.Clear();
        public override bool Contains(T item) => _list.Contains(item);
        public override void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        public override void CopyTo(Array array, int index) => ((ICollection)_list).CopyTo(array, index);
        public override bool Remove(T item) => _list.Remove(item);
        public void Sort() => _list.Sort();
        public void Sort(IComparer<T> comparer) => _list.Sort(comparer);

        public override IEnumerator GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }
}
