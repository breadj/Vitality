using Capstone_Project.CollisionStuff;
using Capstone_Project.Fundamentals;
using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.Globals;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Capstone_Project.GameObjects.Entities
{
    // Guard = stick to a point until aggro'd, Patrol = endlessly walk a path until aggro'd, Pursuit = chase and attack aggro target
    public enum AIState { None, Guard, Patrol, Pursuit }    // None should not be used unless for defaul value

    public class AIAgent : Agent, IPatroller, IPursuer
    {
        #region Default Attributes
        public static LinkedList<Vector2> DefaultPatrolPoints { get; } = new LinkedList<Vector2>(new List<Vector2>{ DefaultPosition });     // hacky solution but oh well, it should only compute once
        public static AIState DefaultAIState { get; } = AIState.Guard;
        public static PatrolType DefaultPatrolType { get; } = PatrolType.Boomerang;
        #endregion Default Attributes


        public Stack<AIState> CurrentState { get; init; } = new Stack<AIState>();
        public PatrolType PatrolType { get; init; }
        protected bool patrolDirectionIsForward = true;
        public LinkedList<Vector2> PatrolPoints { get; init; }
        public LinkedListNode<Vector2> CurrentPatrolPoint { get; private set; }
        protected LinkedList<Vector2> returnToPatrolPath = new LinkedList<Vector2>();

        public bool Aggroed => PursuitTarget != null;
        public Entity PursuitTarget { get; set; } = null;
        public Vector2 TargetLastSeen { get; protected set; }
        public LinkedList<Vector2> PursuitPath { get; } = new LinkedList<Vector2>();
        public Timer LoseAggroTimer { get; } = new Timer(5f);

        public bool TargetInView { get; protected set; }
        protected LinkedList<Vector2> returnToGuardPosition = new LinkedList<Vector2>();

        public AIAgent(string spriteName, Subsprite subsprite, Vector2 position, int vitality, float damage, LinkedList<Vector2> patrolPoints = null, int firstPatrolPointIndex = 0, 
            AIState initialState = AIState.Guard, PatrolType patrolType = PatrolType.Boomerang, float attackRange = 100, float defence = 0, int size = 0, float speed = 1)
            : base(spriteName, subsprite, position, vitality, damage, attackRange, defence, size, speed)
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
                        break;
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

            Look();
            Move();

            Invincibility.Update(gameTime);
            LoseAggroTimer.Update(gameTime);
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
                    base.Move();
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
            Vector2 newOrientation = Orientation;
            switch (CurrentState.Peek())
            {
                case AIState.Patrol:
                    newOrientation = CurrentPatrolPoint.Value - Position;
                    TargetInView = TargetInLineOfSight(CurrentPatrolPoint.Value);
                    break;
                case AIState.Pursuit:
                    if (Aggroed && TargetInLineOfSight(PursuitTarget.Position))
                    {
                        newOrientation = PursuitTarget.Position - Position;
                        TargetInView = true;
                    }
                    else if (PursuitPath.Count > 0)
                    {
                        newOrientation = PursuitPath.First.Value - Position;
                        TargetInView = TargetInLineOfSight(PursuitPath.First.Value);
                    }
                    break;
                case AIState.Guard:
                    break;
            }

            // TOTO: add rotation speed
            Orientation = newOrientation;

            base.Look();
        }

        public override void Attack()
        {
            base.Attack();
        }

        public void Patrol()
        {
            if (returnToPatrolPath.Count == 0)
            {
                if (ArrivedAtPathPoint(CurrentPatrolPoint))
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
                    if (!TargetInLineOfSight(CurrentPatrolPoint.Value))
                    {
                        returnToPatrolPath = new LinkedList<Vector2>(Pathfinding.FindPath(Game1.TileMap.Walls, Position, PatrolPoints.ToArray()));
                    }
                }
            }
            
            if (returnToPatrolPath.Count > 0)
            {
                MoveTowards(returnToPatrolPath.First.Value);
                if (ArrivedAtPathPoint(returnToPatrolPath.First))
                {
                    returnToPatrolPath.RemoveFirst();
                }
            }
            else
            {
                MoveTowards(CurrentPatrolPoint.Value);
            }
        }

        protected bool ArrivedAtPathPoint(LinkedListNode<Vector2> nodePoint)
        {
            // if this comes into 5px of the CurrentPatrolPoint
            return (Position - nodePoint.Value).LengthSquared() < 25f;      // 5px^2 = 25
        }

        protected Entity prevFrameTarget;
        public void Pursue()
        {
            //if (TargetInLineOfSight())
            { /* do something (or get rid of this/move it) */ }

            if (prevFrameTarget != PursuitTarget)
            {
                if (Aggroed) // new target starting this frame
                {
                    if (PursuitPath.Count > 0)
                        PursuitPath.Clear();

                    if (TargetInLineOfSight(PursuitTarget.Position))
                    {
                        MoveTowards(PursuitTarget.Position);
                    }
                    else
                    {
                        PursuitPath.AppendLinkedList(new LinkedList<Vector2>(Pathfinding.FindPath(Game1.TileMap.Walls, Position, PursuitTarget.Position)));
                        MoveTowards(PursuitPath.First.Value);
                    }
                }
                else // if this frame is the first without a target
                {
                    PursuitPath.Clear();
                    CurrentState.Pop();
                    
                    return;
                }
            }
            else if (Aggroed) // basically if pursuing the same target as last frame (and that target isn't null)
            {
                if (TargetInLineOfSight(PursuitTarget.Position))
                {
                    if (PursuitPath.Count > 0)
                        PursuitPath.Clear();
                    MoveTowards(PursuitTarget.Position);
                }
                else if (PursuitPath.Count > 0)
                {
                    if (ArrivedAtPathPoint(PursuitPath.First))
                    {
                        PursuitPath.RemoveFirst();
                    }
                    
                    if (PursuitPath.Count > 0)
                    {
                        MoveTowards(PursuitPath.First.Value);
                    }
                }
            }
            else
            {
                PursuitPath.Clear();
                CurrentState.Pop();
            }

            prevFrameTarget = PursuitTarget;
        }

        public bool TargetInLineOfSight(Vector2 target)
        {
            return Position == target || !Collision.CastRay(new Ray2D(Position, target), Game1.SimulatedTiles.Cast<ICollidable>().ToList());
        }

        protected void MoveTowards(Vector2 target)
        {
            Direction = Vector2.Normalize(target - Position);

            base.Move();
        }

        public static AIAgent Create(string spriteName = null, Subsprite subsprite = null, Vector2? position = null, int? vitality = null, float? damage = null, LinkedList<Vector2> patrolPoints = null, 
            int? firstPatrolPointIndex = 0, AIState? initialState = null, PatrolType? patrolType = null, float? attackRange = null, float? defence = null, int? size = null, float? speed = null)
        {
            firstPatrolPointIndex ??= 0;

            if (position == null && patrolPoints != null)
            {
                position = patrolPoints.ElementAt(firstPatrolPointIndex.Value);
            }
            else if (position != null && patrolPoints == null)
            {
                patrolPoints = new LinkedList<Vector2>();
                patrolPoints.AddFirst(position.Value);
            }

            return new AIAgent(
                spriteName ?? DefaultSpriteName,
                subsprite ?? DefaultSprite,
                position ?? DefaultPosition,
                vitality ?? DefaultVitality,
                damage ?? DefaultDamage,
                patrolPoints ?? DefaultPatrolPoints,
                firstPatrolPointIndex.Value,
                initialState ?? DefaultAIState,
                patrolType ?? DefaultPatrolType,
                attackRange ?? DefaultAttackRange,
                defence ?? DefaultDefence,
                size ?? DefaultSize,
                speed ?? DefaultSpeed
                );
        }
    }
}
