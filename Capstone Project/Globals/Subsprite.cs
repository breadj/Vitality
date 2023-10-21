using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Project.Globals
{
    public record class Subsprite
    {
        public Texture2D SpriteSheet { get; init; }
        public Rectangle Source { get; init; }   // might change the perms for mutator if it's animated

        public Subsprite(ref Texture2D spriteSheet, Rectangle source)
        {
            SpriteSheet = spriteSheet;      // IMPORTANT that the SpriteSheet is passed by reference
            Source = source;
        }
    }
}
