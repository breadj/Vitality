using static Capstone_Project.Globals.Utility;
using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects.Hitboxes
{
    public interface IHitbox
    {
        Entity entity { get; init; }
        public Vector2 Centre { get { return entity.Position; } }
        public Vector2 Size { get; init; }
        public Rectangle BoundingBox { get { return new Rectangle(VtoP(Centre - (Size / 2f)), VtoP(Size)); } }
        public bool Intersects(IHitbox other);
    }
}
