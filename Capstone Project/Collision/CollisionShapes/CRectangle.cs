using Microsoft.Xna.Framework;

namespace Capstone_Project.Collision.CollisionShapes
{
    public class CRectangle : CShape
    {
        public override bool Dynamic { get; init; }
        public new Rectangle BoundingBox => new Rectangle((int)(Centre.X - 0.5f * Bounds.Width), (int)(Centre.Y - (Bounds.Height / 2f)), Bounds.Height, Bounds.Width);
        public (int Width, int Height) Bounds { get; private set; }

        public CRectangle(Vector2 centre, (int width, int height) bounds, bool dynamic = true) : base(centre)
        {
            Dynamic = dynamic;
            Bounds = bounds;
        }
    }
}
