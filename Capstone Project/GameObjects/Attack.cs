<<<<<<< HEAD
﻿using static Capstone_Project.Globals.Globals;
using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.SpriteTextures;
=======
﻿using Capstone_Project.GameObjects.Interfaces;
>>>>>>> bc39f8d78e4142e23321cca44295f357bb9c4054
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Project.GameObjects
{
<<<<<<< HEAD
    public class Attack : Interfaces.ITexturable, ICollidable, IUpdatable
    {
        public bool Visible { get; private set; }
        public Subsprite Subsprite { get; init; }
        public Rectangle Destination => new Rectangle(Position.ToPoint(), new Point((int)attacker.Range));
        public float Rotation => 0f;
        public Vector2 Origin { get; init; }
        public float Layer => 0.09f;

        public bool Active { get; set; } = false;
        public Rectangle Hitbox => new Rectangle(Position.ToPoint() - new Point((int)(attacker.Range / 2f)), new Point((int)attacker.Range));
=======
    public class Attack : ICollidable, IUpdatable
    {
        public bool Active { get; set; } = false;
        public Rectangle Hitbox { get; set; }
>>>>>>> bc39f8d78e4142e23321cca44295f357bb9c4054
        public bool IsCircle { get; } = false;  // will probably never use this
        public float Radius { get; init; }      // nor this

        // all CDs in seconds
<<<<<<< HEAD
        public Timer Windup { get; private set; }            // how long until the attack actually happens (how long to wind up the attacK)
        public Timer Linger { get; private set; }            // how long the attack hurtbox stays there 
        public Timer Cooldown { get; private set; }          // how long until attacker can attack again
        public float Speed => Windup.WaitTime + Linger.WaitTime + Cooldown.WaitTime;
        public bool Lock => Windup.Active || Linger.Active;

        public IAttacker attacker { get; private set; }
        public Vector2 Position { get; private set; }

        public Attack(IAttacker attacker, Vector2 position, float cooldownTime = 0, float windupTime = 0, float lingerTime = 0)
        {
            this.attacker = attacker;
            Position = position;

            Subsprite = new Subsprite(BLANK, BLANK.Bounds);
            Origin = Subsprite.Source.Size.ToVector2() / 2f;

            Windup = new Timer(windupTime);
            Linger = new Timer(lingerTime);
            Cooldown = new Timer(cooldownTime);
        }

        private bool prevActive = false;
        public void Update(GameTime gameTime)
        {
            if (!Active)
                return;

=======
        public Cooldown Windup { get; private set; }            // how long until the attack actually happens (how long to wind up the attacK)
        public Cooldown Linger { get; private set; }            // how long the attack hurtbox stays there 
        public Cooldown Cooldown { get; private set; }          // how long until attacker can attack again


        public Attack(Rectangle swingbox, float windupTime, float lingerTime, float cooldown)
        {
            Hitbox = swingbox;
            Radius = swingbox.Size.X;

            Windup = new Cooldown(windupTime);
            Linger = new Cooldown(lingerTime);
            Cooldown = new Cooldown(cooldown);
        }

        public void Update(GameTime gameTime)
        {
>>>>>>> bc39f8d78e4142e23321cca44295f357bb9c4054
            Windup.Update(gameTime);
            Linger.Update(gameTime);
            Cooldown.Update(gameTime);

<<<<<<< HEAD
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
            if (Visible)
            {
                if (Windup.Active)
                    spriteBatch.Draw(Subsprite.SpriteSheet, Destination, null, new Color(Color.Red, 0.3f), Rotation, Origin, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, Layer);
                else if (Linger.Active)
                    spriteBatch.Draw(Subsprite.SpriteSheet, Destination, null, Color.Red, Rotation, Origin, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, Layer);
            }
        }

        public void Start(Vector2 centre)
        {
            Active = true;
            if (!prevActive)
            {
                Position = centre;
                Windup.Start();
                Visible = true;
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

        public CollisionDetails CollidesWith(ICollidable other)
        {
            CollisionDetails cd = new CollisionDetails();
            if (!Active)
                return cd;

            cd.Intersection = Rectangle.Intersect(Hitbox, other.Hitbox);

            if (!cd.Intersection.IsEmpty)
            {
                cd.Type = CollisionType.RectOnRect;
            }

            return cd;
=======

        }

        public CollisionDetails CollidesWith(ICollidable other)
        {
            throw new NotImplementedException();
>>>>>>> bc39f8d78e4142e23321cca44295f357bb9c4054
        }
    }
}
