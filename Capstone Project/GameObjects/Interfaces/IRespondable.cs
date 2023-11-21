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
        public bool CanMove => CollidesWith.Any();
        public List<ICollidable> CollidesWith { get; }
        public Rectangle ProjectHitbox();
        public void FinallyMove(Vector2 newPos);
    }
}
