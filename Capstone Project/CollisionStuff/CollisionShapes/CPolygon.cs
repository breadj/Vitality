using Capstone_Project.Fundamentals.DrawableShapes;
using Capstone_Project.Globals;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Capstone_Project.CollisionStuff.CollisionShapes
{
    public class CPolygon : CShape
    {
        //public bool Dynamic { get; init; }     // false by default
        public Vector2[] Vertices { get; protected set; }

        // removed constructor for centre = (0, 0), now vertices MUST be local
        public CPolygon(Vector2 centre, Vector2[] vertices, bool dynamic = false) : base(centre)
        {
            Dynamic = dynamic;
            Vertices = vertices.Select(vertex => vertex + centre).ToArray();

            BoundingBox = GenerateBoundingBox();
        }

        public CPolygon(DPolygon poly) : base(poly.Centre)
        {
            Dynamic = false;
            Vertices = poly.Vertices;

            BoundingBox = poly.BoundingBox;
        }

        public override void MoveTo(Vector2 target, float rotation = 0f)
        {
            Vector2 displacement = target - Centre;

            if (rotation != 0f)
                Vertices = DPolygon.Rotate(Vertices, rotation, Centre);
            if (target != Centre)
                Vertices = Vertices.Select(vertex => vertex + displacement).ToArray();

            base.MoveTo(target);
        }

        protected override Rectangle GenerateBoundingBox()
        {
            return Utility.GenerateBoundingBox(Vertices);
        }
    }
}
