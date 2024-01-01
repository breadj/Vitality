using Capstone_Project.GameObjects.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Project.GameObjects
{
    public class Attack : ICollidable, IUpdatable
    {
        public bool Active { get; set; } = false;
        public Rectangle Hitbox { get; set; }
        public bool IsCircle { get; } = false;  // will probably never use this
        public float Radius { get; init; }      // nor this

        // all CDs in seconds
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
            Windup.Update(gameTime);
            Linger.Update(gameTime);
            Cooldown.Update(gameTime);


        }

        public CollisionDetails CollidesWith(ICollidable other)
        {
            throw new NotImplementedException();
        }
    }
}
