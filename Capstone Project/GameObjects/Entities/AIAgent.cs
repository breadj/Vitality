using Capstone_Project.CollisionStuff;
using Capstone_Project.Fundamentals;
using Capstone_Project.Fundamentals.DrawableShapes;
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

        public static readonly float DefaultRotationSpeed = 1.5f;       // radians turned per second
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
        protected Entity prevFrameTarget = null;
        public Vector2? TargetLastSeen { get; protected set; } = null;
        public LinkedList<Vector2> PursuitPath { get; protected set; } = new LinkedList<Vector2>();
        public Timer LoseAggroTimer { get; } = new Timer(5f);

        protected bool Hovering { get; set; } = false;

        protected LinkedList<Vector2> returnToGuardPosition = new LinkedList<Vector2>();

        public float RotationSpeed { get; protected set; }

        private Rectangle healthBar;

        public AIAgent(uint id, bool? visible = null, string spriteName = null, Color? colour = null, float? rotation = null, float? layer = null,
            bool? active = null, Vector2? position = null, Vector2? direction = null, Vector2? velocity = null, float? speed = null,
            int? size = null, bool? dead = null, Comparer<(ICollidable, CollisionDetails)> collisionsComparer = null, Vector2? orientation = null,
            float? leakPercentage = null, int? maxVitality = null, int? vitality = null, float? defence = null, float? damage = null,
            float? windupTime = null, float? lingerTime = null, float? cooldownTime = null, float? attackRange = null,
            float? dashTime = null, float? dashSpeedModifier = null, float? invincibilityTime = null,
            AIState initialAIState = AIState.None, PatrolType patrolType = PatrolType.None, bool? patrolDirectionIsForward = null,
            LinkedList<Vector2> patrolPoints = null, int? firstPatrolPointIndex = null, int? aggroRange = null, float? loseAggroTime = null,
            float? rotationSpeed = null)
            : base(id, visible, spriteName, colour, rotation, layer, active, position, direction, velocity, speed, size, dead,
                  collisionsComparer, orientation, leakPercentage, maxVitality, vitality, defence, damage, windupTime, lingerTime, 
                  cooldownTime, attackRange, dashTime, dashSpeedModifier, invincibilityTime)
        {
            this.patrolDirectionIsForward = patrolDirectionIsForward ?? DefaultPatrolDirectionIsForward;
            AggroRange = aggroRange ?? DefaultAggoRange;

            RotationSpeed = rotationSpeed ?? DefaultRotationSpeed;

            // special stuff
            PatrolType = patrolType == PatrolType.None ? initialAIState == AIState.Guard ? PatrolType.None : DefaultPatrolType : patrolType;

            if (patrolPoints != null && patrolPoints.Count > 0)
            {
                PatrolPoints = patrolPoints;

                int fppi = firstPatrolPointIndex ?? DefaultFirstPatrolPointIndex;
                int count = 0;
                CurrentPatrolPoint = patrolPoints.First;
                for (var cur = patrolPoints.First; cur != null; cur = cur.Next)
                {
                    if (count++ == fppi)
                    {
                        CurrentPatrolPoint = cur;
                        break;
                    }
                }

                Position = position ?? CurrentPatrolPoint.Value;
            }
            else
            {
                PatrolPoints = new LinkedList<Vector2>();
                CurrentPatrolPoint = PatrolPoints.AddFirst(Position);
            }

            CurrentState.Push(initialAIState == AIState.None ? (PatrolPoints.Count > 1 ? AIState.Patrol : DefaultInitialAIState) : initialAIState);

            LoseAggroTimer = new Timer(loseAggroTime ?? DefaultLoseAggroTime);

            healthBar = new Rectangle(Collider.BoundingBox.Left, Collider.BoundingBox.Bottom, Size, 5);
        }

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

        //DCircle aggroRange = new DCircle(Vector2.Zero, DefaultAggoRange + DefaultSize / 2f, new Color(Color.Yellow, 0.2f), 0.005f);
        public override void Draw()
        {
            base.Draw();

            // health bar
            float percentVitality = Vitality / (float)MaxVitality;
            Rectangle curHealthBar = healthBar with { X = Collider.BoundingBox.Left, Y = Collider.BoundingBox.Bottom, Width = (int)(healthBar.Width * percentVitality)};
            Globals.Globals.spriteBatch.Draw(Globals.Globals.Pixel, curHealthBar, null, Color.Red, 0f, Vector2.Zero, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, Layer + 0.01f);

            /*aggroRange.MoveTo(Position);
            aggroRange.Draw();*/
            /*Globals.Globals.spriteBatch.DrawString(Globals.Globals.DebugFont, $"{TargetLastSeen}", Position, Color.Black, 0f, Vector2.Zero,
                1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0.99f);*/
            /*Globals.Globals.spriteBatch.DrawString(Globals.Globals.DebugFont, CurrentState.Peek().ToString(), Position, Color.Black, 0f, Vector2.Zero, 
                1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0.99f);*/
            /*Globals.Globals.spriteBatch.DrawString(Globals.Globals.DebugFont, $"Rotation: {Rotation}", Position, Color.White, 0f, Vector2.Zero,
                1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0.5f);*/
            /*Globals.Globals.spriteBatch.DrawString(Globals.Globals.DebugFont, $"Speed: {Speed}", Position, Color.Black, 0f, 
                Vector2.Zero, 2f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0.9f);*/
            /*Globals.Globals.spriteBatch.DrawString(Globals.Globals.DebugFont, $"Attack Timer: {attackTimer.Percentage}", Position,
                Color.Black, 0f, Vector2.Zero, 1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0.9f);*/
        }

        public override void Move()
        {
            if (Strike.Lock)
                return;

            lastPosition = Position;
            TargetPos = Position;

            switch (CurrentState.Peek())
            {
                case AIState.Guard:
                    Guard();
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
            prevFrameTarget = PursuitTarget;

            // should be more sophisticated so it can recognise agents other than the Player as a target, but unnecessary right now
            if ((Game1.Player.Position - this.Position).Length() - (Game1.Player.Size + this.Size) / 2f <= AggroRange &&
                TargetInLineOfSight(Game1.Player.Position))
            {
                PursuitTarget = Game1.Player;
                TargetLastSeen = PursuitTarget.Position;
                CurrentState.Push(AIState.Pursuit);
            }
            else
            {
                PursuitTarget = null;
            }

            if (Strike.Lock)
                return;

            Vector2 newOrientation = Orientation;
            switch (CurrentState.Peek())
            {
                case AIState.Patrol:
                    newOrientation = CurrentPatrolPoint.Value - Position;
                    break;
                case AIState.Pursuit:
                    if (Aggroed && TargetInLineOfSight(TargetLastSeen.Value))
                    {
                        newOrientation = TargetLastSeen.Value - Position;
                    }
                    else if (PursuitPath.Any())
                    {
                        newOrientation = PursuitPath.First.Value - Position;
                    }
                    else if (TargetLastSeen.HasValue)
                    {
                        newOrientation = TargetLastSeen.Value - Position;
                    }
                    break;
                case AIState.Guard:
                    if (!Aggroed)
                    {
                        // TODO
                    }
                    break;
            }

            if (newOrientation != Orientation)
            {
                float unboundRotation = Utility.VectorToAngle(newOrientation);
                Rotation += Utility.Sign(Utility.NormaliseRotation(unboundRotation - Rotation)) * RotationSpeed * (float)Globals.Globals.gameTime.ElapsedGameTime.TotalSeconds;
                Orientation = Utility.AngleToVector(Rotation);
            }
            /*Orientation = newOrientation;

            base.Look();*/
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

        public void Guard()
        {
            if (ArrivedAtPathPoint(CurrentPatrolPoint))
            {
                return;
            }

            if (TargetInLineOfSight(CurrentPatrolPoint.Value))
            {
                MoveTowards(CurrentPatrolPoint.Value);
            }
            else if (returnToGuardPosition.Count > 0)
            {
                if (!ArrivedAtPathPoint(returnToGuardPosition.First))
                {
                    MoveTowards(returnToGuardPosition.First.Value);
                }
                else
                {
                    returnToGuardPosition.RemoveFirst();
                }
            }
            else
            {
                returnToGuardPosition = new LinkedList<Vector2>(Pathfinding.FindPath(Game1.TileMap.Walls, this.Position, CurrentPatrolPoint.Value, Game1.TileMap.TileSize));
            }
        }

        public void Patrol()
        {
            if (!returnToPatrolPath.Any())
            {
                if (ArrivedAtPathPoint(CurrentPatrolPoint))
                {
                    switch (PatrolType)
                    {
                        default:
                        case PatrolType.None:
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
                else if (TargetInLineOfSight(CurrentPatrolPoint.Value))
                {
                    MoveTowards(CurrentPatrolPoint.Value);
                }
                else
                {
                    Debug.WriteLine("here");
                    returnToPatrolPath = new LinkedList<Vector2>(Pathfinding.FindPath(Game1.TileMap.Walls, Position, CurrentPatrolPoint.Value, Game1.TileMap.TileSize));
                }
            }
            else
            {
                MoveTowards(returnToPatrolPath.First());

                if (ArrivedAtPathPoint(returnToPatrolPath.First))
                {
                    returnToPatrolPath.RemoveFirst();
                }
            }
        }

        public void Pursue()
        {
            if (Aggroed)    // if the Agent can see the target
            {
                if ((TargetLastSeen.Value - this.Position).Length() - PursuitTarget.Size / 2f <= AttackRange)       // if the Agent is within Hovering distance
                {
                    HoverOrAttack();
                }
                else if (Hovering)      // if the Agent was hovering last frame but is no longer within Hovering distance
                {
                    Speed = BaseSpeed;
                    Hovering = false;

                    MoveTowards(TargetLastSeen.Value);
                }
                else                    // if the Agent is outside of Hovering distance (so it can close the gap)
                {
                    MoveTowards(PursuitTarget.Position);
                }
            }
            else if (prevFrameTarget != null)       // if the Agent cannot see the target, but could last frame
            {
                PursuitPath = new LinkedList<Vector2>(Pathfinding.FindPath(Game1.TileMap.Walls, Position, TargetLastSeen.Value, Game1.TileMap.TileSize));
                if (PursuitPath.Any())
                {
                    MoveTowards(PursuitPath.First());
                }
            }
            else            // if the Agent cannot see the target, and couldn't before either
            {
                if (PursuitPath.Any())      // if the Agent is still "hot on the trail"
                {
                    MoveTowards(PursuitPath.First());

                    if (ArrivedAtPathPoint(PursuitPath.First))
                    {
                        PursuitPath.RemoveFirst();
                    }
                }
                else            // if the Agent has "lost the scent"
                {
                    CurrentState.Pop();
                }
            }

            Speed = Hovering ? BaseSpeed / 2f : BaseSpeed;
        }

        private float? prevDistanceFromPursuitTarget = null;
        protected void HoverOrAttack()
        {
            // should only be called inside Pursuit() if the target is in the line of sight and within the AttackRange
            if (Strike.Lock)
                return;

            float distanceFromPursuitTarget = (TargetLastSeen.Value - this.Position).Length() - PursuitTarget.Size / 2f;

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
                        MoveTowards(TargetLastSeen.Value);
                    }
                    else
                    {
                        // if the distance between this and the PursuitTarget is less than half the AttackRange, move away from the PursuitTarget
                        var directionModifier = distanceFromPursuitTarget < AttackRange / 2f ? -1 : 1;
                        Direction = directionModifier * Vector2.Normalize(TargetLastSeen.Value - this.Position);
                        base.Move();
                    }
                }
            }
            else
            {
                MoveTowards(TargetLastSeen.Value);
            }

            prevDistanceFromPursuitTarget = distanceFromPursuitTarget;
        }

        public bool TargetInLineOfSight(Vector2 target)
        {
            return Position == target || !Collision.CastRay(new Ray2D(Position, target), Game1.SimulatedTiles.Cast<ICollidable>().ToList());
        }

        protected bool ArrivedAtPathPoint(LinkedListNode<Vector2> nodePoint)
        {
            // if this comes into 5px of the CurrentPatrolPoint
            return (Position - nodePoint.Value).LengthSquared() < 25f;      // 5px^2 = 25
        }

        protected void MoveTowards(Vector2 target)
        {
            Direction = Vector2.Normalize(target - Position);

            base.Move();
        }
    }
}
