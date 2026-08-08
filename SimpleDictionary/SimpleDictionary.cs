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

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public SimpleDictionary() => Clear();
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public void Clear()
        {
            _content = new Element<Key, Value>[INITIAL_SIZE];
            Capacity = INITIAL_SIZE;
            Count = 0;
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
#pragma warning disable CS8601 // Possible null reference assignment.
            value = default;
#pragma warning restore CS8601 // Possible null reference assignment.

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
                throw new ArgumentException($"An item with key = {key?.ToString()} already exists");

            if (Count + 1 > Capacity * HASHING_FILLNESS)
                Resize();

            var index = GetIndex(key, Capacity);
            var newElem = new Element<Key, Value>(key, value);
            PutAt(index, newElem, _content);
            Count++;
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
        private static int GetIndex(Key key, int size)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            //todo: following can be improved so it is stable when restarting an app.
            //In that way, dictionary can be serialized
            var hashCode = key.GetHashCode();
            return (hashCode & 0x7fffffff) % size;
        }

        (Element<Key, Value>? prev, Element<Key, Value>? curr) FindRemovalTarget(int index, Key key)
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

        void Relink(Element<Key, Value>? prev, Element<Key, Value> curr, int index)
        {
            if (prev != null)
                prev.Next = curr.Next;
            else
                _content[index] = curr.Next;
        }


        private Element<Key, Value>? Find(Key key)
        {
            var index = GetIndex(key, Capacity);

            var curr = _content[index];

            while (curr != null && !AreEqual(curr.Key, key))
                curr = curr.Next;

            return curr;
        }

        private static bool AreEqual<T>(T val1, T val2)
            => EqualityComparer<T>.Default.Equals(val1, val2);

        private static void PutAt(int index, Element<Key, Value> newElem, Element<Key, Value>?[] array)
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
                ProcessResizeAt(i);

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
