using Microsoft.Xna.Framework;

namespace Capstone_Project.Fundamentals.DrawableShapes
{
    public class DCircle : DShape
    {
        public float Radius { get; set; }

        public DCircle(Vector2 centre, float radius) : base(centre)
        {
            Radius = radius;
        }

        public DCircle(Vector2 centre, float radius, Color colour, float layer = 0.005f) : base(centre, colour, layer)
        {
            Radius = radius;
        }

        public override void Draw()
        {
            throw new System.NotImplementedException();
        }
    }
}
