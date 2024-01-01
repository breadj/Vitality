using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects.Entities
{
    public class Enemy : Mob
    {
        public Enemy(Subsprite subsprite, Vector2 position, int size = 100, int speed = 100) : base(subsprite, position, size, speed)
        {
            //Direction = new Vector2(0, 1);
        }
    }
}
