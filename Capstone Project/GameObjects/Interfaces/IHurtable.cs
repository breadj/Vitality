
namespace Capstone_Project.GameObjects.Interfaces
{
    public interface IHurtable : ICollidable
    {
        public int Vitality { get; }    // basically hit points
        public float Defence { get; }
        //public void TakeDamage(float damage, float invincibilityTime);
        public void TakeDamage(float damage, IAttacker attacker, float invincibilityTime);
        public bool Invincible => !Invincibility.Done;
        public Timer Invincibility { get; }
    }
}
