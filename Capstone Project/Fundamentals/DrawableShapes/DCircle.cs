using Capstone_Project.CollisionStuff.CollisionShapes;
using Capstone_Project.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Capstone_Project.Fundamentals.DrawableShapes
{
    public class DCircle : DShape
    {
        public float Radius { get; set; }

        public DCircle(Vector2 centre, float radius, bool scanned = true) : base(centre)
        {
            Scanned = scanned;
            Radius = radius;

            BoundingBox = new Rectangle((int)(centre.X - radius), (int)(centre.Y - radius), (int)(2 * radius) + 1, (int)(2 * radius) + 1);
            Outline = GraphicalMethods.GenerateOutline(centre, radius);
            GenerateFillMode(Scanned);
        }

        public DCircle(Vector2 centre, float radius, Color colour, float layer = 0.005f, bool scanned = true)
            : base(centre, colour, layer, scanned)
        {
            Radius = radius;

            BoundingBox = new Rectangle((int)(centre.X - radius), (int)(centre.Y - radius), (int)(2 * radius) + 1, (int)(2 * radius) + 1);

            Outline = GraphicalMethods.GenerateOutline(centre, radius);
            GenerateFillMode(scanned);
        }

        public DCircle(CCircle circ, bool scanned = true) : base(circ.Centre)
        {
            Scanned = scanned;
            Radius = circ.Radius;

            BoundingBox = circ.BoundingBox;

            Outline = GraphicalMethods.GenerateOutline(circ.Centre, circ.Radius);
            GenerateFillMode(scanned);
        }

        public DCircle(CCircle circ, Color colour, float layer = 0.005f, bool scanned = true) : base(circ.Centre, colour, layer, scanned)
        {
            Radius = circ.Radius;

            BoundingBox = circ.BoundingBox;

            Outline = GraphicalMethods.GenerateOutline(circ.Centre, circ.Radius);
            GenerateFillMode(scanned);
        }

        public override void MoveTo(Vector2 target, float rotation = 0f)
        {
            Centre = target;
            //BoundingBox = new Rectangle((int)(Centre.X - Radius), (int)(Centre.Y - Radius), 2 * (int)Radius + 1, 2 * (int)Radius + 1);
            BoundingBox = BoundingBox with { X = (int)(Centre.X - Radius), Y = (int)(Centre.Y - Radius) };

            Outline = GraphicalMethods.GenerateOutline(Centre, Radius);
            GenerateFillMode(Scanned);
        }

        protected override void GenerateFillMode(bool scanned)
        {
            Point roundedCentre = new Point(Utility.Round(Centre.X), Utility.Round(Centre.Y));
            if (scanned)
                ScanLines = GraphicalMethods.GenerateCircleLineFill(roundedCentre, Outline, Utility.Round(Radius));
            else
                Pixels = GraphicalMethods.GenerateCirclePixelFill(roundedCentre, Outline, Utility.Round(Radius));
        }
    }
}
