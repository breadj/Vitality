
namespace Capstone_Project.GameObjects.Interfaces
{
    public interface IDasher : IMovable
    {
        public Dash Dash { get; }
        public float BaseSpeed { get; }
        public float DashTime { get; }
        public float DashSpeedModifier { get; }
    }
}
