using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects.Interfaces
{
    public interface IMovable : IUpdatable
    {
        public Vector2 Position { get; }
        public Vector2 Direction { get; }
        public Vector2 Velocity { get; }
        public float Speed { get; }
    }
}
