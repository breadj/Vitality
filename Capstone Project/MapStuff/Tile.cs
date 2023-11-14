using Capstone_Project.GameObjects.Hitboxes;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Capstone_Project.MapStuff
{
    public class Tile
    {
        private readonly Subsprite subsprite;
        private readonly int size;
        private readonly bool isWall;

        public Point Position { get; init; }
        public TileHitbox Hitbox { get; init; }

        public Tile(Subsprite subsprite, Point position, int size, bool isWall = false)
        {
            this.subsprite = subsprite;
            this.size = size;
            this.isWall = isWall;

            Position = position;
            if (isWall)
                Hitbox = new TileHitbox(new Point(size));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // assuming the destination Rectangle is the same as the Hitbox BoundingBox
            spriteBatch.Draw(subsprite.SpriteSheet, Hitbox.BoundingBox, subsprite.Source, Color.White, 
                0f, Vector2.Zero, SpriteEffects.None, 0f);
        }
    }
}
