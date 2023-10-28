using Capstone_Project.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Capstone_Project.GameObjects
{
    public class Player : Entity
    {
        private float speed = 128;      // in px/s
        private int size = 100;         // in px

        public Player(Subsprite subsprite, Vector2 position, Vector2? direction = null) : base(subsprite, position, direction)
        {

        }

        public override void Update(GameTime gameTime)
        {
            velocity = Movement(Game1.Controls.ActivatedActions);

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(subsprite.Source.Width / 2, subsprite.Source.Height / 2);
            spriteBatch.Draw(subsprite.SpriteSheet, new Rectangle((int)position.X, (int)position.Y, size, size), 
                subsprite.Source, Color.White, 0f, origin, SpriteEffects.None, 0.1f);
        }


        private Vector2 Movement(List<Input.Action> relevantActions)
        {
            direction = new Vector2(0);
            foreach (var action in relevantActions)
            {
                switch (action.Name)
                {
                    case "Up": 
                        direction.Y -= 1;
                        break;
                    case "Down":
                        direction.Y += 1;
                        break;
                    case "Left":
                        direction.X -= 1;
                        break;
                    case "Right":
                        direction.X += 1;
                        break;
                }
            }

            if (direction == Vector2.Zero)
                return Vector2.Zero;
            return Vector2.Normalize(direction) * speed;
        }
    }
}
