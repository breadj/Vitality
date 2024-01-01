using Capstone_Project.GameObjects.Interfaces;
using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects
{
    public class Timer : IUpdatable
    {
        public bool Active { get; set; } = false;
        public float WaitTime { get; private set; }
        private float timer { get; set; } = 0;

        public void SetNewWaitTime(float waitTime) => WaitTime = waitTime;
        public void Reset() => timer = 0;
        public float Percentage => timer / WaitTime;
        public bool Done => timer >= WaitTime;

        /// <summary>
        /// Creates a new Timer object that needs to be updated every Update(GameTime) call
        /// </summary>
        /// <param name="waitTime">How long (in seconds) until the Cooldown returns .Done as true</param>
        /// <param name="initialTime">Optional initial value for how long has already been waited</param>
        public Timer(float waitTime, float initialTime = 0) 
        {
            Active = false;
            WaitTime = waitTime;
            timer = initialTime;
        }

        public void Update(GameTime gameTime)
        {
            if (Active)
                timer += timer >= WaitTime ? 0 : (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Start()
        {
            Reset();
            Active = true;
        }

        public static implicit operator bool(Timer c) => c.Done;
    }
}
