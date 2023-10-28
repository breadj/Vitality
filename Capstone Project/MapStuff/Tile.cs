using Capstone_Project.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Capstone_Project.MapStuff
{
    public class Tile
    {
        private readonly Subsprite subsprite;

        public Tile(Subsprite subsprite)
        {
            this.subsprite = subsprite;
        }


        public void Draw(SpriteBatch spriteBatch, Rectangle destination)
        {
            spriteBatch.Draw(subsprite.SpriteSheet, destination, subsprite.Source, Color.White, 
                0f, Vector2.Zero, SpriteEffects.None, 0f);
        }
    }
}
