using System.Collections;

namespace Ravel.Values
{
    public class SingleLinkedList<T> : IReadOnlyList<T>
    {
        private T Value { get; }
        public SingleLinkedList<T>? Next { get; }
        public int Count { get; }

        public T this[int index]
        {
            get
            {
                if (index >= Count)
                {
                    throw new IndexOutOfRangeException(nameof(index));
                }
                SingleLinkedList<T>? list = this;
                for (int i = 0; i < Count - 1 - index; i++)
                {
                    list = list!.Next;
                }
                return list!.Value;
            }
        }
        public T this[Index index]
        {
            get
            {
                return index.IsFromEnd ? this[Count - index.Value] : this[index.Value];
            }
        }
        public static SingleLinkedList<T> Empty
        {
            get
            {
                return new();
            }
        }
        public SingleLinkedList(T value, SingleLinkedList<T>? next = null)
        {
            Value = value;
            if (next == null || next.Count == 0)
            {
                Next = null;
                Count = 1;
            }
            else
            {
                Next = next;
                Count = next.Count + 1;
            }
        }
        private SingleLinkedList()
        {
            Value = default!;
            Count = 0;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return GetReversed().Reverse().GetEnumerator();
        }

        public IEnumerable<T> GetReversed()
        {
            if (Count == 0)
            {
                yield break;
            }
            SingleLinkedList<T>? list = this;
            while (list != null)
            {
                yield return list.Value;
                list = list.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
