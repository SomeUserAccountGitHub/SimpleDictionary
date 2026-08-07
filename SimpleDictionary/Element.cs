namespace SimpleDictionary
{
    internal class Element<TKey, TValue>
    {
        public TKey Key { get; }
        public TValue Value { get; set; }
        public Element<TKey, TValue>? Next { get; set; }
        public Element(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            Next = null;
        }
        public override string ToString()
        {
            return $"({Key}, {Value})->" + (Next != null ? Next.ToString() : "");
        }

    }
}