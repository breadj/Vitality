using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Capstone_Project.GameObjects.Entities
{
    // Guard = stick to a point until aggro'd, Patrol = endlessly walk a path until aggro'd, Pursuit = chase and attack aggro target
    public enum AIState { Guard, Patrol, Pursuit }

    public class AIAgent : Agent, IPatroller
    {
        public Stack<AIState> CurrentState { get; init; } = new Stack<AIState>();
        public PatrolType PatrolType { get; init; }
        public LinkedList<Vector2> PatrolPoints { get; init; }
        public LinkedListNode<Vector2> CurrentPatrolPoint { get; private set; }

        public AIAgent(Subsprite subsprite, Vector2 position, int vitality, float damage, LinkedList<Vector2> patrolPoints = null, int firstPatrolPointIndex = 0, 
            AIState initialState = AIState.Guard, PatrolType patrolType = PatrolType.Boomerang, float attackRange = 100, float defence = 0, int size = 0, float speed = 1)
            : base(subsprite, patrolPoints == null ? position : patrolPoints.ElementAt(firstPatrolPointIndex), vitality, damage, attackRange, defence, size, speed)
        {
            CurrentState.Push(initialState);
            PatrolPoints = patrolPoints;
            if (patrolPoints != null && patrolPoints.Count > 0)
            {
                int count = 0;
                for (var cur = patrolPoints.First; cur != null; cur = cur.Next)
                {
                    if (count++ == firstPatrolPointIndex)
                    {
                        CurrentPatrolPoint = cur;
                    }
                }
            }
            else
            {
                PatrolPoints = new LinkedList<Vector2>();
                CurrentPatrolPoint = PatrolPoints.AddFirst(position);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void Move()
        {
            
        }

        public override void Look()
        {
            base.Look();
        }

        public override void Attack()
        {
            base.Attack();
        }

        protected void MoveTowards(Vector2 target)
        {

        }
    }
}
