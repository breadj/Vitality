using static Capstone_Project.Globals.Utility;
using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects.Hitboxes
{
    public class CircleHitbox : IHitbox
    {
        public Entity entity { get; init; }
        public Vector2 Centre { get { return entity.Position; } }
        public Vector2 Size { get; init; }
        public Rectangle BoundingBox { get { return new Rectangle(VtoP(Centre - (Size / 2f)), VtoP(Size)); } }
        public float Radius { get; init; }

        public CircleHitbox(Entity entity, Vector2 size)
        {
            this.entity = entity;
            Size = size;
            Radius = size.X / 2f;
        }

        public bool Intersects(IHitbox other)
        {
            if (other is CircleHitbox circleHitbox)
                return (circleHitbox.Centre - Centre).Length() < circleHitbox.Radius + Radius;
            if (other is RectangleHitbox rectangleHitbox)
                return rectangleHitbox.Intersects(this);    // easier to let the RectangleHitbox.Intersects(CircleHitbox) method fire instead of recreating the method here too
            return false;
        }
    }
}
