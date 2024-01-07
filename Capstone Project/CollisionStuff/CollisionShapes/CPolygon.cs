using Capstone_Project.Fundamentals.DrawableShapes;
using Capstone_Project.Globals;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Capstone_Project.CollisionStuff.CollisionShapes
{
    public class CPolygon : CShape
    {
        public override bool Dynamic { get; init; }     // will ALWAYS be false
        public new Rectangle BoundingBox { get; set; }
        public Vector2[] Vertices { get; init; }

        // removed constructor for centre = (0, 0), now vertices MUST be local
        public CPolygon(Vector2 centre, Vector2[] vertices) : base(centre)
        {
            Dynamic = false;
            Vertices = vertices.Select(vertex => vertex + centre).ToArray();

            BoundingBox = GenerateBoundingBox();
        }

        public CPolygon(DPolygon poly) : base(poly.Centre)
        {
            Dynamic = false;
            Vertices = poly.Vertices;

            BoundingBox = poly.BoundingBox;
        }

        protected override Rectangle GenerateBoundingBox()
        {
            return Utility.GenerateBoundingBox(Vertices);
        }
    }
}
