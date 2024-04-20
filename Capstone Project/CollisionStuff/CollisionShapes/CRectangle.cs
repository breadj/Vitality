using Capstone_Project.Fundamentals.DrawableShapes;
using Microsoft.Xna.Framework;

namespace Capstone_Project.CollisionStuff.CollisionShapes
{
    public class CRectangle : CShape
    {
        public (int Width, int Height) Bounds { get; private set; }

        public CRectangle(Vector2 centre, (int width, int height) bounds, bool dynamic = true) : base(centre)
        {
            Dynamic = dynamic;
            Bounds = bounds;
            BoundingBox = GenerateBoundingBox();
        }

        public CRectangle(DRectangle rect, bool dynamic = true) : base(rect.Centre)
        {
            Dynamic = dynamic;
            Bounds = (rect.BoundingBox.Width, rect.BoundingBox.Height);
            BoundingBox = rect.BoundingBox;
        }

        protected override Rectangle GenerateBoundingBox()
        {
            return new Rectangle((int)(Centre.X - (Bounds.Width / 2f)), (int)(Centre.Y - (Bounds.Height / 2f)), Bounds.Width, Bounds.Height);
        }
    }
}
