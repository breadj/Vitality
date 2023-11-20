using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects.Hitboxes
{
    public class CircleHitbox : IHitbox
    {
        private Entity entity { get; init; }
        public Vector2 Centre { get { return entity.Position; } }
        public Vector2 Size { get; init; }
        public Rectangle BoundingBox { get { return new Rectangle((Centre - (Size / 2f)).ToPoint(), Size.ToPoint()); } }
        public float Radius { get; init; }

        public CircleHitbox(Entity entity, Vector2 size)
        {
            this.entity = entity;
            Size = size;
            Radius = size.X / 2f;
        }

        public CollisionDetails Intersects(IHitbox other)
        {
            CollisionDetails collisionDetails = new CollisionDetails();
            collisionDetails.From = other;
            collisionDetails.To = this;

            // if the BoundingBoxes don't intersect, then they can't be colliding
            Rectangle intersection = Rectangle.Intersect(BoundingBox, other.BoundingBox);
            if (intersection.IsEmpty)
                return collisionDetails;

            collisionDetails.Intersection = intersection;

            if (other is CircleHitbox circleHitbox)
            {
                if ((circleHitbox.Centre - Centre).Length() < circleHitbox.Radius + Radius)
                {
                    collisionDetails.Type = CollisionType.CircOnCirc;
                    return collisionDetails;
                }
            }
            if (other is RectangleHitbox rectangleHitbox)
            {
                return rectangleHitbox.Intersects(this);    // easier to let the RectangleHitbox.Intersects(CircleHitbox) method fire instead of recreating the method here too
            }
            if (other is TileHitbox tileHitbox)
            {
                return tileHitbox.Intersects(this);         // same deal here
            }
            return collisionDetails;
        }
    }
}
