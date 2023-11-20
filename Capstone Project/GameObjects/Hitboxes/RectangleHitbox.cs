using Microsoft.Xna.Framework;
using System;

namespace Capstone_Project.GameObjects.Hitboxes
{
    public class RectangleHitbox : IHitbox
    {
        private Entity entity { get; init; }
        public Vector2 Centre { get { return entity.Position; } }
        public Vector2 Size { get; init; }
        public Rectangle BoundingBox { get { return new Rectangle((Centre - (Size / 2f)).ToPoint(), Size.ToPoint()); } }

        public RectangleHitbox(Entity entity, Vector2 size)
        {
            this.entity = entity;
            Size = size;
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
            if (other is TileHitbox tileHitbox)
            {
                collisionDetails.Type = CollisionType.RectOnRect;
                return collisionDetails;

            }
            return collisionDetails;
        }

        private CollisionDetails Intersects(CircleHitbox circle, CollisionDetails collisionDetails)
        {
            /*/// please see Explanations > RectangleHitbox.Intersects(CircleHitbox) to really understand wtf is happening here

            // if the centre of the circle is inside the BoundingBox of the rectangle
            if (BoundingBox.Contains(circle.Centre))
                return true;

            // creates temporary translations of the circle centre and rectangle, with the centre of the original rectangle at (0,0)
            Vector2 localCircleCentre = new Vector2(Math.Abs(circle.Centre.X - Centre.X), Math.Abs(circle.Centre.Y - Centre.Y));
            Rectangle localRectangle = new Rectangle(new(0), VtoP(Size / 2 + new Vector2(circle.Radius)));

            if (!localRectangle.Contains(localCircleCentre))
                return false;

            Rectangle smallerlocalRectangle = new Rectangle(VtoP(Size / 2), new((int)circle.Radius));
            if (!smallerlocalRectangle.Contains(localCircleCentre))
                return true;

            return (localCircleCentre - PtoV(smallerlocalRectangle.Location)).Length() < circle.Radius;*/

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
            if (localCirclePos.X - circle.Radius < Size.X / 2 && localCirclePos.Y - circle.Radius < Size.Y / 2)
            {
                collisionDetails.Type = CollisionType.CircOnCirc;
                collisionDetails.CornerCollision = false;
                return collisionDetails;
            }
            return collisionDetails;
        }
    }
}
