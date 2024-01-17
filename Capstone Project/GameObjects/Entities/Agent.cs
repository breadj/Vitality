using static Capstone_Project.Globals.Globals;
using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects.Entities
{
    public class Agent : Mob, IHurtable, IAttacker, IDasher
    {
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

        public bool PerformingAction => Attack.Active || Dash.Active;

        public float Damage { get; protected set; }
        public Attack Attack { get; protected set; }
        public float Range { get; protected set; } = 100f;

        public Dash Dash { get; protected set; }
        public float BaseSpeed { get; protected set; }
        public float DashTime { get; protected set; }
        public float DashSpeedModifier { get; protected set; }

        public bool Invincible => !Invincibility.Done;
        public Timer Invincibility { get; protected set; }

        public Agent(Subsprite subsprite, Vector2 position, int vitality, float damage, float attackRange = 100f, float defence = 0, int size = 0, float speed = 1)
            : base(subsprite, position, size, speed)
        {
            Vitality = vitality;
            Defence = defence;

            Damage = damage;
            Attack = new Attack(this);
            Range = attackRange;

            BaseSpeed = speed;
            DashTime = 0.5f;
            DashSpeedModifier = 1.6f;
            Dash = new Dash(this, DashSpeedModifier, DashTime);

            Invincibility = new Timer(0f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Active)
                return;

            Invincibility.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();

            spriteBatch.DrawString(DebugFont, Vitality.ToString(), new Vector2(Collider.BoundingBox.Left, Collider.BoundingBox.Bottom), Color.Black, 
                0f, Vector2.Zero, 1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1f);
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
            Attack.Start(Position, Orientation, Range);
        }
    }
}
