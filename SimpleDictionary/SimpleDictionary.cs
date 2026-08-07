using System;
using System.Text;

namespace SimpleDictionary
{
    public class SimpleDictionary<Key, Value>
    {
        private const int INITIAL_SIZE = 2;
        private const double HASHING_FILLNESS = 0.5;
        public int Capacity { get; private set; } = INITIAL_SIZE;
        public int Count { get; private set; } = 0;

        private Element<Key, Value>?[] _content;

        public SimpleDictionary()
        {
            Clear();
        }

        public Value this[Key key]
        {
            get => Get(key);
            set
            {
                var curr = Find(key);
                if (curr != null)
                    curr.Value = value;
                else
                    Add(key, value);
            }
        }

        public Value Get(Key key) => TryGetValue(key, out var res)
            ? res
            : throw new KeyNotFoundException();

        public bool TryGetValue(Key key, out Value value)
        {
            value = default;

            var curr = Find(key);
            if (curr == null)
                return false;

            value = curr.Value;
            return true;
        }

        public void Add(Key key, Value value)
        {
            //var hashCode = key.GetHashCode();
            //Console.WriteLine($"hashcode for key={key}: {hashCode}");

            if (Find(key) != null)
                throw new ArgumentException($"An item with key = {key} already exists");

            if (Count + 1 > Capacity * HASHING_FILLNESS)
                Resize();

            var index = GetIndex(key, Capacity);
            var newElem = new Element<Key, Value>(key, value);
            PutAt(index, newElem, _content);
            Count++;
        }

        public void Clear()
        {
            _content = new Element<Key, Value>[INITIAL_SIZE];
            Capacity = INITIAL_SIZE;
            Count = 0;
        }

        public bool Remove(Key key)
        {
            var index = GetIndex(key, Capacity);

            var (prev, curr) = FindRemovalTarget(index, key);
            if (curr == null)
                return false;

            Relink(prev, curr, index);
            Count--;

            return true;

        }
        private int GetIndex(Key key, int size)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            //todo: following can be improved so it is stable when restarting an app.
            //In this way, dictionary can be serialized
            var hashCode = key.GetHashCode();
            return (hashCode & 0x7fffffff) % size;
        }

        (Element<Key, Value>, Element<Key, Value>) FindRemovalTarget(int index, Key key)
        {
            Element<Key, Value>? prev = null;
            var curr = _content[index];
            while (curr != null && !AreEqual(curr.Key, key))
            {
                prev = curr;
                curr = curr.Next;
            }
            return (prev, curr);
        }

        void Relink(Element<Key, Value> prev, Element<Key, Value> curr, int index)
        {
            if (prev != null)
                prev.Next = curr.Next;
            else
                _content[index] = curr.Next;
        }


        private Element<Key, Value> Find(Key key)
        {
            var index = GetIndex(key, Capacity);

            var curr = _content[index];

            while (curr != null && !AreEqual(curr.Key, key))
                curr = curr.Next;

            return curr;
        }

        private bool AreEqual<T>(T val1, T val2)
            => EqualityComparer<T>.Default.Equals(val1, val2);

        private void PutAt(int index, Element<Key, Value> newElem, Element<Key, Value>[] array)
        {
            newElem.Next = array[index];
            array[index] = newElem;
        }

        private void Resize()
        {
            if (Capacity >= int.MaxValue / 2)
                throw new Exception("Max capacity reached");
            var doubleCapacity = Capacity * 2;
            var newContent = new Element<Key, Value>[doubleCapacity];
            for (var i = 0; i < _content.Length; i++)
            {
                ProcessResizeAt(i);
            }

            _content = newContent;
            Capacity = doubleCapacity;

            void ProcessResizeAt(int ind)
            {
                var current = _content[ind];
                while (current != null)
                {
                    var newInd = GetIndex(current.Key, doubleCapacity);
                    var next = current.Next;
                    current.Next = null;
                    PutAt(newInd, current, newContent);
                    current = next;
                }
            }
        }

        public override string ToString()
        {
            var res = new StringBuilder();

            res.AppendLine($"size: {Capacity}, count: {Count}");

            for (var i = 0; i < _content.Length; i++)
            {
                if (_content[i] == null)
                    continue;
                var curr = _content[i];
                res.AppendLine($"[{i.ToString()}] " + curr.ToString());
            }
            return res.ToString();
        }
    }
}
