using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace ripple.Model
{
    public class DependencyList : IList<Dependency>
    {
        private readonly IList<Dependency> _inner = new List<Dependency>();

        public IEnumerator<Dependency> GetEnumerator()
        {
            return _inner.OrderBy(x => x.Name).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Dependency item)
        {
            _inner.Fill(item);
        }

        public void Clear()
        {
            _inner.Clear();
        }

        public bool Contains(Dependency item)
        {
            return _inner.Contains(item);
        }

        public void CopyTo(Dependency[] array, int arrayIndex)
        {
            _inner.CopyTo(array, arrayIndex);
        }

        public bool Remove(Dependency item)
        {
            return _inner.Remove(item);
        }

        public int Count { get { return _inner.Count; } }
        public bool IsReadOnly { get { return _inner.IsReadOnly; } }

        public int IndexOf(Dependency item)
        {
            return _inner.IndexOf(item);
        }

        public void Insert(int index, Dependency item)
        {
            _inner.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _inner.RemoveAt(index);
        }

        public Dependency this[int index]
        {
            get { return _inner[index]; }
            set { _inner[index] = value; }
        }

        public override string ToString()
        {
            return "Count: {0}".ToFormat(Count);
        }
    }
}