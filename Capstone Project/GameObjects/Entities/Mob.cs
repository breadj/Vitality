using static Capstone_Project.Globals.Globals;
using static Capstone_Project.Globals.Utility;
using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Capstone_Project.CollisionStuff;
using Capstone_Project.Fundamentals;

namespace Capstone_Project.GameObjects.Entities
{
    public class Mob : Entity, IRespondable
    {
        #region Default Attributes
        public static readonly new float DefaultSpeed = 100f;
        public static readonly new int DefaultSize = 70;

        public static readonly Comparer<(ICollidable, CollisionDetails)> DefaultCollisionsComparer = Comparer<(ICollidable, CollisionDetails d)>
            .Create((a, b) => b.d.IntersectionArea.CompareTo(a.d.IntersectionArea));

        public static readonly Vector2 DefaultOrientation = new Vector2(0, 1);      // down
        #endregion Default Attributes


        public Rectangle OldBoundingBox => new Rectangle((int)(Position.X - Size / 2f), (int)(Position.Y - Size / 2f), Size, Size);
        public Vector2 TargetPos { get; set; }
        public Rectangle PathCollider => Collision.GeneratePathCollider(OldBoundingBox, Collider.BoundingBox);
        public SortedLinkedList<(ICollidable Other, CollisionDetails Details)> Collisions { get; init; }

        public Vector2 Orientation { get; protected set; }
        protected Vector2 actualVelocity { get; set; } = Vector2.Zero;      // how far is actually travelled in a frame (Velocity * seconds elapsed)

        public Mob(bool? visible = null, string spriteName = null, Color? colour = null, float? rotation = null, float? layer = null, 
            bool? active = null, Vector2? position = null, Vector2? direction = null, Vector2? velocity = null, float? speed = null,
            int? size = null, bool? dead = null,
            Comparer<(ICollidable, CollisionDetails)> collisionsComparer = null, Vector2? orientation = null)
            : base(visible, spriteName, colour, rotation, layer, active, position, direction, velocity, speed ?? DefaultSpeed, size ?? DefaultSize, 
                  dead)
        {
            Collisions = new SortedLinkedList<(ICollidable Other, CollisionDetails Details)>(collisionsComparer ?? DefaultCollisionsComparer);

            Orientation = orientation ?? DefaultOrientation;
        }

        /*public Mob(string spriteName, Subsprite subsprite, Vector2 position, int size = 0, float speed = 1) 
            : base(spriteName, subsprite, position, size, speed)
        {
            Collisions = new SortedLinkedList<(ICollidable Other, CollisionDetails Details)>(Comparer<(ICollidable, CollisionDetails d)>
                .Create((a, b) => b.d.IntersectionArea.CompareTo(a.d.IntersectionArea)));       // b.CompareTo(a) = descending order

            Orientation = new Vector2(0, 1); // down
            actualVelocity = Vector2.Zero;
        }*/

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

            Position = TargetPos;
            Collider.MoveTo(Position);
        }

        protected virtual void RecalculateCollisions()
        {
            List<(ICollidable Other, CollisionDetails Details)> newCollisions = new List<(ICollidable Other, CollisionDetails Details)>(Collisions);
            Collisions.Clear();

            foreach (var collision in newCollisions)
            {
                if (CollidesWith(collision.Other, out CollisionDetails cd))
                    Collisions.Add((collision.Other, cd));
            }
        }

        #endregion
    }
}
