using Capstone_Project.Fundamentals;
using Microsoft.Xna.Framework;
using System;

namespace Capstone_Project.MapStuff
{
    public class TileMap
    {
        public Array2D<Tile> TileArray { get; init; }
        public Array2D<bool> Walls { get; init; }       // for pathfinding AI

        // in px
        private readonly int width;
        private readonly int height;

        public Rectangle MapBounds => new Rectangle(0, 0, width, height);
        // width and height of the Tiles (tiles are square so w and h are the same value)
        private readonly int tileSize;

        /// <param name="width">The width in number of tiles the map is</param>
        /// <param name="height">The height in number of tiles the map is</param>
        /// <param name="tileSize">Side length of the tiles</param>
        public TileMap(int tileWidth, int tileHeight, int tileSize, Tile[] tiles)
        {
            this.tileSize = tileSize;

            if (tiles.Length != tileWidth * tileHeight)
                throw new Exception("Number of Tiles and size of the TileMap are different");

            TileArray = new Array2D<Tile>(tileWidth, tileHeight, tiles);
            
            width = tileWidth * tileSize;
            height = tileHeight * tileSize;
        }

        public TileMap(int tileWidth, int tileHeight, int tileSize, Array2D<Tile> tiles, Array2D<bool> walls)
        {
            this.tileSize = tileSize;

            if (tiles.Length != tileWidth * tileHeight)
                throw new Exception("Number of Tiles and size of the TileMap are different");

            TileArray = tiles;

            width = tileWidth * tileSize;
            height = tileHeight * tileSize;
            Walls = walls;
        }

        // probably deprecated
        public void Draw()
        {
            for (int y = 0; y < TileArray.Height; y++)
                for (int x = 0; x < TileArray.Width; x++)
                    if (TileArray[x, y] != null)
                        TileArray[x, y].Draw();
        }
    }
}
