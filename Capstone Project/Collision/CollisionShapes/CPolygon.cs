using Microsoft.Xna.Framework;
using System.Linq;

namespace Capstone_Project.Collision.CollisionShapes
{
    public class CPolygon : CShape
    {
        public override bool Dynamic { get; init; }     // will ALWAYS be false
        public Vector2[] Vertices { get; init; }

        // removed constructor for centre = (0, 0), now vertices MUST be local
        public CPolygon(Vector2 centre, Vector2[] vertices) : base(centre)
        {
            Dynamic = false;
            Vertices = vertices.Select(vertex => vertex + centre).ToArray();
        }
    }
}
