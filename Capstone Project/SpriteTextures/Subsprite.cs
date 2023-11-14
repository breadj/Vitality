using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Capstone_Project.SpriteTextures
{
    public record class Subsprite
    {
        public Texture2D SpriteSheet { get; init; }
        // The area on the SpriteSheet where the texture for this sprite is taken from
        public Rectangle Source { get; init; }   // might change the perms for mutator if it's animated

        public Subsprite(Texture2D spriteSheet, Rectangle source)
        {
            SpriteSheet = spriteSheet;
            Source = source;
        }
    }
}
