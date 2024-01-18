
namespace Capstone_Project.Fundamentals
{
    public class Array2D<T>
    {
        private readonly T[] arr;
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

        public T this[int x, int y] => arr[x + (y * Width)];
        public T this[int i] => arr[i];
        public (int x, int y) FindPosition(int index) => new(index % Width, index / Width);
        public int Length => arr.Length;
    }
}
