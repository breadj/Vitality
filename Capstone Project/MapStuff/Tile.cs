using static Capstone_Project.Globals.Globals;
using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Capstone_Project.GameObjects;

namespace Capstone_Project.MapStuff
{
    public class Tile : GameObjects.Interfaces.IDrawable, ICollidable
    {
        public bool Visible { get; set; } = true;
        public Subsprite Subsprite { get; init; }
        public Rectangle Destination { get; init; }
        public float Rotation => 0f;
        public Vector2 Origin { get; init; }            // Tiles have their positions as the top-left of their sprite
        public float Layer { get; set; } = 0.001f;

        public bool Active { get; set; } = false;
        public Rectangle Hitbox { get; init; }
        public bool IsCircle { get; } = false;          // always a square
        public float Radius => size / 2f;

        public Point Position { get; init; }
        private int size { get; init; }


        public Tile(Subsprite subsprite, Point position, int size, bool isWall = false)
        {
            Subsprite = subsprite;
            Destination = new Rectangle(position, new Point(size));
            Origin = Vector2.Zero;

            Active = isWall;
            Hitbox = Destination;

            Position = position;
            this.size = size;
        }

        public void Draw()
        {
            // assuming the destination Rectangle is the same as the Hitbox BoundingBox
            spriteBatch.Draw(Subsprite.SpriteSheet, Destination, Subsprite.Source, Color.White, 
                Rotation, Origin, SpriteEffects.None, Layer);

            //spriteBatch.DrawString(Globals.Globals.DebugFont, IsWall.ToString(), Position.ToVector2(), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.2f);
            /*if (Active)
                spriteBatch.Draw(BLANK, Hitbox, null, new Color(Color.Pink, 0.4f), 0f, Vector2.Zero, SpriteEffects.None, 0.2f);*/
        }

        // this shouldn't actually ever have to be called, since it will always be 'other' that checks and handles collision with a Tile
        public CollisionDetails CollidesWith(ICollidable other)
        {
            throw new System.NotImplementedException();
        }
    }
}
