
namespace Capstone_Project.GameObjects.Interfaces
{
    public interface IAttacker
    {
        public float Damage { get; }
        public Attack Attack { get; }
        public bool Attacking => Attack.Attacking;
        public float Range { get; }
        public void Swing();
    }
}
