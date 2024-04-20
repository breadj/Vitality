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
        public void Resume() => Active = true;
        public void Pause() => Active = false;
        public float TimeRemaining => WaitTime - timer;
        public float Percentage => timer / WaitTime;
        public bool Done => timer >= WaitTime;

        /// <summary>
        /// Creates a new Timer object that needs to be updated every Update(GameTime) call
        /// </summary>
        /// <param name="waitTime">How long (in seconds) until the Cooldown returns .Done as true</param>
        /// <param name="initialTime">Optional initial value for how long has already been waited</param>
        /// <param name="startImmediately">Optional value to set the timer to start counting as soon as initialised</param>
        public Timer(float waitTime, float initialTime = 0, bool startImmediately = false)
        {
            WaitTime = waitTime;
            timer = initialTime;
            Active = startImmediately;
        }

        public void Update(GameTime gameTime)
        {
            if (Done)
                Pause();

            if (Active)
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Start()
        {
            Reset();
            Resume();
        }

        // acts like "Stop" (square button) on a CD player
        public void Reset()
        {
            timer = 0;
            Pause();
        }
    }
}
