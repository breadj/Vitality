using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects.Hitboxes
{
    public class CircleHitbox : IHitbox
    {
        public Vector2 Centre { get; init; }
        public Vector2 Size { get; init; }
        public float Radius { get; init; }

        public CircleHitbox(Vector2 centre, Vector2 size)
        {
            Centre = centre;
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
