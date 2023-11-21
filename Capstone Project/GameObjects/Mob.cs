using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Capstone_Project.GameObjects
{
    public class Mob : Entity, IRespondable
    {
        public List<ICollidable> CollidesWith => throw new NotImplementedException();

        private Vector2 lastPosition { get; set; }
        private Vector2 actualVelocity { get; set; }

        public Mob(Subsprite subsprite, Vector2 position) : base(subsprite, position)
        {
            lastPosition = position;
            actualVelocity = Vector2.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            // if there's been any movement since last frame
            if (lastPosition != Position)
            {
                // sets lastPosition to Position before Position is changed in base.Update()
                lastPosition = Position;

                base.Update(gameTime);
            }
        }

        public override void Draw()
        {
            base.Draw();
        }

        public Rectangle ProjectHitbox()      // predictedOffset should be Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds
        {
            Rectangle projected = new Rectangle(Hitbox.X + (int)actualVelocity.X, Hitbox.Y + (int)actualVelocity.Y, Size, Size);
            return projected;
        }

        // to be called once the Hitbox projection and collision has all been handled
        public void FinallyMove(Vector2 newPos)
        {
            Position = newPos;
        }
    }
}
