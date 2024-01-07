using static Capstone_Project.Globals.Globals;
using Microsoft.Xna.Framework;
using Capstone_Project.CollisionStuff.CollisionShapes;
using Capstone_Project.Globals;

namespace Capstone_Project.Fundamentals.DrawableShapes
{
    public class DRectangle : DShape
    {
        public DRectangle(Vector2 centre, Rectangle boundingBox) : base(centre)
        {
            BoundingBox = boundingBox;

            Outline = GraphicalMethods.GenerateOutline(Utility.GenerateVertices(boundingBox));
            GenerateFillMode(Scanned);
        }

        public DRectangle(Vector2 centre, Rectangle boundingBox, Color colour, float layer = 0.005f, bool scanned = true)
            : base(centre, colour, layer, scanned)
        {
            BoundingBox = boundingBox;

            Outline = GraphicalMethods.GenerateOutline(Utility.GenerateVertices(boundingBox));
            GenerateFillMode(scanned);
        }

        public DRectangle(CRectangle rect, bool scanned = true) : base(rect.Centre)
        {
            BoundingBox = rect.BoundingBox;

            Outline = GraphicalMethods.GenerateOutline(Utility.GenerateVertices(BoundingBox));
            Scanned = scanned;
            GenerateFillMode(scanned);
        }

        public DRectangle(CRectangle rect, Color colour, float layer = 0.005f, bool scanned = true) : base(rect.Centre, colour, layer, scanned)
        {
            BoundingBox = rect.BoundingBox;

            Outline = GraphicalMethods.GenerateOutline(Utility.GenerateVertices(BoundingBox));
            GenerateFillMode(scanned);
        }

        protected override void GenerateFillMode(bool scanned)
        {
            if (scanned)
                ScanLines = GraphicalMethods.GenerateLineFill(BoundingBox);
            else
                Pixels = GraphicalMethods.GeneratePixelFill(BoundingBox);
        }
    }
}
