using Capstone_Project.GameObjects.Entities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Capstone_Project.GameObjects.Interfaces
{
    public interface IPursuer : IMovable
    {
        public bool Aggroed { get; }
        public Entity PursuitTarget { get; }
        public Vector2 TargetLastSeen { get; }
        public LinkedList<Vector2> PursuitPath { get; }
        public void Pursue();
        public bool TargetInLineOfSight(Vector2 target);
    }
}
