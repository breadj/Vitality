using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects.Entities
{
    public class Enemy : Agent
    {
        public Enemy(Subsprite subsprite, Vector2 position, int vitality, float damage, float attackRange = 100f, float defence = 0f, int size = 100, float speed = 100f)
            : base(subsprite, position, vitality, damage, attackRange, defence, size, speed)
        {
            //Direction = new Vector2(0, 1);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
