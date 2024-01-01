
namespace Capstone_Project.GameObjects.Interfaces
{
    public interface IAttacker
    {
        public float Damage { get; }
        public Attack Attack { get; }
        public float Range { get; }
        public void Swing();
    }
}
