using Microsoft.Xna.Framework;

namespace Capstone_Project.Collision.CollisionShapes
{
    public abstract class CShape
    {
        public abstract bool Dynamic { get; init; }
        public Rectangle BoundingBox { get; protected set; }
        public Vector2 Centre { get; protected set; }

        public CShape(Vector2 centre)
        {
            Centre = centre;
        }
    }
}
