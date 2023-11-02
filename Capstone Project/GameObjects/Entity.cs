using Capstone_Project.GameObjects.Hitboxes;
using Capstone_Project.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Capstone_Project.GameObjects
{
    public abstract class Entity
    {
        protected Subsprite subsprite;

        public Vector2 Position { get; protected set; }
        public Vector2 Direction { get; protected set; }
        public Vector2 Velocity { get; protected set; }

        public IHitbox Hitbox { get; protected set; }

        public Entity(Subsprite subsprite, Vector2 position, Vector2? direction = null) 
        {
            this.subsprite = subsprite;
            Position = position;
            Direction = direction ?? Vector2.Zero;
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

        public virtual bool CollidesWith(Entity other)
        {
            if (Hitbox.BoundingBox.Intersects(other.Hitbox.BoundingBox))
                return Hitbox.Intersects(other.Hitbox);
            return false;
        }
    }
}
