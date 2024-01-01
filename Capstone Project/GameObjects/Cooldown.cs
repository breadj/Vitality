using Capstone_Project.GameObjects.Interfaces;
using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects
{
    public class Cooldown : IUpdatable
    {
        public bool Active { get; set; } = false;
        public float WaitTime { get; private set; }
        private float timer { get; set; } = 0;

        public void SetNewWaitTime(float waitTime) => WaitTime = waitTime;
        public void Reset() => timer = 0;
        public bool Done => timer >= WaitTime;

        /// <summary>
        /// Creates a new Cooldown object that needs to be updated every Update(GameTime) call
        /// </summary>
        /// <param name="waitTime">How long (in seconds) until the Cooldown returns .Done as true</param>
        /// <param name="initialTime">Optional initial value for how long has already been waited</param>
        public Cooldown(float waitTime, float initialTime = 0) 
        {
            Active = true;
            WaitTime = waitTime;
            timer = initialTime;
        }

        public void Update(GameTime gameTime)
        {
            if (Active)
                timer += timer >= WaitTime ? 0 : (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public static implicit operator bool(Cooldown c) => c.Done;
    }
}
