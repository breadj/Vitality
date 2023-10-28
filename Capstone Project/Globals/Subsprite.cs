using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Capstone_Project.Globals
{
    public record class Subsprite
    {
        public Texture2D SpriteSheet { get; init; }
        // The area on the SpriteSheet where the texture for this sprite is taken from
        public Rectangle Source { get; init; }   // might change the perms for mutator if it's animated

        public Subsprite(ref Texture2D spriteSheet, Rectangle source)
        {
            SpriteSheet = spriteSheet;      // IMPORTANT that the SpriteSheet is passed by reference
            Source = source;
        }
    }
}
