using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects.Interfaces
{
    public interface ICollidable
    {
        public bool Active { get; }
        public Rectangle Hitbox { get; }
    }
}
