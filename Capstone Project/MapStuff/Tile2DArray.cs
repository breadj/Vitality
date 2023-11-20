using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Project.MapStuff
{
    public class Tile2DArray
    {
        private Tile[] tiles;
        public int Width { get; init; }
        public int Height { get; init; }

        public Tile2DArray(int width, int height, Tile[] tiles)
        {
            Width = width;
            Height = height;
            this.tiles = tiles;
        }

        public Tile this[int x, int y] => tiles[x + (y * Width)];
        public Tile this[int i] => tiles[i];
        public Point FindPosition(int index) => new(index % Width, index / Width);
        public int Length => Width * Height;
    }
}
