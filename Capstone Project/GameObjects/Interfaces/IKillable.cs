
namespace Capstone_Project.GameObjects.Interfaces
{
    public interface IKillable
    {
        public bool Dead { get; }
        public void Kill();
    }
}
