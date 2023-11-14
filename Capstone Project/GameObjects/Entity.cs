using Capstone_Project.GameObjects.Hitboxes;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Capstone_Project.GameObjects
{
    public abstract class Entity
    {
        protected Subsprite subsprite;
        protected int size = 0;

        public Vector2 Position { get; protected set; }
        public Vector2 Direction { get; protected set; }
        public Vector2 Velocity { get; protected set; }

        public IHitbox Hitbox { get; protected set; }

        public Entity(Subsprite subsprite, Vector2 position, Vector2? direction = null) 
        {
            this.subsprite = subsprite;
            Position = position;
            Direction = direction ?? Vector2.Zero;
            Velocity = Vector2.Zero;
        }

        public virtual void Update(GameTime gameTime)
        {
            // default movement code
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // default draw code
            spriteBatch.Draw(Game1.BLANK, Position, Color.Red); // draws a red dot
        }

        public void ClampToMap(Rectangle mapBounds)
        {
            // checks if the Hitbox Rectangle is fully contained within the mapBounds Rectangle
            if (mapBounds.Contains(Hitbox.BoundingBox))
                return;

            Vector2 offset = Vector2.Zero;

            // I'm not too sure why I need to add the '+1', but if I don't, the Entity will be able to go 1px past the edges in the top-left
            if (mapBounds.Left > Hitbox.BoundingBox.Left)
                offset.X = mapBounds.Left - Hitbox.BoundingBox.Left + 1;
            else if (mapBounds.Right < Hitbox.BoundingBox.Right)
                offset.X = mapBounds.Right - Hitbox.BoundingBox.Right;

            if (mapBounds.Top > Hitbox.BoundingBox.Top)
                offset.Y = mapBounds.Top - Hitbox.BoundingBox.Top + 1;
            else if (mapBounds.Bottom < Hitbox.BoundingBox.Bottom)
                offset.Y = mapBounds.Bottom - Hitbox.BoundingBox.Bottom;

            Position += offset;
        }

        public virtual bool CollidesWith(Entity other)
        {
            if (Hitbox.BoundingBox.Intersects(other.Hitbox.BoundingBox))
                return Hitbox.Intersects(other.Hitbox);
            return false;
        }
    }
}
