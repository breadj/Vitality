using Capstone_Project.Globals;
using static Capstone_Project.Globals.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Capstone_Project.GameObjects.Hitboxes;

namespace Capstone_Project.GameObjects
{
    public class Player : Entity
    {
        private float speed = 200;      // in px/s
        private int size = 100;         // in px

        public Player(Subsprite subsprite, Vector2 position, Vector2? direction = null) : base(subsprite, position, direction)
        {
            Hitbox = new CircleHitbox(position, new(size));
        }

        public override void Update(GameTime gameTime)
        {
            Velocity = Movement(Game1.Controls.ActivatedActions);

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(subsprite.Source.Width / 2f, subsprite.Source.Height / 2f);
            spriteBatch.Draw(subsprite.SpriteSheet, new Rectangle(VtoP(Position), new (size)),
                subsprite.Source, Color.White, 0f, origin, SpriteEffects.None, 0.1f);
        }


        private Vector2 Movement(List<Input.Action> relevantActions)
        {
            Vector2 tempDirection = new Vector2(0);
            foreach (var action in relevantActions)
            {
                switch (action.Name)
                {
                    case "Up": 
                        tempDirection.Y -= 1;
                        break;
                    case "Down":
                        tempDirection.Y += 1;
                        break;
                    case "Left":
                        tempDirection.X -= 1;
                        break;
                    case "Right":
                        tempDirection.X += 1;
                        break;
                }
            }
            Direction = tempDirection;

            if (Direction == Vector2.Zero)
                return Vector2.Zero;
            return Vector2.Normalize(Direction) * speed;
        }
    }
}
