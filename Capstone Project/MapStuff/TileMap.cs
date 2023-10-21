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
        public Tile[,] tileMap;

        // TODO: make sure the constructor takes Tiles in its constructor
        public TileMap(int width, int height)
        {
            tileMap = new Tile[width, height];
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Tile tile in tileMap)
                tile.Draw(spriteBatch);
        }
    }
}
