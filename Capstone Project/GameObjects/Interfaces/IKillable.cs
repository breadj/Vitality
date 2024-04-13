
namespace Capstone_Project.GameObjects.Interfaces
{
    public interface IKillable
    {
        public bool Dead { get; }
        public IAttacker Killer { get; }
        public void Kill();
    }
}
