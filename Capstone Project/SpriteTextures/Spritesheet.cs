﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Capstone_Project.SpriteTextures
{
    public class Spritesheet
    {
        public Texture2D Texture { get; init; }
        private int tileSize;   // sidelength of a tile in px
        private int columns;    // width (x) in tiles
        private int rows;       // height (y) in tiles

        public Spritesheet(Texture2D texture, int tileSize)
        {
            Texture = texture;
            this.tileSize = tileSize;
            rows = texture.Bounds.Height / tileSize;
            columns = texture.Bounds.Width / tileSize;
        }

        public Spritesheet(Texture2D texture, int columns, int rows)
        {
            Texture = texture;
            this.columns = columns;
            this.rows = rows;
            tileSize = texture.Bounds.Width / columns;
        }

        public Subsprite GetSubsprite(int x, int y)
        {
            if (x > columns || y > rows)
            {
                Console.WriteLine("Sprite index out of Spritesheet bounds.");
                return null;
            }
            // returns a Subsprite referencing the same Texture, with the source rectangle at the specified tile's coordinates (taking into account the tileSize)
            return new Subsprite(Texture, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize));
        }

        public Subsprite GetSubsprite(int index)
        {
            // converts index to coordinates in the context of a 2D array of side lengths (columns, rows)
            var xy = Globals.Utility.IndexToCoord(index, columns, rows);
            return GetSubsprite(xy.X, xy.Y);
        }
    }
}
