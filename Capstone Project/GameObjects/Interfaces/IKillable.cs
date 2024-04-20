
namespace Capstone_Project.GameObjects.Interfaces
{
    public interface IKillable
    {
        public uint ID { get; init; }       // PLAYER ALWAYS HAS ID = 0
        public bool Dead { get; }
        public IAttacker Killer { get; }
        public void Kill();
    }
}
