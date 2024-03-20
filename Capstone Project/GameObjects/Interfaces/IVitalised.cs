
namespace Capstone_Project.GameObjects.Interfaces
{
    public interface IVitalised : IUpdatable, IKillable, IAttacker, IHurtable
    {
        //public int Vitality { get; }          -> already a member of IHurtable
        public int MaxVitality { get; }
        public float LeakPercentage { get; }    // how much Vitality is leaked when LeakVitality() is used

        public void AbsorbVitality(IVitalised vitalised);
        public int LeakVitality();      // should be lossy, aka: only X% of Vitality lost is 'leaked' and therefore absorbed by the killer
        public int LetVitality();       // should be 100% transfer amount/rate
    }
}
