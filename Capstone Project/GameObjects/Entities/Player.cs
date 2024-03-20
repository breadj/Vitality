using static Capstone_Project.Globals.Globals;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Capstone_Project.GameObjects.Interfaces;

namespace Capstone_Project.GameObjects.Entities
{
    public class Player : Agent, IHurtable, IAttacker
    {
        public Player(string spriteName, Subsprite subsprite, Vector2 position, int vitality, int damage, float attackRange = 100f, float defence = 0f, int size = 100, float speed = 250)
            : base(spriteName, subsprite, position, vitality, damage, attackRange, defence, size, speed)
        {
            Strike.ChangeCDs(2f, 0.5f, 1f);
        }
        public override void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            Attack();
            Look();
            Move();
        }

        public override void Draw()
        {
            base.Draw();

            Strike.Draw();
            if (Strike.Cooldown.Active)
                spriteBatch.Draw(Pixel, new Rectangle(Collider.BoundingBox.Left, Collider.BoundingBox.Bottom, 
                    (int)(Size * Strike.Cooldown.Percentage), 10), null, new Color(Color.DarkGray, 0.9f), 0f, 
                    Vector2.Zero, SpriteEffects.None, Strike.Layer);

            //spriteBatch.Draw(BLANK, PathCollider, null, new Color(Color.MediumPurple, 0.5f), 0f, Vector2.Zero, SpriteEffects.None, 0.9f);
            //spriteBatch.DrawString(DebugFont, Position.ToString(), Position, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
            //spriteBatch.Draw(BLANK, new Rectangle(Position.ToPoint(), new Point(Size)), null, new Color(Color.Pink, 0.5f), MathHelper.PiOver4, BLANK.Bounds.Size.ToVector2() / 2f, SpriteEffects.None, 0.05f);
            //spriteBatch.Draw(BLANK, Hitbox, null, new Color(Color.Blue, 0.5f), 0f, Vector2.Zero, SpriteEffects.None, 0.04f);
        }

        public override void Move()
        {
            if (Strike.Lock)
                return;

            bool bufActExists = !Game1.Controls.ActionBuffer.IsEmpty;
            bool prevDashActive = Dash.Active;

            if (bufActExists && !PerformingAction && Game1.Controls.ActionBuffer.Peek().Name == "dash")
            {
                Game1.Controls.ActionBuffer.Remove();
                if (Direction == Vector2.Zero)
                    Direction = Orientation;
                Dash.Start();
            }

            if (Dash.Active)
            {
                Dash.Update(gameTime);

                Speed = Dash.Speed;
                Direction = Dash.Direction;
            }

            if (!Dash.Active)
            {
                if (prevDashActive)     // if this is the first frame since ending a dash, revert speed
                    Speed = BaseSpeed;

                Direction = Movement(Game1.Controls.ActivatedActions);
            }

            base.Move();
        }

        public override void Look()
        {
            if (!(Strike.Lock || Dash.Active))
                Orientation = LookAt(Game1.Camera.ScreenToWorld(Game1.Controls.MousePos.ToVector2()));

            base.Look();
        }

        public override void Attack()
        {
            bool bufActExists = !Game1.Controls.ActionBuffer.IsEmpty;

            if (Strike.Active || Strike.OnCD)
                Strike.Update(gameTime);
            else if (bufActExists && !PerformingAction && Game1.Controls.ActionBuffer.Peek().Name == "attack")
            {
                Game1.Controls.ActionBuffer.Remove();
                Swing();
            }
        }

        private Vector2 Movement(List<Input.Action> relevantActions)
        {
            Vector2 tempDirection = Vector2.Zero;
            foreach (var action in relevantActions)
            {
                switch (action.Name)
                {
                    case "up":
                        tempDirection.Y -= 1;
                        break;
                    case "down":
                        tempDirection.Y += 1;
                        break;
                    case "left":
                        tempDirection.X -= 1;
                        break;
                    case "right":
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
