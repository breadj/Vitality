using Capstone_Project.Fundamentals.DrawableShapes;
using Microsoft.Xna.Framework;

namespace Capstone_Project.CollisionStuff.CollisionShapes
{
    public class CRectangle : CShape
    {
        public override bool Dynamic { get; init; }
        public (int Width, int Height) Bounds { get; private set; }

        public CRectangle(Vector2 centre, (int width, int height) bounds, bool dynamic = true) : base(centre)
        {
            Dynamic = dynamic;
            BoundingBox = GenerateBoundingBox();
            Bounds = bounds;
        }

        public CRectangle(DRectangle rect, bool dynamic = true) : base(rect.Centre)
        {
            Dynamic = dynamic;
            BoundingBox = rect.BoundingBox;
            Bounds = (rect.BoundingBox.Size.X, rect.BoundingBox.Size.Y);
        }

        protected override Rectangle GenerateBoundingBox()
        {
            return new Rectangle((int)(Centre.X - 0.5f * Bounds.Width), (int)(Centre.Y - (Bounds.Height / 2f)), Bounds.Height, Bounds.Width);
        }
    }
}
