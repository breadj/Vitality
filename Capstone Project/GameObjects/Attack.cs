using static Capstone_Project.Globals.Globals;
using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using Capstone_Project.CollisionStuff;
using Capstone_Project.CollisionStuff.CollisionShapes;
using Capstone_Project.Globals;
using Capstone_Project.Fundamentals.DrawableShapes;

namespace Capstone_Project.GameObjects
{
    public class Attack : Interfaces.IDrawable, ICollidable, IUpdatable
    {
        public bool Visible { get; private set; }
        public float Layer => 0.09f;
        public DPolygon Polygon { get; private set; } = null;

        public bool Active { get; set; } = false;
        public CShape Collider { get; private set; } = null;

        // all CDs in seconds
        public Timer Windup { get; private set; }            // how long until the attack actually happens (how long to wind up the attacK)
        public Timer Linger { get; private set; }            // how long the attack hurtbox stays there 
        public Timer Cooldown { get; private set; }          // how long until attacker can attack again
        public float Speed => Windup.WaitTime + Linger.WaitTime + Cooldown.WaitTime;
        public bool Lock => Windup.Active || Linger.Active;
        public bool Attacking => Linger.Active;

        public IAttacker Attacker { get; init; }
        public Vector2 Position { get; private set; }

        public Attack(IAttacker attacker, Vector2 position, float cooldownTime = 0, float windupTime = 0, float lingerTime = 0)
        {
            Attacker = attacker;
            Position = position;

            Windup = new Timer(windupTime);
            Linger = new Timer(lingerTime);
            Cooldown = new Timer(cooldownTime);
        }

        private bool prevActive = false;
        public void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            Windup.Update(gameTime);
            Linger.Update(gameTime);
            Cooldown.Update(gameTime);

            if (Windup.Active && Windup.Done)
            {
                Windup.Active = false;
                Windup.Reset();

                Linger.Start();
            }

            if (Linger.Active && Linger.Done)
            {
                Linger.Active = false;
                Linger.Reset();

                Cooldown.Start();
                Visible = false;
            }

            if (Cooldown.Active && Cooldown.Done)
            {
                Cooldown.Active = false;
                Cooldown.Reset();

                Active = false;
            }

            prevActive = Active;
        }

        public void Draw()
        {
            if (!Visible)
                return;
            if (Windup.Active)
            {
                float doubleWindup = Windup.Percentage * 2f;
                Polygon.Draw(new Color(Polygon.Colour, doubleWindup <= 1f ? 0.7f * doubleWindup : 0.7f));
            }
            else if (Linger.Active)
                Polygon.Draw();
        }

        public void Start(Vector2 centre, Vector2 direction)
        {
            Active = true;
            if (!prevActive)
            {
                Position = centre;
                Windup.Start();
                Visible = true;

                Collider = new CPolygon(Position, DPolygon.Rotate(DPolygon.GenerateNarrowArc(100), Utility.VectorToAngle(direction)));
                Polygon = new DPolygon(Collider as CPolygon, Color.Red, Layer, true);
            }
        }

        #region Change Cooldown CDs
        public void ChangeWindupCD(float time)
        {
            Windup.SetNewWaitTime(time);
        }

        public void ChangeLingerCD(float time)
        {
            Linger.SetNewWaitTime(time);
        }

        public void ChangeCooldownCD(float time)
        {
            Cooldown.SetNewWaitTime(time);
        }
        #endregion

        public bool CollidesWith(ICollidable other, out CollisionDetails cd)
        {
            cd = new CollisionDetails();
            if (!Active || !other.Active)
                return false;

            return Collision.Colliding(this.Collider, other.Collider, out cd);
        }

        public static void Swing(IAttacker attacker, IHurtable hurtable)
        {
            if (attacker == null || hurtable == null)
                return;

            if (attacker.Attack.Attacking && Collision.Colliding(attacker.Attack.Collider, hurtable.Collider))
                hurtable.TakeDamage(attacker.Damage);
        }
    }
}
