using static Capstone_Project.Globals.Globals;
using Capstone_Project.Globals;
using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects.Entities
{
    public class Agent : Mob, IAgent, IHurtable, IAttacker, IDasher
    {
        #region Default Attributes
        public static string DefaultSpriteName { get; } = "Enemy";
        public static Subsprite DefaultSprite { get; } = DefaultSprites[DefaultSpriteName];
        public static Vector2 DefaultPosition { get; } = Vector2.Zero;
        public static int DefaultVitality { get; } = 50;
        public static float DefaultDamage { get; } = 10f;
        public static float DefaultAttackRange { get; } = 100f;
        public static float DefaultDefence { get; } = 5f;
        public static int DefaultSize { get; } = 100;
        public static float DefaultSpeed { get; } = 150f;
        #endregion Default Attributes


        public int Vitality
        {
            get => vit;
            protected set => vit = value < 0 ? 0 : value;
        }                   // Can only be positive (or 0)
        private int vit = 1;
        public float Defence
        { 
            get => def; 
            protected set => def = value > 0 ? value <= 100 ? value : 100 : 0; 
        }                   // Can only be between 0-100
        private float def = 0;

        public bool PerformingAction => Strike.Active || Dash.Active;

        public float Damage { get; protected set; }
        public Attack Strike { get; protected set; }
        public float Range { get; protected set; } = 100f;

        public Dash Dash { get; protected set; }
        public float BaseSpeed { get; protected set; }
        public float DashTime { get; protected set; }
        public float DashSpeedModifier { get; protected set; }

        public bool Invincible => !Invincibility.Done;
        public Timer Invincibility { get; protected set; }

        public Agent(string spriteName, Subsprite subsprite, Vector2 position, int vitality, float damage, float attackRange = 100f, float defence = 0, int size = 0, float speed = 1)
            : base(spriteName, subsprite, position, size, speed)
        {
            Vitality = vitality;
            Defence = defence;

            Damage = damage;
            Strike = new Attack(this);
            Range = attackRange;

            BaseSpeed = speed;
            DashTime = 0.5f;
            DashSpeedModifier = 1.6f;
            Dash = new Dash(this, DashSpeedModifier, DashTime);

            Invincibility = new Timer(0f);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            Move();
            Look();

            Invincibility.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();

            spriteBatch.DrawString(DebugFont, Vitality.ToString(), new Vector2(Collider.BoundingBox.Left, Collider.BoundingBox.Bottom), Color.Black, 
                0f, Vector2.Zero, 1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1f);
            // shows Position
            spriteBatch.DrawString(DebugFont, Position.ToString(), Position, Color.Black,
                0f, Vector2.Zero, 1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1f);
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

        public void TakeDamage(float damage, float invincibilityTime = 0f)
        {
            if (Invincible)
                return;

            damage *= (100f - def) / 100f;      // def is just the % damage reduction

            Vitality -= (int)damage;
            if (Vitality == 0)
                Kill();

            Invincibility.SetNewWaitTime(invincibilityTime);
            Invincibility.Start();
        }

        public void Swing()
        {
            Strike.Start(Position, Orientation, Range);
        }
    }
}
