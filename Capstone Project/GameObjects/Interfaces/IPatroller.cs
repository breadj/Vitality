using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Capstone_Project.GameObjects.Interfaces
{
    public enum PatrolType { None, Circular, Boomerang }
    public interface IPatroller
    {
        public PatrolType PatrolType { get; }
        public LinkedList<Vector2> PatrolPoints { get; }
        public LinkedListNode<Vector2> CurrentPatrolPoint { get; }
    }
}
