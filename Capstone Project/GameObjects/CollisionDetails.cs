using Capstone_Project.GameObjects.Interfaces;
using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects
{
    // Rect (Rectangle) is interchangeable with Tile
<<<<<<< HEAD
<<<<<<< Updated upstream
    public enum CollisionType { None, RectOnRect, RectOnCirc, CircOnCirc }
=======
    public enum CollisionType { None, RectOnRect, CircOnRect, CircOnCirc }
>>>>>>> bc39f8d78e4142e23321cca44295f357bb9c4054
    public class CollisionDetails
=======
    public enum CollisionType { None, RectOnRect, CircOnRect, CircOnCirc }
    public struct CollisionDetails
>>>>>>> Stashed changes
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
