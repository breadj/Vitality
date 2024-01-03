using Capstone_Project.Collision;
using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects.Interfaces
{
    public interface ICollidable
    {
        public bool Active { get; }
        public Rectangle Hitbox { get; }
        public bool IsCircle { get; }
        public float Radius { get; }
        public CollisionDetails CollidesWith(ICollidable other);
    }
}
