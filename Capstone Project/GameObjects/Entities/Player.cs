﻿using static Capstone_Project.Globals.Globals;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Capstone_Project.GameObjects.Interfaces;

namespace Capstone_Project.GameObjects.Entities
{
    public class Player : Mob, IHurtable, IAttacker
    {
        public int Vitality { get; private set; }
        public float Defence { get; private set; }

        public float Damage { get; private set; }
        public Attack Attack { get; private set; }

        public Player(Subsprite subsprite, Vector2 position, int size = 100, int speed = 250) : base(subsprite, position, size, speed)
        {

        }

        public override void Update(GameTime gameTime)
        {
            Direction = Movement(Game1.Controls.ActivatedActions);
            Orientation = LookAt(Game1.Camera.ScreenToWorld(Game1.Controls.MousePos.ToVector2()));

            base.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();

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
