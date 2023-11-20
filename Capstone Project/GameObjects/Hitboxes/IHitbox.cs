using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects.Hitboxes
{
    public interface IHitbox
    {
        public Vector2 Centre { get; }
        public Vector2 Size { get; init; }
        public Rectangle BoundingBox { get { return new Rectangle((Centre - (Size / 2f)).ToPoint(), Size.ToPoint()); } }
        public CollisionDetails Intersects(IHitbox other);
    }
}
