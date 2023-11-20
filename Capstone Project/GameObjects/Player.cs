using Capstone_Project.GameObjects.Hitboxes;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Capstone_Project.GameObjects
{
    public class Player : Entity
    {
        private float speed = 250;      // in px/s
        private new int size = 100;     // in px

        public Player(Subsprite subsprite, Vector2 position, Vector2? direction = null) : base(subsprite, position, direction)
        {
            Hitbox = new RectangleHitbox(this, new(size));
        }

        public override void Update(GameTime gameTime)
        {
            Direction = Movement(Game1.Controls.ActivatedActions);
            Velocity = Direction * speed;

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(subsprite.SpriteSheet, Hitbox.BoundingBox,
                subsprite.Source, Color.White, 0f, subsprite.Origin, SpriteEffects.None, 0.1f);

            //spriteBatch.Draw(Globals.Globals.BLANK, Hitbox.BoundingBox, null, new Color(Color.Pink, 0.5f), 0f, Vector2.One / 2f, SpriteEffects.None, 0.05f);
        }


        private Vector2 Movement(List<Input.Action> relevantActions)
        {
            Vector2 tempDirection = Vector2.Zero;
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

            if (tempDirection == Vector2.Zero)
                return Vector2.Zero;
            return Vector2.Normalize(tempDirection);
        }
    }
}
