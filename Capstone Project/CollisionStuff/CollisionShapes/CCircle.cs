using Capstone_Project.Fundamentals.DrawableShapes;
using Microsoft.Xna.Framework;

namespace Capstone_Project.CollisionStuff.CollisionShapes
{
    public class CCircle : CShape
    {
        public float Radius { get; private set; }

        public CCircle(Vector2 centre, float radius, bool dynamic = true) : base(centre)
        {
            Dynamic = dynamic;
            BoundingBox = GenerateBoundingBox();
            Radius = radius;
        }

        public CCircle(DCircle circ, bool dynamic = true) : base(circ.Centre)
        {
            Dynamic = dynamic;
            BoundingBox = circ.BoundingBox;
            Radius = circ.Radius;
        }

        protected override Rectangle GenerateBoundingBox()
        {
            return new Rectangle((int)(Centre.X - Radius), (int)(Centre.Y - Radius), (int)(2 * Radius), (int)(2 * Radius));
        }
    }
}
