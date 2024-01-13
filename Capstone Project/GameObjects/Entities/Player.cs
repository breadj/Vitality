using static Capstone_Project.Globals.Globals;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Capstone_Project.GameObjects.Interfaces;
using System.Linq;
using System.Diagnostics;

namespace Capstone_Project.GameObjects.Entities
{
    public class Player : Agent, IHurtable, IAttacker
    {
        public Player(Subsprite subsprite, Vector2 position, int vitality, int damage, float attackRange = 100f, float defence = 0f, int size = 100, int speed = 250)
            : base(subsprite, position, vitality, damage, attackRange, defence, size, speed)
        {
            Attack.ChangeCDs(2f, 0.5f, 1f);
        }

        public override void Update(GameTime gameTime)
        {
            Attack.Update(gameTime);
            if (!Attack.Lock)
            {
                Direction = Movement(Game1.Controls.ActivatedActions);
                Orientation = LookAt(Game1.Camera.ScreenToWorld(Game1.Controls.MousePos.ToVector2()));
            }

            if (Game1.Controls.ActivatedActions.Any(action => action.Name.Equals("Attack", System.StringComparison.OrdinalIgnoreCase)))
                Swing();

            if (!Attack.Lock)
                base.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();

            Attack.Draw();
            if (Attack.Cooldown.Active)
                spriteBatch.Draw(Pixel, new Rectangle(Collider.BoundingBox.Left, Collider.BoundingBox.Bottom, 
                    (int)(Size * Attack.Cooldown.Percentage), 10), null, new Color(Color.DarkGray, 0.9f), 0f, 
                    Vector2.Zero, SpriteEffects.None, Attack.Layer);

            //spriteBatch.Draw(BLANK, PathCollider, null, new Color(Color.MediumPurple, 0.5f), 0f, Vector2.Zero, SpriteEffects.None, 0.9f);
            //spriteBatch.DrawString(DebugFont, Position.ToString(), Position, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
            //spriteBatch.Draw(BLANK, new Rectangle(Position.ToPoint(), new Point(Size)), null, new Color(Color.Pink, 0.5f), MathHelper.PiOver4, BLANK.Bounds.Size.ToVector2() / 2f, SpriteEffects.None, 0.05f);
            //spriteBatch.Draw(BLANK, Hitbox, null, new Color(Color.Blue, 0.5f), 0f, Vector2.Zero, SpriteEffects.None, 0.04f);
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

        // returns normalised Vector2 of the direction from this to the target
        private Vector2 LookAt(Vector2 target)
        {
            return Vector2.Normalize(target - Position);
        }
    }
}
