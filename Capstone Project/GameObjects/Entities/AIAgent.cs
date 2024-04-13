using Capstone_Project.CollisionStuff;
using Capstone_Project.Fundamentals;
using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.Globals;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Capstone_Project.GameObjects.Entities
{
    // Guard = stick to a point until aggro'd, Patrol = endlessly walk a path until aggro'd, Pursuit = chase and attack aggro target
    public enum AIState { None, Guard, Patrol, Pursuit }    // None should not be used unless for default value

    public class AIAgent : Agent, IPatroller, IPursuer
    {
        #region Default Attributes
        public static readonly AIState DefaultInitialAIState = AIState.Guard;
        public static readonly PatrolType DefaultPatrolType = PatrolType.Boomerang;
        public static readonly bool DefaultPatrolDirectionIsForward = true;
        public static readonly int DefaultFirstPatrolPointIndex = 1;

        public static readonly int DefaultAggoRange = 300;
        public static readonly float DefaultLoseAggroTime = 5f;
        #endregion Default Attributes


        public Stack<AIState> CurrentState { get; init; } = new Stack<AIState>();
        public PatrolType PatrolType { get; init; }
        protected bool patrolDirectionIsForward = true;
        public LinkedList<Vector2> PatrolPoints { get; init; }
        public LinkedListNode<Vector2> CurrentPatrolPoint { get; private set; }
        protected LinkedList<Vector2> returnToPatrolPath = new LinkedList<Vector2>();

        public bool Aggroed => PursuitTarget != null;
        public int AggroRange { get; protected set; }
        public Entity PursuitTarget { get; set; } = null;
        public Vector2 TargetLastSeen { get; protected set; }
        public LinkedList<Vector2> PursuitPath { get; } = new LinkedList<Vector2>();
        public Timer LoseAggroTimer { get; } = new Timer(5f);

        protected bool Hovering { get; set; } = false;

        public bool TargetInView { get; protected set; }
        protected LinkedList<Vector2> returnToGuardPosition = new LinkedList<Vector2>();

        public AIAgent(bool? visible = null, string spriteName = null, Color? colour = null, float? rotation = null, float? layer = null,
            bool? active = null, Vector2? position = null, Vector2? direction = null, Vector2? velocity = null, float? speed = null,
            int? size = null, bool? dead = null, Comparer<(ICollidable, CollisionDetails)> collisionsComparer = null, Vector2? orientation = null,
            float? leakPercentage = null, int? maxVitality = null, int? vitality = null, float? defence = null, float? damage = null,
            float? windupTime = null, float? lingerTime = null, float? cooldownTime = null, float? attackRange = null, 
            float? dashTime = null, float? dashSpeedModifier = null, float? invincibilityTime = null,
            AIState? initialAIState = null, PatrolType? patrolType = null, bool? patrolDirectionIsForward = null,
            LinkedList<Vector2> patrolPoints = null, int? firstPatrolPointIndex = null, int? aggroRange = null, float? loseAggroTime = null)
            : base(visible, spriteName, colour, rotation, layer, active, position, direction, velocity, speed, size, dead,
                  collisionsComparer, orientation, leakPercentage, maxVitality, vitality, defence, damage, windupTime, lingerTime, 
                  cooldownTime, attackRange, dashTime, dashSpeedModifier, invincibilityTime)
        {
            PatrolType = patrolType ?? DefaultPatrolType;

            this.patrolDirectionIsForward = patrolDirectionIsForward ?? DefaultPatrolDirectionIsForward;
            AggroRange = aggroRange ?? DefaultAggoRange;

            // special stuff
            CurrentState.Push(initialAIState ?? DefaultInitialAIState);

            if (patrolPoints != null && patrolPoints.Count > 0)
            {
                PatrolPoints = patrolPoints;

                int fppi = firstPatrolPointIndex ?? DefaultFirstPatrolPointIndex;
                int count = 0;
                for (var cur = patrolPoints.First; cur != null; cur = cur.Next)
                {
                    if (count++ == fppi)
                    {
                        CurrentPatrolPoint = cur;
                        break;
                    }
                }
            }
            else
            {
                PatrolPoints = new LinkedList<Vector2>();
                CurrentPatrolPoint = PatrolPoints.AddFirst(Position);
            }

            LoseAggroTimer = new Timer(loseAggroTime ?? DefaultLoseAggroTime);
        }

        /*public AIAgent(string spriteName, Subsprite subsprite, Vector2 position, int vitality, float damage, LinkedList<Vector2> patrolPoints = null, int firstPatrolPointIndex = 0, 
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
        }*/

        public override void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            Look();
            Move();
            Attack();

            Invincibility.Update(gameTime);
            LoseAggroTimer.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();

            /*Globals.Globals.spriteBatch.DrawString(Globals.Globals.DebugFont, $"Speed: {Speed}", Position, Color.Black, 0f, 
                Vector2.Zero, 2f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0.9f);*/
            /*Globals.Globals.spriteBatch.DrawString(Globals.Globals.DebugFont, $"Attack Timer: {attackTimer.Percentage}", Position,
                Color.Black, 0f, Vector2.Zero, 1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0.9f);*/
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
            // should be more sophisticated so it can recognise agents other than the Player as a target, but unnecessary right now
            if ((Game1.Player.Position - this.Position).Length() - (Game1.Player.Size + this.Size) / 2f <= AggroRange &&
                TargetInLineOfSight(Game1.Player.Position))
            {
                CurrentState.Push(AIState.Pursuit);
                PursuitTarget = Game1.Player;
            }
            else
            {
                PursuitTarget = null;
            }

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
                    else
                    {
                        TargetInView = false;
                    }
                    break;
                case AIState.Guard:
                    break;
            }

            // TOTO: add rotation speed
            Orientation = newOrientation;

            base.Look();
        }

        private readonly Timer attackTimer = new Timer(3f, startImmediately: true);
        public override void Attack()
        {
            if (Strike.Active || Strike.OnCD)
            {
                Strike.Update(Globals.Globals.gameTime);
            }

            if (Hovering)
            {
                if (attackTimer.Done)
                {
                    Swing();
                    // since distance will always be <= AttackRange, it can be divided by AttackRange to get a float from 0-1, giving a somewhat-predictable attack timing pattern
                    float distanceFromPursuitTarget = (PursuitTarget.Position - this.Position).Length() - PursuitTarget.Size / 2f;

                    attackTimer.SetNewWaitTime(distanceFromPursuitTarget / AttackRange * 7f);
                    attackTimer.Start();
                }

                if (!Strike.Active)
                {
                    attackTimer.Update(Globals.Globals.gameTime);
                }
            }
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
                }
            }
            else if (Aggroed) // basically if pursuing the same target as last frame (and that target isn't null)
            {
                if (TargetInLineOfSight(PursuitTarget.Position))
                {
                    if (PursuitPath.Count > 0)
                        PursuitPath.Clear();
                    
                    // has to take into account the size of the target (not this, since the Attack originates from the centre)
                    if ((PursuitTarget.Position - this.Position).Length() - PursuitTarget.Size / 2f <= AttackRange)
                    {
                        HoverOrAttack();
                    }
                    else
                    {
                        if (Hovering)
                        {
                            Speed = BaseSpeed;
                            Hovering = false;
                        }
                        MoveTowards(PursuitTarget.Position);
                    }
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

            Speed = Hovering ? BaseSpeed / 2f : BaseSpeed;

            prevFrameTarget = PursuitTarget;
        }

        private float? prevDistanceFromPursuitTarget = null;
        protected void HoverOrAttack()
        {
            // should only be called inside Pursuit() if the target is in the line of sight and within the AttackRange
            if (Strike.Lock)
                return;

            float distanceFromPursuitTarget = (PursuitTarget.Position - this.Position).Length() - PursuitTarget.Size / 2f;

            bool atHoverDistance = false;
            if (prevDistanceFromPursuitTarget.HasValue)
            {
                atHoverDistance = prevDistanceFromPursuitTarget < AttackRange / 2f != distanceFromPursuitTarget < AttackRange / 2f;
            }

            // if agent has only just gotten into the correct position (half the attack range away from the target)
            if (!Hovering && atHoverDistance)
            {
                Hovering = true;
                Speed = BaseSpeed / 2f;
            }

            if (Hovering)
            {
                if (!atHoverDistance)
                {
                    if (distanceFromPursuitTarget > AttackRange)
                    {
                        Hovering = false;
                        MoveTowards(PursuitTarget.Position);
                    }
                    else
                    {
                        // if the distance between this and the PursuitTarget is less than half the AttackRange, move away from the PursuitTarget
                        var directionModifier = distanceFromPursuitTarget < AttackRange / 2f ? -1 : 1;
                        Direction = directionModifier * Vector2.Normalize(PursuitTarget.Position - this.Position);
                        base.Move();
                    }
                }
            }
            else
            {
                MoveTowards(PursuitTarget.Position);
            }

            prevDistanceFromPursuitTarget = distanceFromPursuitTarget;
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

        /*public static AIAgent Create(string spriteName = null, Subsprite subsprite = null, Vector2? position = null, int? vitality = null, float? damage = null, LinkedList<Vector2> patrolPoints = null, 
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
        }*/
    }
}
