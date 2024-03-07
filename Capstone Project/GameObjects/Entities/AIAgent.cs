using Capstone_Project.CollisionStuff;
using Capstone_Project.Fundamentals;
using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Capstone_Project.GameObjects.Entities
{
    // Guard = stick to a point until aggro'd, Patrol = endlessly walk a path until aggro'd, Pursuit = chase and attack aggro target
    public enum AIState { None, Guard, Patrol, Pursuit }    // None should not be used unless for defaul value

    public class AIAgent : Agent, IPatroller, IPursuer
    {
        #region Default Attributes
        public static LinkedList<Vector2> DefaultPatrolPoints { get; } = new LinkedList<Vector2>(new List<Vector2>{ Vector2.Zero });        // hacky solution but oh well, it should only compute once
        public static AIState DefaultAIState { get; } = AIState.Guard;
        public static PatrolType DefaultPatrolType { get; } = PatrolType.Boomerang;
        #endregion Default Attributes


        public Stack<AIState> CurrentState { get; init; } = new Stack<AIState>();
        public PatrolType PatrolType { get; init; }
        protected bool patrolDirectionIsForward = true;
        public LinkedList<Vector2> PatrolPoints { get; init; }
        public LinkedListNode<Vector2> CurrentPatrolPoint { get; private set; }

        public bool Aggroed => PursuitTarget != null;
        public Entity PursuitTarget { get; set; } = null;
        public Vector2 TargetLastSeen { get; protected set; }
        public LinkedList<Vector2> PursuitPath { get; protected set; }

        public List<Vector2> Path { get; protected set; }

        public AIAgent(Subsprite subsprite, Vector2 position, int vitality, float damage, LinkedList<Vector2> patrolPoints = null, int firstPatrolPointIndex = 0, 
            AIState initialState = AIState.Guard, PatrolType patrolType = PatrolType.Boomerang, float attackRange = 100, float defence = 0, int size = 0, float speed = 1)
            : base(subsprite, patrolPoints == null ? position : patrolPoints.ElementAt(firstPatrolPointIndex), vitality, damage, attackRange, defence, size, speed)
        {
            CurrentState.Push(initialState);
            PatrolType = patrolType;
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
            if (!Active)
                return;

            Move();

            Invincibility.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void Move()
        {
            switch (CurrentState.Peek())
            {
                case AIState.Guard:
                    break;
                case AIState.Patrol:
                    Patrol();
                    break;
                case AIState.Pursuit:
                    Pursue();
                    break;
            }
        }

        public override void Look()
        {
            base.Look();
        }

        public override void Attack()
        {
            base.Attack();
        }

        public void Patrol()
        {
            if (ArrivedAtPatrolPoint())
            {
                switch (PatrolType)
                {
                    case PatrolType.None:
                        CurrentPatrolPoint = CurrentPatrolPoint;
                        break;
                    case PatrolType.Circular:
                        CurrentPatrolPoint = CurrentPatrolPoint.Next ?? PatrolPoints.First;
                        break;
                    case PatrolType.Boomerang:
                        var next = patrolDirectionIsForward ? CurrentPatrolPoint.Next : CurrentPatrolPoint.Previous;
                        if (next == null)
                        {
                            patrolDirectionIsForward = !patrolDirectionIsForward;
                            next = patrolDirectionIsForward ? CurrentPatrolPoint.Next : CurrentPatrolPoint.Previous;
                        }
                        CurrentPatrolPoint = next;
                        break;
                }
            }
            else
            {
                if (Collision.CastRay(new Ray2D(Position, CurrentPatrolPoint.Value), (List<ICollidable>)Game1.SimulatedTiles.Cast<ICollidable>()))
                {
                    // DO SHIT WITH THIS - Pathfinding.FindPath(Game1.TileMap.Walls, Position, PatrolPoints.ToArray());
                }
                MoveTowards(CurrentPatrolPoint.Value);
            }
        }

        protected bool ArrivedAtPatrolPoint()
        {
            // if this comes into 5px of the CurrentPatrolPoint
            return (Position - CurrentPatrolPoint.Value).LengthSquared() < 25f;     // 5px^2 = 25
        }

        protected Entity prevFrameTarget;
        public void Pursue()
        {
            if (TargetInLineOfSight())
            { /* do something (or get rid of this/move it) */ }

            if (prevFrameTarget != PursuitTarget)
            {
                if (Aggroed) // and didn't have a Target last frame
                {

                }
                else
                {
                    
                }
            }
            else if (Aggroed) // basically if PursuitTarget != null AND prevFrameTarget != null
            {

            }

            prevFrameTarget = PursuitTarget;
        }

        public bool TargetInLineOfSight()
        {
            return Collision.CastRay(new Ray2D(Position, CurrentPatrolPoint.Value), (List<ICollidable>)Game1.SimulatedTiles.Cast<ICollidable>());
        }

        protected void MoveTowards(Vector2 target)
        {

        }

        public static AIAgent Create(Subsprite subsprite = null, Vector2? position = null, int? vitality = null, float? damage = null, LinkedList<Vector2> patrolPoints = null, 
            int firstPatrolPointIndex = 0, AIState? initialState = null, PatrolType? patrolType = null, float? attackRange = null, float? defence = null, int? size = null, float? speed = null)
        {
            if (position == null && patrolPoints != null)
            {
                position = patrolPoints.ElementAt(firstPatrolPointIndex);
            }
            else if (position != null && patrolPoints == null)
            {
                patrolPoints = new LinkedList<Vector2>();
                patrolPoints.AddFirst(position.Value);
            }

            return new AIAgent(
                subsprite ?? DefaultSprite,
                position ?? DefaultPosition,
                vitality ?? DefaultVitality,
                damage ?? DefaultDamage,
                patrolPoints ?? DefaultPatrolPoints,
                firstPatrolPointIndex,
                initialState ?? DefaultAIState,
                patrolType ?? DefaultPatrolType,
                attackRange ?? DefaultRange,
                defence ?? DefaultDefence,
                size ?? DefaultSize,
                speed ?? DefaultSpeed
                );
        }
    }
}
