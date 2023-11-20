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
        public readonly bool IsWall;

        public Point Position { get; init; }
        public TileHitbox Hitbox { get; init; }

        public Tile(Subsprite subsprite, Point position, int size, bool isWall = false)
        {
            this.subsprite = subsprite;
            this.size = size;
            IsWall = isWall;

            Position = position;
            Hitbox = new TileHitbox(this, new Point(size));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // assuming the destination Rectangle is the same as the Hitbox BoundingBox
            spriteBatch.Draw(subsprite.SpriteSheet, new Rectangle(Position, new Point(size)), subsprite.Source, Color.White, 
                0f, Vector2.Zero, SpriteEffects.None, 0f);

            //spriteBatch.DrawString(Globals.Globals.DebugFont, IsWall.ToString(), Position.ToVector2(), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.2f);
            if (IsWall)
                spriteBatch.Draw(Globals.Globals.BLANK, Hitbox.BoundingBox, null, new Color(Color.Pink, 0.4f), 0f, Vector2.Zero, SpriteEffects.None, 0.2f);
        }
    }
}
