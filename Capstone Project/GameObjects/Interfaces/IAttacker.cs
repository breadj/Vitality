
namespace Capstone_Project.GameObjects.Interfaces
{
    public interface IAttacker
    {
        public float Damage { get; }
        public Attack Strike { get; }
        public bool Attacking => Strike.Attacking;
        public float Range { get; }
        public void Swing();
    }
}
