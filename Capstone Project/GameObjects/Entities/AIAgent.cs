using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Capstone_Project.GameObjects.Entities
{
    // Guard = stick to a point until aggro'd, Patrol = endlessly walk a path until aggro'd, Pursuit = chase and attack aggro target
    public enum AIState { Guard, Patrol, Pursuit }

    public class AIAgent : Agent
    {
        public Stack<AIState> CurrentState = new Stack<AIState>();

        public AIAgent(Subsprite subsprite, Vector2 position, int vitality, float damage, float attackRange = 100, float defence = 0, int size = 0, float speed = 1)
            : base(subsprite, position, vitality, damage, attackRange, defence, size, speed)
        {

        }

        public void Move()
        {

        }
    }
}
