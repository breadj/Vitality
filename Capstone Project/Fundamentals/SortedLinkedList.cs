using System;
using System.Collections.Generic;
using System.Linq;

namespace Capstone_Project.Fundamentals
{
    public class SortedLinkedList<T> : LinkedList<T>, ICollection<T>
    {
        public readonly IComparer<T> Comparer = default;

        public SortedLinkedList() : base() { }

        public SortedLinkedList(IComparer<T> comparer)
        {
            Comparer = comparer;
        }

        public SortedLinkedList(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            var sortedCollection = collection.OrderBy(x => x, Comparer);
            foreach (var elem in sortedCollection)
            {
                this.AddLast(elem);
            }
        }

        public SortedLinkedList(IEnumerable<T> collection, IComparer<T> comparer)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            Comparer = comparer;

            var sortedCollection = collection.OrderBy(x => x, Comparer);
            foreach (var elem in sortedCollection)
            {
                this.AddLast(elem);
            }
        }

        public void SortedInsert(T item)
        {
            if (!this.Any())
            {
                this.AddFirst(item);
                return;
            }

            if (Comparer.Compare(this.Last.Value, item) < 0)
            {
                this.AddLast(item);
                return;
            }

            for (var node = this.First; node != null; node = node.Next)
                if (Comparer.Compare(item, node.Value) < 0)
                    this.AddBefore(node, item);
        }

        public void Add(T item)
        {
            this.SortedInsert(item);
        }

        void ICollection<T>.Add(T item)
        {
            this.SortedInsert(item);
        }
    }
}
