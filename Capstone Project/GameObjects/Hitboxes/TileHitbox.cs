using Capstone_Project.MapStuff;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Project.GameObjects.Hitboxes
{
    public class TileHitbox : IHitbox
    {
        private Tile tile { get; init; }
        public Vector2 Centre { get { return tile.Position.ToVector2() + Size / 2f; } }
        public Vector2 Size { get; init; }
        public Point PSize { get; init; }
        public Rectangle BoundingBox { get; init; }

        public TileHitbox(Tile tile, Point size)
        {
            this.tile = tile;

            PSize = size;
            Size = PSize.ToVector2();
            BoundingBox = new Rectangle(tile.Position, size);
        }

        public CollisionDetails Intersects(IHitbox other)
        {
            CollisionDetails collisionDetails = new CollisionDetails();
            collisionDetails.From = this;
            collisionDetails.To = other;

            // if the BoundingBoxes don't intersect, then they can't be colliding
            Rectangle intersection = Rectangle.Intersect(BoundingBox, other.BoundingBox);
            if (intersection.IsEmpty)
                return collisionDetails;

            collisionDetails.Intersection = intersection;

            if (other is CircleHitbox circleHitbox)
            {
                return Intersects(circleHitbox, collisionDetails);
            }
            if (other is RectangleHitbox rectangleHitbox)
            {
                collisionDetails.Type = CollisionType.RectOnRect;
                return collisionDetails;
            }
            // if a TileHitbox collides with another TileHitbox... something has gone VERY wrong
            return collisionDetails;
        }

        private CollisionDetails Intersects(CircleHitbox circle, CollisionDetails collisionDetails)
        {
            Vector2 localCirclePos = new Vector2(MathF.Abs(circle.Centre.X - Centre.X), MathF.Abs(circle.Centre.Y - Centre.Y));

            // testing if any of the corners of the Rectangle are in the Circle
            float intersectionDepth = circle.Radius - (localCirclePos - (Size / 2)).Length();
            if (intersectionDepth > 0)
            {
                collisionDetails.IntersectionDepth = intersectionDepth;
                collisionDetails.Type = CollisionType.CircOnCirc;
                if (localCirclePos.X > Size.X / 2f && localCirclePos.Y > Size.Y / 2f)
                    collisionDetails.CornerCollision = true;
                return collisionDetails;
            }

            // check if the local Circle's cardinal points are within the bounds of the local Rectangle
            if (localCirclePos.X - circle.Radius < PSize.X / 2 && localCirclePos.Y - circle.Radius < PSize.Y / 2)        // prefer to use PSize here just for the minor efficiency bonus
            {
                collisionDetails.Type = CollisionType.CircOnCirc;
                collisionDetails.CornerCollision = false;
                return collisionDetails;
            }
            return collisionDetails;
        }
    }
}
