using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Project.MapStuff
{
    public class TileMap
    {
        private Tile2DArray tileMap;

        private readonly int width;
        private readonly int height;
        // width and height of the Tiles (tiles are square so w and h are the same value)
        private readonly int tileSize;

        // TODO: make sure the constructor takes Tiles in its constructor
        /// <param name="width">The width in number of tiles the map is</param>
        /// <param name="height">The height in number of tiles the map is</param>
        /// <param name="tileSize">Side length of the tiles</param>
        public TileMap(int width, int height, int tileSize, Tile[] tiles)
        {
            this.tileSize = tileSize;

            if (tiles.Length != width * height)
                throw new Exception("Number of Tiles and size of the TileMap are different");

            tileMap = new Tile2DArray(width, height, tiles);
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < tileMap.Height; y++)
                for (int x = 0; x < tileMap.Width; x++)
                    if (tileMap[x, y] != null)
                        tileMap[x, y].Draw(spriteBatch, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize));
        }
    }
}
