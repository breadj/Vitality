using Capstone_Project.CollisionStuff;
using Capstone_Project.Fundamentals;
using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects.Interfaces
{
    /// <summary>
    /// Responds to collision - different to ICollidable, as that just means it can be collided against,
    /// but not handle the collision itself/move due to collidion handling
    /// </summary>
    public interface IRespondable : IMovable, ICollidable
    {
        public Rectangle OldBoundingBox { get; }
        public Vector2 TargetPos { get; set; }                      // where the implementer wants to move, regardless of collision status
        public Rectangle PathCollider => Collision.GeneratePathCollider(OldBoundingBox, Collider.BoundingBox);  // the large Rectangle that encapsulates both the current Position and TargetPos Hitboxes
        public SortedLinkedList<(ICollidable Other, CollisionDetails Details)> Collisions { get; }    // this should be ordered by Rectangle intersection area ([0] = largest intersection, [n-1] = smallest intersection) 
        //public void InsertIntoCollisions(ICollidable other, CollisionDetails details);          // this method should ensure the above ^ to be true
        public void HandleCollisions();                             // also moves the actual responder position to the target position after all collisions accounted for
    }
}
