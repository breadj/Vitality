using Capstone_Project.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Capstone_Project.GameObjects
{
    public abstract class Entity
    {
        protected Subsprite subsprite;

        protected Vector2 position;
        protected Vector2 direction;
        protected Vector2 velocity;

        public Entity(Subsprite subsprite, Vector2 position, Vector2? direction = null) 
        {
            this.subsprite = subsprite;
            this.position = position;
            this.direction = direction ?? Vector2.Zero;
        }

        public virtual void Update(GameTime gameTime)
        {
            // default movement code
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // default draw code
            spriteBatch.Draw(Game1.BLANK, position, Color.Red); // draws a red dot
        }
    }
}
