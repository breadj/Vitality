using static Capstone_Project.Globals.Globals;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Capstone_Project.GameObjects.Interfaces;
<<<<<<< HEAD
using System.Linq;
using System.Diagnostics;
=======
>>>>>>> bc39f8d78e4142e23321cca44295f357bb9c4054

namespace Capstone_Project.GameObjects.Entities
{
    public class Player : Mob, IHurtable, IAttacker
    {
        public int Vitality { get; private set; }
        public float Defence { get; private set; }

<<<<<<< HEAD
        public float Damage { get; private set; } = 10f;
        public Attack Attack { get; private set; }
        public float Range { get; private set; } = 85f;

        public Player(Subsprite subsprite, Vector2 position, int size = 100, int speed = 250) : base(subsprite, position, size, speed)
        {
            Attack = new Attack(this, Vector2.Zero, 2.5f, 0.5f, 1f);
=======
        public float Damage { get; private set; }
        public Attack Attack { get; private set; }

        public Player(Subsprite subsprite, Vector2 position, int size = 100, int speed = 250) : base(subsprite, position, size, speed)
        {

>>>>>>> bc39f8d78e4142e23321cca44295f357bb9c4054
        }

        public override void Update(GameTime gameTime)
        {
<<<<<<< HEAD
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
=======
            Direction = Movement(Game1.Controls.ActivatedActions);
            Orientation = LookAt(Game1.Camera.ScreenToWorld(Game1.Controls.MousePos.ToVector2()));

            base.Update(gameTime);
>>>>>>> bc39f8d78e4142e23321cca44295f357bb9c4054
        }

        public override void Draw()
        {
            base.Draw();

<<<<<<< HEAD
            Attack.Draw();
            if (Attack.Cooldown.Active)
                spriteBatch.Draw(Attack.Subsprite.SpriteSheet, new Rectangle(Hitbox.Left, Hitbox.Bottom, (int)(Size * Attack.Cooldown.Percentage), 10), Attack.Subsprite.Source,
                    new Color(Color.DarkGray, 0.9f), Attack.Rotation, Vector2.Zero, SpriteEffects.None, Attack.Layer);

=======
>>>>>>> bc39f8d78e4142e23321cca44295f357bb9c4054
            //spriteBatch.Draw(BLANK, PathCollider, null, new Color(Color.MediumPurple, 0.5f), 0f, Vector2.Zero, SpriteEffects.None, 0.9f);
            //spriteBatch.DrawString(DebugFont, Position.ToString(), Position, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
            //spriteBatch.Draw(BLANK, new Rectangle(Position.ToPoint(), new Point(Size)), null, new Color(Color.Pink, 0.5f), MathHelper.PiOver4, BLANK.Bounds.Size.ToVector2() / 2f, SpriteEffects.None, 0.05f);
            //spriteBatch.Draw(BLANK, Hitbox, null, new Color(Color.Blue, 0.5f), 0f, Vector2.Zero, SpriteEffects.None, 0.04f);
        }

        public void TakeDamage(float damage)
        {
            float accDmg = damage;
            // do Defence calculations later

            Vitality -= (int)accDmg;
        }

        public void Swing()
        {
<<<<<<< HEAD
            Attack.Start(Position + (Orientation * Range));
=======

>>>>>>> bc39f8d78e4142e23321cca44295f357bb9c4054
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
