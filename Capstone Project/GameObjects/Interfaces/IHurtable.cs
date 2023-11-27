
namespace Capstone_Project.GameObjects.Interfaces
{
    public interface IHurtable : ICollidable
    {
        public int Vitality { get; }    // basically hit points
        public float Defence { get; }
        public void TakeDamage(int damage);
    }
}
