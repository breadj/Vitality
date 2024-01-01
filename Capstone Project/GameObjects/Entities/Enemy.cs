using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects.Entities
{
    public class Enemy : Mob
    {
        public Enemy(Subsprite subsprite, Vector2 position, int size = 100, int speed = 100) : base(subsprite, position, size, speed)
        {
<<<<<<< HEAD
            //Direction = new Vector2(0, 1);
=======
            Direction = new Vector2(0, 1);
>>>>>>> bc39f8d78e4142e23321cca44295f357bb9c4054
        }
    }
}
