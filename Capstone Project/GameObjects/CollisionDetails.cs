using Capstone_Project.GameObjects.Hitboxes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Project.GameObjects
{
    // Rect (Rectangle) is interchangeable with Tile
<<<<<<< Updated upstream
    public enum CollisionType { None, RectOnRect, RectOnCirc, CircOnCirc }
    public class CollisionDetails
=======
    public enum CollisionType { None, RectOnRect, CircOnRect, CircOnCirc }
    public struct CollisionDetails
>>>>>>> Stashed changes
    {
        public CollisionType Type;
        
        // order only really matters if the CollisionType is RectOnCirc
        public IHitbox To;
        public IHitbox From;

        public Rectangle Intersection;

        // only used for RectOnCirc
        public bool CornerCollision;
        public float IntersectionDepth;

        // if the CollisionType is anything other than None then it returns true to a collision
        public bool HasCollided => !(Type == CollisionType.None);

        public CollisionDetails()
        {
            Type = CollisionType.None;
            To = null;
            From = null;
            Intersection = Rectangle.Empty;
            CornerCollision = false;
            IntersectionDepth = 0;
        }

        public static implicit operator bool(CollisionDetails cd) => cd.HasCollided;
    }
}
