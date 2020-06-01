using System;
using System.Collections.Generic;
using System.Linq;

namespace JToolbox.Core
{
    public class ConcurrentList<T>
    {
        private readonly IList<T> internalList;
        private readonly object lockObject = new object();

        public ConcurrentList()
        {
            internalList = new List<T>();
        }

        public ConcurrentList(IEnumerable<T> list)
        {
            internalList = list.ToList();
        }

        public T this[int index]
        {
            get
            {
                return LockAndGet(l => l[index]);
            }
            set
            {
                LockAndCommand(l => l[index] = value);
            }
        }

        public int Count
        {
            get
            {
                return LockAndQuery(l => l.Count);
            }
        }

        public void Add(T item)
        {
            LockAndCommand(l => l.Add(item));
        }

        public void Clear()
        {
            LockAndCommand(l => l.Clear());
        }

        public bool Contains(T item)
        {
            return LockAndQuery(l => l.Contains(item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            LockAndCommand(l => l.CopyTo(array, arrayIndex));
        }

        public int IndexOf(T item)
        {
            return LockAndQuery(l => l.IndexOf(item));
        }

        public void Insert(int index, T item)
        {
            LockAndCommand(l => l.Insert(index, item));
        }

        public bool Remove(T item)
        {
            return LockAndQuery(l => l.Remove(item));
        }

        public void RemoveAt(int index)
        {
            LockAndCommand(l => l.RemoveAt(index));
        }

        public void ForEach(Action<IList<T>, T> action)
        {
            lock (lockObject)
            {
                foreach (var item in internalList)
                {
                    action(internalList, item);
                }
            }
        }

        public T[] ToArray()
        {
            lock (lockObject)
            {
                return internalList.ToArray();
            }
        }

        protected virtual void LockAndCommand(Action<IList<T>> action)
        {
            lock (lockObject)
            {
                action(internalList);
            }
        }

        protected virtual T LockAndGet(Func<IList<T>, T> func)
        {
            lock (lockObject)
            {
                return func(internalList);
            }
        }

        protected virtual TReturn LockAndQuery<TReturn>(Func<IList<T>, TReturn> query)
        {
            lock (lockObject)
            {
                return query(internalList);
            }
        }
    }
}