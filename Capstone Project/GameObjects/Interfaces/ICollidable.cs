using Capstone_Project.CollisionStuff;
using Capstone_Project.CollisionStuff.CollisionShapes;

namespace Capstone_Project.GameObjects.Interfaces
{
    public interface ICollidable
    {
        public bool Active { get; }
        public CShape Collider { get; }
        public bool CollidesWith(ICollidable other, out CollisionDetails cd);
    }
}
