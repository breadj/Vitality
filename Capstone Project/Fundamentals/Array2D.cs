using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;

namespace Capstone_Project.Fundamentals
{
    public class Array2D<T> : ICollection<T>, IEnumerable<T>
    {
        private T[] arr { get; set; }
        public int Width { get; init; }
        public int Height { get; init; }

        public Array2D(int width, int height, T[] arr)
        {
            Width = width;
            Height = height;
            this.arr = arr;
        }

        public Array2D(int width, int height)
        {
            Width = width;
            Height = height;
            arr = new T[Width * Height];
        }

        public T this[int i]
        {
            get => arr[i];
            set => arr[i] = value;
        }
        public T this[int x, int y]
        {
            get => arr[x + (y* Width)];
            set => arr[x + (y * Width)] = value;
        }
        public T this[Point p]
        {
            get => this[p.X, p.Y];
            set => this[p.X, p.Y] = value;
        }
        public (int x, int y) FindPosition(int index) => new(index % Width, index / Width);

        public void Add(T item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            System.Array.Clear(arr);
        }

        public bool Contains(T item)
        {
            foreach (T x in arr)
                if (item.Equals(x))
                    return true;
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            arr.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)arr.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return arr.GetEnumerator();
        }

        public int Length => arr.Length;

        public int Count => arr.Length;

        public bool IsReadOnly => false;

        public static implicit operator System.Array(Array2D<T> arr2d) => arr2d.arr;
    }
}
