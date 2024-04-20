using static Capstone_Project.Globals.Globals;
using Capstone_Project.Globals;
using Capstone_Project.GameObjects.Interfaces;
using Microsoft.Xna.Framework;
using Capstone_Project.CollisionStuff;
using System.Collections.Generic;

namespace Capstone_Project.GameObjects.Entities
{
    public class Agent : Mob, IAgent, IHurtable, IAttacker, IDasher, IVitalised
    {
        #region Default Attributes
        public static readonly new string DefaultSpriteName = "Enemy";

        public static readonly float DefaultLeakPercentage = 0.7f;
        public static readonly int DefaultMaxVitality = DefaultVitality;
        public static readonly int DefaultVitality = 50;
        public static readonly float DefaultDefence = 5f;

        public static readonly float DefaultDamage = 10f;
        public static readonly float DefaultWindupTime = 2f;
        public static readonly float DefaultLingerTime = 0.5f;
        public static readonly float DefaultCooldownTime = 1f;
        public static readonly float DefaultAttackRange = 100f;

        public static readonly float DefaultDashTime = 0.5f;
        public static readonly float DefaultDashSpeedModifier = 1.6f;

        public static readonly float DefaultInvincibilityTime = 0f;
        #endregion Default Attributes


        public float LeakPercentage { get; init; }
        public int MaxVitality { get; protected set; }
        public int Vitality
        {
            get => vitality;
            protected set => vitality = value < 0 ? 0 : value;
        }                   // Can only be positive (or 0)
        private int vitality = 1;
        public float Defence
        { 
            get => defence; 
            protected set => defence = value > 0 ? value <= 100 ? value : 100 : 0; 
        }                   // Can only be between 0-100
        private float defence = 0;

        public bool PerformingAction => Strike.Active || Dash.Active;

        public float Damage { get; protected set; }
        public Attack Strike { get; protected set; }
        public float AttackRange { get; protected set; } = 100f;

        public Dash Dash { get; protected set; }
        public float BaseSpeed { get; protected set; }
        public float DashTime { get; protected set; }
        public float DashSpeedModifier { get; protected set; }

        public bool Invincible => !Invincibility.Done;
        public Timer Invincibility { get; protected set; }

        public Agent(uint id, bool? visible = null, string spriteName = null, Color? colour = null, float? rotation = null, float? layer = null,
            bool? active = null, Vector2? position = null, Vector2? direction = null, Vector2? velocity = null, float? speed = null,
            int? size = null, bool? dead = null, Comparer<(ICollidable, CollisionDetails)> collisionsComparer = null,
            Vector2? orientation = null,
            float? leakPercentage = null, int? maxVitality = null, int? vitality = null, float? defence = null, float? damage = null, 
            float? windupTime = null, float? lingerTime = null, float? cooldownTime = null, float? attackRange = null, float? dashTime = null, float? dashSpeedModifier = null, 
            float? invincibilityTime = null)
            : base(id, visible, spriteName ?? DefaultSpriteName, colour, rotation, layer, active, position, direction, velocity, speed, size,
                  dead, collisionsComparer, orientation)
        {
            LeakPercentage = leakPercentage ?? DefaultLeakPercentage;
            MaxVitality = maxVitality ?? DefaultMaxVitality;
            Vitality = vitality ?? DefaultVitality;
            Defence = defence ?? DefaultDefence;

            Damage = damage ?? DefaultDamage;
            Strike = new Attack(this, windupTime ?? DefaultWindupTime, lingerTime ?? DefaultLingerTime, cooldownTime ?? DefaultCooldownTime);
            AttackRange = attackRange ?? DefaultAttackRange;

            DashTime = dashTime ?? DefaultDashTime;
            DashSpeedModifier = dashSpeedModifier ?? DefaultDashSpeedModifier;

            // special stuff
            if (MaxVitality < Vitality)
            {
                MaxVitality = Vitality;
            }

            BaseSpeed = Speed;
            Dash = new Dash(this, DashSpeedModifier, DashTime);

            Invincibility = new Timer(invincibilityTime ?? DefaultInvincibilityTime);
        }

        /*public Agent(string spriteName, Subsprite subsprite, Vector2 position, int vitality, float damage, float attackRange = 100f, float defence = 0, int size = 0, float speed = 1)
            : base(spriteName, subsprite, position, size, speed)
        {
            Vitality = vitality;
            Defence = defence;

            Damage = damage;
            Strike = new Attack(this);
            AttackRange = attackRange;

            BaseSpeed = speed;
            DashTime = 0.5f;
            DashSpeedModifier = 1.6f;
            Dash = new Dash(this, DashSpeedModifier, DashTime);

            Invincibility = new Timer(0f);
        }*/

        public override void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            Move();
            Look();
            //Attack();

            Invincibility.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();

            Strike.Draw();

            spriteBatch.DrawString(DebugFont, Vitality.ToString(), new Vector2(Collider.BoundingBox.Left, Collider.BoundingBox.Bottom), Color.Black, 
                0f, Vector2.Zero, 1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1f);
            // shows Position
            /*spriteBatch.DrawString(DebugFont, Position.ToString(), Position, Color.Black,
                0f, Vector2.Zero, 1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1f);*/
        }

        public virtual void Move()
        {
            lastPosition = Position;
            TargetPos = Position;

            Velocity = Direction * Speed;
            actualVelocity = Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            TargetPos += actualVelocity;

            Collider.MoveTo(TargetPos);
        }

        public virtual void Look()
        {
            Rotation = Utility.VectorToAngle(Orientation);
        }

        public virtual void Attack()
        {
            Swing();
        }

        public void TakeDamage(float damage, IAttacker attacker, float invincibilityTime = 0f)
        {
            if (Invincible)
                return;

            damage *= (100f - defence) / 100f;      // def is just the % damage reduction

            Vitality -= (int)damage;
            if (Vitality == 0)
            {
                Killer = attacker;
                Kill();
            }

            Invincibility.SetNewWaitTime(invincibilityTime);
            Invincibility.Start();
        }

        public void Swing()
        {
            Strike.Start(Position, Orientation, AttackRange);
        }

        public void AbsorbVitality(IVitalised vitalised)
        {
            int leakAmount = vitalised.LeakVitality();

            MaxVitality += leakAmount;
            Vitality += leakAmount;
        }

        public int LeakVitality()
        {
            return (int)(MaxVitality * LeakPercentage);
        }

        public int LetVitality()
        {
            return MaxVitality;
        }

        public override void Kill()
        {
            if (Killer is IVitalised v)
            {
                v.AbsorbVitality(this);
            }

            Dead = true;
        }
    }
}
