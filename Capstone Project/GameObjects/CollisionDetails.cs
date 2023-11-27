using Capstone_Project.GameObjects.Interfaces;
using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects
{
    // Rect (Rectangle) is interchangeable with Tile
    public enum CollisionType { None, RectOnRect, CircOnRect, CircOnCirc }
    public class CollisionDetails
    {
        public CollisionType Type { get; set; } = CollisionType.None;

        // order only really matters if the CollisionType is CircOnRect
        public ICollidable From { get; set; } = null;
        public ICollidable Against { get; set; } = null;

        public Rectangle Intersection { get; set; } = Rectangle.Empty;
        public int IntersectionArea => Intersection.Width * Intersection.Height;

        // only used for RectOnCirc
        public bool CornerCollision { get; set; } = false;
        public float IntersectionDepth { get; set; } = 0;

        // if the CollisionType is anything other than None then it returns true to a collision
        public bool HasCollided => !(Type == CollisionType.None);

        public CollisionDetails()
        {
            
        }

        public static implicit operator bool(CollisionDetails cd) => cd.HasCollided;
    }
}
