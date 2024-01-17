﻿using static Capstone_Project.Globals.Globals;
using Capstone_Project.GameObjects.Interfaces;
using Microsoft.Xna.Framework;
using Capstone_Project.CollisionStuff;
using Capstone_Project.CollisionStuff.CollisionShapes;
using Capstone_Project.Globals;
using Capstone_Project.Fundamentals.DrawableShapes;
using Capstone_Project.GameObjects.Entities;

namespace Capstone_Project.GameObjects
{
    public class Attack : Interfaces.IDrawable, ICollidable, IUpdatable
    {
        public bool Visible => Windup.Active || Linger.Active;
        public float Layer => 0.009f;
        public DPolygon Polygon { get; private set; } = null;

        public bool Active => Windup.Active || Linger.Active;
        public bool OnCD => Cooldown.Active;
        public CShape Collider { get; private set; } = null;

        // all CDs in seconds
        public Timer Windup { get; private set; }            // how long until the attack actually happens (how long to wind up the attacK)
        public Timer Linger { get; private set; }            // how long the attack hurtbox stays there 
        public Timer Cooldown { get; private set; }          // how long until attacker can attack again
        public float Speed => Windup.WaitTime + Linger.WaitTime + Cooldown.WaitTime;
        public bool Lock => (Windup.Active && Windup.Percentage >= 0.5f) || Linger.Active;
        public bool Attacking => Linger.Active;

        public IAttacker Attacker { get; init; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; } = 0f;

        public Attack(IAttacker attacker, float cooldownTime = 0, float windupTime = 0, float lingerTime = 0)
        {
            Attacker = attacker;

            Windup = new Timer(windupTime);
            Linger = new Timer(lingerTime);
            Cooldown = new Timer(cooldownTime);
        }

        public void Update(GameTime gameTime)
        {
            if (Windup.Active)
            {
                Windup.Update(gameTime);

                if (Windup.Percentage <= 0.5f && Attacker is Agent a)
                {
                    Polygon.MoveTo(a.Position, a.Rotation - Rotation);
                    Rotation = a.Rotation;
                }
            }
            if (Windup.Done)
            {
                Windup.Reset();

                Linger.Start();
                Collider = new CPolygon(Polygon);
            }

            if (Linger.Active)
                Linger.Update(gameTime);
            if (Linger.Done)
            {
                Linger.Reset();

                Cooldown.Start();
            }

            if (Cooldown.Active)
                Cooldown.Update(gameTime);
            if (Cooldown.Done)
                Cooldown.Reset();
        }

        public void Draw()
        {
            if (!Visible)
                return;

            if (Windup.Active)
                Polygon.Draw(new Color(Polygon.Colour, Windup.Percentage <= 0.5f ? 0.5f * Windup.Percentage * 2f : 0.5f));
            else if (Linger.Active)
                Polygon.Draw();
        }

        public void Start(Vector2 centre, Vector2 direction, float range)
        {
            if (!Active && !Cooldown.Active)
            {
                Position = centre;
                Rotation = Utility.VectorToAngle(direction);
                Windup.Start();

                Polygon = new DPolygon(Position, DPolygon.Rotate(DPolygon.GenerateNarrowArc(range), Rotation), Color.Red, Layer, true);
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

        public void ChangeCDs(float windup = 0, float linger = 0, float cooldown = 0)
        {
            ChangeWindupCD(windup);
            ChangeLingerCD(linger);
            ChangeCooldownCD(cooldown);
        }
        #endregion

        public bool CollidesWith(ICollidable other, out CollisionDetails cd)
        {
            cd = new CollisionDetails();
            if (!Active || !other.Active)
                return false;

            return Collision.Colliding(this.Collider, other.Collider, out cd);
        }

        public static void CheckSwing(IAttacker attacker, IHurtable hurtable)
        {
            if (attacker == null || hurtable == null)
                return;

            if (attacker.Attack.Attacking && Collision.Colliding(attacker.Attack.Collider, hurtable.Collider))
                hurtable.TakeDamage(attacker.Damage, attacker.Attack.Linger.TimeRemaining);
        }
    }
}
