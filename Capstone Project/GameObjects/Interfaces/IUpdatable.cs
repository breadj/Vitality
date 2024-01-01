using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects.Interfaces
{
    public interface IUpdatable
    {
        public bool Active { get; }
        public void Update(GameTime gameTime);
    }
}
