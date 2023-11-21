using static Capstone_Project.Globals.Globals;
using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Capstone_Project.MapStuff
{
    public class Tile : GameObjects.Interfaces.IDrawable, ICollidable
    {
        public bool Visible { get; set; } = true;
        public Subsprite Subsprite { get; init; }
        public Rectangle Destination { get; init; }
        public Vector2 Origin => Vector2.Zero;      // Tiles have their positions as the top-left of their sprite
        public float Layer { get; set; } = 0.001f;

        public bool Active { get; set; } = false;
        public Rectangle Hitbox { get; init; }

        public Point Position { get; init; }

        public Tile(Subsprite subsprite, Point position, int size, bool isWall = false)
        {
            Subsprite = subsprite;
            Destination = new Rectangle(position, new Point(size));

            Active = isWall;
            Hitbox = Destination;

            Position = position;
        }

        public void Draw()
        {
            // assuming the destination Rectangle is the same as the Hitbox BoundingBox
            spriteBatch.Draw(Subsprite.SpriteSheet, Destination, Subsprite.Source, Color.White, 
                0f, Origin, SpriteEffects.None, Layer);

            //spriteBatch.DrawString(Globals.Globals.DebugFont, IsWall.ToString(), Position.ToVector2(), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.2f);
            if (Active)
                spriteBatch.Draw(BLANK, Hitbox, null, new Color(Color.Pink, 0.4f), 0f, Vector2.Zero, SpriteEffects.None, 0.2f);
        }
    }
}
