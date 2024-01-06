using Capstone_Project.CollisionStuff.CollisionShapes;
using Microsoft.Xna.Framework;

namespace Capstone_Project.CollisionStuff
{
    // Rect (Rectangle) is interchangeable with Tile
    public struct CollisionDetails
    {
        public bool Collided { get; set; } = false;

        public CShapes AType { get; set; } = CShapes.None;
        public CShape A { get; set; } = null;
        public Vector2 ANormal { get; set; } = Vector2.Zero;

        public CShapes BType { get; set; } = CShapes.None;
        public CShape B { get; set; } = null;
        public Vector2 BNormal { get; set; } = Vector2.Zero;

        public Rectangle Intersection { get; set; } = Rectangle.Empty;
        public int IntersectionArea => Intersection.Width * Intersection.Height;

        public float Depth { get; set; } = 0f;

        public CollisionDetails() { }

        public CollisionDetails(CShapes aType, CShape a, CShapes bType, CShape b)
        {
            AType = aType;
            A = a;

            BType = bType;
            B = b;
        }

        public void SwapAB()
        {
            (AType, BType) = (BType, AType);
            (A, B) = (B, A);
            (ANormal, BNormal) = (BNormal, ANormal);
        }
    }
}
