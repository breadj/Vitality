using Capstone_Project.Collision.CollisionShapes;

namespace Capstone_Project.GameObjects.Interfaces
{
    public interface ICollidable
    {
        public bool Active { get; }
        public CShape Collider { get; }
    }
}
