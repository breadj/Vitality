using static Capstone_Project.Globals.Globals;
using static Capstone_Project.Globals.Utility;
using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Capstone_Project.GameObjects
{
    public class Mob : Entity, IRespondable
    {
        public Vector2 TargetPos { get; set; }
        public Rectangle TargetHitbox => new Rectangle((int)(TargetPos.X - (Size / 2f)), (int)(TargetPos.Y - (Size / 2f)), Size, Size);
        public Rectangle PathCollider => generatePathCollider();
        public LinkedList<CollisionDetails> Collisions { get; protected set; }

        protected Vector2 actualVelocity { get; set; }      // how far is actually travelled in a frame (Velocity * seconds elapsed)

        public Mob(Subsprite subsprite, Vector2 position, int size = 0, int speed = 1) : base(subsprite, position, size, speed)
        {
            Collisions = new LinkedList<CollisionDetails>();      // reverses Comparer so the largest area is always first
            actualVelocity = Vector2.Zero;
        }

        // only moves the TargetPos, not actual Position
        // to move the actual Position, use Move() after using this and handling the collisions
        public override void Update(GameTime gameTime)
        {
            lastPosition = Position;
            TargetPos = Position;

            Velocity = Direction * Speed;
            actualVelocity = Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            TargetPos += actualVelocity;
        }

        public override void Draw()
        {
            base.Draw();

            spriteBatch.DrawString(DebugFont, whichHandler, Position, Color.White, 0f, Vector2.Zero, 1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0.9f);
        }

        #region Overridden Entity Collision Checking
        public override CollisionDetails CollidesWith(ICollidable other)
        {
            CollisionDetails details = new CollisionDetails();
            details.From = this;
            details.Against = other;
            details.Intersection = Rectangle.Intersect(TargetHitbox, other.Hitbox);

            // if there is no intersection between the Hitboxes
            if (details.Intersection.IsEmpty)
                return details;
            // continues if there is a tangible intersection

            // if both are square
            if (!IsCircle && !other.IsCircle)
            {
                details.Type = CollisionType.RectOnRect;
                return details;
            }

            // knowing that at least one is a circle:
            if (!IsCircle)                              // if this is the square
                return CircOnRect(other, this, details);
            if (!other.IsCircle)                        // if other is the square
                return CircOnRect(this, other, details);
            return CircOnCirc(this, other, details);    // finally, knowing that neither are squares (aka: both are circles)
        }

        protected static CollisionDetails CircOnCirc(IRespondable a, ICollidable b, CollisionDetails details)
        {
            // pretty much impossible for two circles to collide only cardinally, so may as well deal with it as a corner collision
            details.CornerCollision = true;

            float intersectionDepth = a.Radius + b.Radius - (a.TargetHitbox.Center - b.Hitbox.Center).ToVector2().Length();
            if (intersectionDepth > 0)
            {
                details.IntersectionDepth = intersectionDepth;
                details.Type = CollisionType.CircOnCirc;
            }

            return details;
        }

        protected static CollisionDetails CircOnCirc(ICollidable a, IRespondable b, CollisionDetails details)
        {
            details.CornerCollision = true;

            float intersectionDepth = a.Radius + b.Radius - (a.Hitbox.Center - b.TargetHitbox.Center).ToVector2().Length();
            if (intersectionDepth > 0)
            {
                details.IntersectionDepth = intersectionDepth;
                details.Type = CollisionType.CircOnCirc;
            }

            return details;
        }

        // checks if the square and circle collide by creating localised versions of each around the square's center being at the origin
        // note: localised square has side lengths (square.Radius, square.Radius) and top-left (0, 0)
        protected static CollisionDetails CircOnRect(IRespondable circle, ICollidable square, CollisionDetails details)
        {
            details.From = circle;
            details.Against = square;

            Vector2 localCirclePos = new Vector2(MathF.Abs(circle.TargetHitbox.Center.X - square.Hitbox.Center.X), MathF.Abs(circle.TargetHitbox.Center.Y - square.Hitbox.Center.Y));

            // testing to see if any of the corners of the square are in the circle
            float intersectionDepth = circle.Radius - (localCirclePos - new Vector2(square.Radius)).Length();
            if (intersectionDepth > 0)
            {
                details.IntersectionDepth = intersectionDepth;
                details.Type = CollisionType.CircOnRect;

                // knowing there's already intersection, checks if the intersection is on the corner or just via a cardinal point
                if (localCirclePos.X > square.Radius && localCirclePos.Y > square.Radius)
                    details.CornerCollision = true;

                return details;
            }

            // check if the local circle's cardinal points are within the bounds of the local square
            if (localCirclePos.X - circle.Radius < square.Radius && localCirclePos.Y - circle.Radius < square.Radius)
            {
                details.Type = CollisionType.CircOnRect;
                details.CornerCollision = false;    // not needed because it's false by default, but it doesn't hurt to show it explicitly

                return details;
            }

            return details;
        }

        protected static CollisionDetails CircOnRect(ICollidable circle, IRespondable square, CollisionDetails details)
        {
            details.From = circle;
            details.Against = square;

            Vector2 localCirclePos = new Vector2(MathF.Abs(circle.Hitbox.Center.X - square.TargetHitbox.Center.X), MathF.Abs(circle.Hitbox.Center.Y - square.TargetHitbox.Center.Y));

            // testing to see if any of the corners of the square are in the circle
            float intersectionDepth = circle.Radius - (localCirclePos - new Vector2(square.Radius)).Length();
            if (intersectionDepth > 0)
            {
                details.IntersectionDepth = intersectionDepth;
                details.Type = CollisionType.CircOnRect;

                // knowing there's already intersection, checks if the intersection is on the corner or just via a cardinal point
                if (localCirclePos.X > square.Radius && localCirclePos.Y > square.Radius)
                    details.CornerCollision = true;

                return details;
            }

            // check if the local circle's cardinal points are within the bounds of the local square
            if (localCirclePos.X - circle.Radius < square.Radius && localCirclePos.Y - circle.Radius < square.Radius)
            {
                details.Type = CollisionType.CircOnRect;
                details.CornerCollision = false;    // not needed because it's false by default, but it doesn't hurt to show it explicitly

                return details;
            }

            return details;
        }
        #endregion

        #region Collision Handling
        private Rectangle generatePathCollider()
        {
            // gets the smallest x and y, then the largest width and height to encapsulate both Rectangles
            int x = MathHelper.Min(Hitbox.Left, TargetHitbox.Left);
            int y = MathHelper.Min(Hitbox.Top, TargetHitbox.Top);
            int width = MathHelper.Max(Hitbox.Right, TargetHitbox.Right) - x;
            int height = MathHelper.Max(Hitbox.Bottom, TargetHitbox.Bottom) - y;

            return new Rectangle(x, y, width, height);
        }

        public virtual void InsertIntoCollisions(CollisionDetails details)
        {
            // if the list is empty, just add it straight in
            if (!Collisions.Any())
            {
                Collisions.AddFirst(details);
                return;
            }

            bool insertion = false;

            // inserts into the list before the first element that has a smaller IntersectionArea than it
            for (var node = Collisions.First; node != null; node = node.Next)
            {
                if (details.IntersectionArea > node.Value.IntersectionArea)
                {
                    Collisions.AddBefore(node, details);
                    insertion = true;
                    break;
                }
            }
            if (!insertion)
                Collisions.AddLast(details);

            // remember to do other.InsertIntoCollisions(details) if other is also an IRespondable
        }

        public virtual void HandleCollisions()
        {
            // while there are still collisions to be handled
            while (Collisions.Any())
            {
                RecalculateCollisions();

                bool anyChange = HandleCollision(Collisions.First.Value);
                if (anyChange)
                    Collisions.RemoveFirst();
                else
                    Collisions.Clear();
            }
        }

        private static bool FindRespondable(CollisionDetails details, out IRespondable responder, out ICollidable collider)
        {
            if (details.From is IRespondable)
            {
                responder = details.From as IRespondable;
                collider = details.Against;
            }
            else
            {
                responder = details.Against as IRespondable;
                collider = details.From;
            }

            return !(responder == null || collider == null);
        }

        protected virtual void RecalculateCollisions()
        {
            List<CollisionDetails> newCollisions = new List<CollisionDetails>(Collisions);
            Collisions.Clear();

            foreach (var oldDetails in newCollisions)
            {
                CollisionDetails newDetails = CollidesWith(oldDetails.From == this ? oldDetails.Against : oldDetails.From);
                InsertIntoCollisions(newDetails);
            }
        }

        protected static bool HandleCollision(CollisionDetails details)
        {
            return details.Type switch
            {
                CollisionType.None => false,                            // if there's no collision to be had, return that there's been no change
                CollisionType.RectOnRect => HandleRectOnRect(details),
                CollisionType.CircOnRect => HandleCircOnRect(details),
                CollisionType.CircOnCirc => HandleCircOnCirc(details),
                _ => true                                               // just to make sure the default is checked
            };
        }

        private static string whichHandler = "";
        protected static bool HandleRectOnRect(CollisionDetails details)
        {
            whichHandler = "RectOnRect";
            IRespondable responder;
            ICollidable collider;

            if (!FindRespondable(details, out responder, out collider))
                return false;

            // Hitbox.Center should be used instead of Position, as it is unknown where the Position is on the ICollidable (for a Tile: top-left, for a Mob: centre)
            Point displacement = responder.TargetHitbox.Center - collider.Hitbox.Center;

            int absDiffX = Math.Abs(displacement.X);
            int absDiffY = Math.Abs(displacement.Y);

            // if the absolute different has X > Y then that means it's an East-West-wise collision (on x-axis), and vice versa
            float newX = absDiffX < absDiffY ? responder.TargetPos.X : responder.TargetPos.X + (Sign(displacement.X) * details.Intersection.Width);
            float newY = absDiffY < absDiffX ? responder.TargetPos.Y : responder.TargetPos.Y + (Sign(displacement.Y) * details.Intersection.Height);

            Vector2 newTargetPos = new Vector2(newX, newY);
            if (responder.TargetPos == newTargetPos)
                return false;

            responder.TargetPos = newTargetPos;
            return true;
        }

        protected static bool HandleCircOnRect(CollisionDetails details)
        {
            whichHandler = "CircOnRect";
            // if it's not corner collision, and just cardinal collision, then it's the same as RectOnRect collision
            if (!details.CornerCollision)
                return HandleRectOnRect(details);

            IRespondable responder;
            ICollidable collider;

            if (!FindRespondable(details, out responder, out collider))
                return false;

            Point displacement = responder.TargetHitbox.Center - collider.Hitbox.Center;

            // push initialised as bottom-right-facing (+x, +y) version of Direction, then rotates it by the sign of the displacement.
            // Since it's already normalised (Direction should always be normalised), multiplying by IntersectionDepth will push the
            // IRespondable away from the ICollidable enough to escape collision
            Vector2 pushAway = new Vector2(Math.Abs(responder.Direction.X) * Sign(displacement.X), Math.Abs(responder.Direction.Y) * Sign(displacement.Y)) * details.IntersectionDepth;

            Vector2 newTargetPos = responder.TargetPos + pushAway;
            if (responder.TargetPos == newTargetPos)
                return false;

            responder.TargetPos = newTargetPos;
            return true;
        }

        protected static bool HandleCircOnCirc(CollisionDetails details)
        {
            IRespondable responder;
            ICollidable collider;

            if (!FindRespondable(details, out responder, out collider))
                return false;

            // displacement is the direction from the collider to the responder with the length of the depth of the intersection
            Vector2 pushAway = Vector2.Normalize((responder.TargetHitbox.Center - collider.Hitbox.Center).ToVector2()) * details.IntersectionDepth;

            Vector2 newTargetPos = responder.TargetPos + pushAway;
            if (responder.TargetPos == newTargetPos)
                return false;

            responder.TargetPos = newTargetPos;
            return true;
        }
        #endregion

        // to be called once all the collision has been handled
        public virtual void Move()
        {
            Position = TargetPos;
        }
    }
}
