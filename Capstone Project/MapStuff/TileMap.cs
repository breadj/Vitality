using Capstone_Project.Fundamentals;
using Capstone_Project.GameObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Capstone_Project.MapStuff
{
    public class TileMap
    {
        public Array2D<Tile> TileArray { get; init; }
        public Array2D<bool> Walls { get; init; }       // for pathfinding AI, true = wall (impassable)
        public Vector2 Spawn { get; init; }
        public List<LevelExit> Exits { get; init; }

        // in px
        private readonly int width;
        private readonly int height;

        public Rectangle MapBounds => new Rectangle(0, 0, width, height);
        // width and height of the Tiles (tiles are square so w and h are the same value)
        private readonly int tileSize;

        public TileMap(int tileWidth, int tileHeight, int tileSize, Array2D<Tile> tiles, Array2D<bool> walls, Vector2 spawn, List<LevelExit> exits = null)
        {
            this.tileSize = tileSize;

            if (tiles.Length != tileWidth * tileHeight)
                throw new Exception("Number of Tiles and size of the TileMap are different");

            TileArray = tiles;

            width = tileWidth * tileSize;
            height = tileHeight * tileSize;
            Walls = walls;

            Spawn = spawn;
            Exits = exits ?? new List<LevelExit>();
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
