using Microsoft.Xna.Framework;
using System;

namespace Capstone_Project.MapStuff
{
    public class TileMap
    {
        public Tile2DArray TileArray { get; init; }

        // in px
        private readonly int width;
        public readonly int height;

        public Rectangle MapBounds => new Rectangle(0, 0, width, height);
        // width and height of the Tiles (tiles are square so w and h are the same value)
        private readonly int tileSize;

        // TODO: make sure the constructor takes Tiles in its constructor
        /// <param name="width">The width in number of tiles the map is</param>
        /// <param name="height">The height in number of tiles the map is</param>
        /// <param name="tileSize">Side length of the tiles</param>
        public TileMap(int tileWidth, int tileHeight, int tileSize, Tile[] tiles)
        {
            this.tileSize = tileSize;

            if (tiles.Length != tileWidth * tileHeight)
                throw new Exception("Number of Tiles and size of the TileMap are different");

            TileArray = new Tile2DArray(tileWidth, tileHeight, tiles);
            
            width = tileWidth * tileSize;
            height = tileHeight * tileSize;
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
