using Capstone_Project.Collision;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Capstone_Project.GameObjects.Interfaces
{
    /// <summary>
    /// Responds to collision - different to ICollidable, as that just means it can be collided against,
    /// but not handle the collision itself/move due to collidion handling
    /// </summary>
    public interface IRespondable : IMovable, ICollidable
    {
        public Vector2 TargetPos { get; set; }                      // where the implementer wants to move, regardless of collision status 
        public Rectangle TargetHitbox { get; }
        public Rectangle PathCollider { get; }                      // the large Rectangle that encapsulates both the current Position and TargetPos Hitboxes
        public LinkedList<CollisionDetails> Collisions { get; }     // this should be ordered by Rectangle intersection area ([0] = largest intersection, [n] = smallest intersection) 
        public void InsertIntoCollisions(CollisionDetails details); // this method should ensure the above ^ to be true
        public void HandleCollisions();
        public void Move();                                         // should be called after collision is dealt with
    }
}
