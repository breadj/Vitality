using static Capstone_Project.Globals.Globals;
using static Capstone_Project.Globals.Utility;
using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Capstone_Project.CollisionStuff;

namespace Capstone_Project.GameObjects.Entities
{
    public class Mob : Entity, IRespondable
    {
        public Rectangle OldBoundingBox => new Rectangle((int)(Position.X - Size / 2f), (int)(Position.Y - Size / 2f), Size, Size);
        public Vector2 TargetPos { get; set; }
        public Rectangle PathCollider => Collision.GeneratePathCollider(OldBoundingBox, Collider.BoundingBox);
        public LinkedList<(ICollidable Other, CollisionDetails Details)> Collisions { get; protected set; }

        public Vector2 Orientation { get; protected set; }
        protected Vector2 actualVelocity { get; set; }      // how far is actually travelled in a frame (Velocity * seconds elapsed)

        public Mob(Subsprite subsprite, Vector2 position, int size = 0, int speed = 1) : base(subsprite, position, size, speed)
        {
            Collisions = new LinkedList<(ICollidable Other, CollisionDetails Details)>();

            Orientation = new Vector2(0, 1); // down
            actualVelocity = Vector2.Zero;
        }

        // only moves the TargetPos, not actual Position
        // to move the actual Position, use Move() after using this and handling the collisions
        public override void Update(GameTime gameTime)
        {
            if (!Active) 
                return;

            lastPosition = Position;
            TargetPos = Position;

            Velocity = Direction * Speed;
            actualVelocity = Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            TargetPos += actualVelocity;

            Collider.MoveTo(TargetPos);

            Rotation = VectorToAngle(Orientation);
        }

        public override void Draw()
        {
            base.Draw();

            /*spriteBatch.Draw(Pixel, OldBoundingBox, null, new Color(Color.Red, 0.4f), 0f, Vector2.Zero, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0.04f);
            spriteBatch.Draw(Pixel, Collider.BoundingBox, null, new Color(Color.Yellow, 0.4f), 0f, Vector2.Zero, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0.04f);*/
        }


        #region Collision Handling

        public override bool CollidesWith(ICollidable other, out CollisionDetails cd)
        {
            // needs to intersect the PathCollider
            if (!Collision.Rectangular(PathCollider, other.Collider.BoundingBox, out Rectangle intersection))
            {
                cd = new CollisionDetails();
                return false;
            }

            if (base.CollidesWith(other, out cd))
                cd.Intersection = intersection;
            return cd.Collided;
        }

        public virtual void InsertIntoCollisions(ICollidable other, CollisionDetails details)
        {
            // if the list is empty, just add it straight in
            if (!Collisions.Any())
            {
                Collisions.AddFirst((other, details));
                return;
            }

            bool insertion = false;
            // inserts into the list before the first element that has a smaller IntersectionArea than it
            // can't be a foreach loop as you can't convert Collisions.First.Value back into a LinkedListNode
            for (var node = Collisions.First; node != null; node = node.Next)
            {
                if (details.IntersectionArea > node.Value.Details.IntersectionArea)
                {
                    Collisions.AddBefore(node, (other, details));
                    insertion = true;
                    break;
                }
            }
            if (!insertion)
                Collisions.AddLast((other, details));

            // remember to do other.InsertIntoCollisions(details) if other is also an IRespondable
        }

        public virtual void HandleCollisions()
        {
            // while there are still collisions to be handled
            while (Collisions.Any())
            {
                RecalculateCollisions();

                // if the first collision doesn't even BB collide with the PathCollider, then neither does the rest of them
                if (!Collisions.Any() || Collisions.First.Value.Details.IntersectionArea == 0)
                {
                    Collisions.Clear();
                    break;
                }

                Collision.HandleCollision(this, Collisions.First.Value.Other, Collisions.First.Value.Details);
                Collider.MoveTo(TargetPos);
                Collisions.RemoveFirst();
            }
        }

        protected virtual void RecalculateCollisions()
        {
            List<(ICollidable Other, CollisionDetails Details)> newCollisions = new List<(ICollidable Other, CollisionDetails Details)>(Collisions);
            Collisions.Clear();

            foreach (var collision in newCollisions)
            {
                if (CollidesWith(collision.Other, out CollisionDetails cd))
                    InsertIntoCollisions(collision.Other, cd);
            }
        }

        #endregion

        // to be called once all the collision has been handled
        public virtual void Move()
        {
            Position = TargetPos;
            Collider.MoveTo(Position);
        }
    }
}
