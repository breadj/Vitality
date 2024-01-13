using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects.Entities
{
    public class Agent : Mob, IHurtable, IAttacker
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

        public float Damage { get; protected set; }
        public Attack Attack { get; protected set; }
        public float Range { get; protected set; } = 100f;

        public Agent(Subsprite subsprite, Vector2 position, int vitality, float damage, float attackRange = 100f, float defence = 0, int size = 0, int speed = 1) : base(subsprite, position, size, speed)
        {
            Vitality = vitality;
            Defence = defence;

            Damage = damage;
            Attack = new Attack(this);
            Range = attackRange;
        }

        public void TakeDamage(float damage)
        {
            Colour = Color.Yellow;
            damage *= (100f - def) / 100f;      // Def is just the % damage reduction

            Vitality -= (int)damage;
            if (Vitality == 0)
                Kill();
        }

        public void Swing()
        {
            Attack.Start(Position, Orientation, Range);
        }
    }
}
