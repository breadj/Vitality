
using Capstone_Project.GameObjects.Entities;
using Capstone_Project.GameObjects.Interfaces;
using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects
{
    public class Dash : IUpdatable
    {
        public bool Active => Timer.Active;

        public float SpeedModifier { get; set; }
        public Timer Timer { get; init; }

        // both for snapshotting at Start()
        public float Speed { get; private set; }
        public Vector2 Direction { get; private set; }

        private Mob dasher;

        public Dash(Mob dasher, float speedModifier, float dashTime)
        {
            this.dasher = dasher;
            SpeedModifier = speedModifier;
            Timer = new Timer(dashTime);
        }

        public void Start()
        {
            Direction = dasher.Direction;
            Speed = dasher.Speed * SpeedModifier;

            Timer.Start();
        }

        public void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            Timer.Update(gameTime);
        }
    }
}
