
using System.Collections;
using System.Collections.Immutable;

namespace Rogue.Utils
{
    public class SafeHashSet<T>: IImmutableSet<T>
    {
        public int Count { 
            get 
            {
                lock (_lock)
                {
                    return _set.Count;
                }
            } 
        }

        private ImmutableHashSet<T> _set = [];

        private Lock _lock = new (); // Used to make sure that (hopefully) no operations can happen while the state of _set is being read from

        public IImmutableSet<T> Add(T value)
        {
            ImmutableInterlocked.Update(ref _set, (set) => set.Add(value));
            return _set;
        }

        public IImmutableSet<T> Clear()
        {
            ImmutableInterlocked.Update(ref _set, (set) => set.Clear());
            return _set;
        }

        public bool Contains(T value)
        {
            lock (_lock)
            {
                return _set.Contains(value);
            }
        }

        public IImmutableSet<T> Except(IEnumerable<T> values)
        {
            ImmutableInterlocked.Update(ref _set, (set) => set.Except(values));
            return _set;
        }

        public IImmutableSet<T> Intersect(IEnumerable<T> values)
        {
            ImmutableInterlocked.Update(ref _set, (set) => set.Intersect(values));
            return _set;
        }

        public bool IsProperSubsetOf(IEnumerable<T> values)
        {
            lock (_lock)
            {
               return _set.IsProperSubsetOf(values);
            }
        }

        public bool IsProperSupersetOf(IEnumerable<T> values)
        {
            lock (_lock)
            {
                return _set.IsProperSupersetOf(values);
            }
        }

        public bool IsSubsetOf(IEnumerable<T> values)
        {
            lock (_lock)
            {
                return _set.IsSubsetOf(values);
            }
        }

        public bool IsSupersetOf(IEnumerable<T> values)
        {
            lock (_lock)
            {
                return _set.IsSupersetOf(values);
            }
        }

        public bool Overlaps(IEnumerable<T> values)
        {
            lock (_lock)
            {
                return _set.Overlaps(values);
            }
        }

        public IImmutableSet<T> Remove(T value)
        {
            ImmutableInterlocked.Update(ref _set, (set) => set.Remove(value));
            return _set;
        }

        public bool SetEquals(IEnumerable<T> values)
        {
            lock (_lock)
            {
                return _set.SetEquals(values);
            }
        }

        public IImmutableSet<T> SymmetricExcept(IEnumerable<T> values)
        {
            ImmutableInterlocked.Update(ref _set, (set) => set.SymmetricExcept(values));
            return _set;
        }

        public bool TryGetValue(T equalValue, out T value)
        {
            lock (_lock)
            {
                return _set.TryGetValue(equalValue, out value);
            }
        }

        public IImmutableSet<T> Union(IEnumerable<T> values)
        {
            ImmutableInterlocked.Update(ref _set, (set) => set.Union(values));
            return _set;
        }

        public IEnumerator<T> GetEnumerator() => _set.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}