using Microsoft.Xna.Framework;

namespace Capstone_Project.CollisionStuff.CollisionShapes
{
    public class CCircle : CShape
    {
        public override bool Dynamic { get; init; }
        public new Rectangle BoundingBox => new Rectangle((int)(Centre.X - Radius), (int)(Centre.Y - Radius), (int)(2 * Radius), (int)(2 * Radius));
        public float Radius { get; private set; }

        public CCircle(Vector2 centre, float radius, bool dynamic = true) : base(centre)
        {
            Dynamic = dynamic;
            Radius = radius;
        }
    }
}
