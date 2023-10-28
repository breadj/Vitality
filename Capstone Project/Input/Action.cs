using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Capstone_Project.Input
{
    public enum ActionType { OnPress, OnRelease, OnHold, OnFirstPress, OnFirstRelease }

    public class Action
    {
        public readonly string Name;
        public Keys[] Keys { get; set; }
        private readonly ActionType actionType;
        private readonly float holdTime;       // if it is of ActionType OnHold, how long it needs to be held before performing the action

        public Action(string action, Keys[] keys, ActionType actionType, float holdTime = 0)
        {
            Name = action;
            Keys = keys;
            this.actionType = actionType;
            this.holdTime = holdTime;
        }

        private float holdTimer = 0;
        public bool Activated(GameTime gameTime, KeyboardState prevState, KeyboardState curState)
        {
            for (int i = 0; i < Keys.Length; i++)
            {
                switch (actionType)
                {
                    case ActionType.OnPress:
                        if (curState.IsKeyDown(Keys[i]))
                            return true;
                        break;
                     case ActionType.OnRelease:
                         if (curState.IsKeyUp(Keys[i]))
                            return true;
                        break;
                     case ActionType.OnHold:
                        if (prevState.IsKeyUp(Keys[i]) && curState.IsKeyDown(Keys[i]))      // if the player has only just started holding, start the timer from 0
                            holdTimer = 0;
                        if (curState.IsKeyDown(Keys[i]))
                            holdTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (holdTimer >= holdTime)
                            return true;
                        break;
                    case ActionType.OnFirstPress:
                        if (curState.IsKeyDown(Keys[i]) && prevState.IsKeyUp(Keys[i]))
                            return true;
                        break;
                    case ActionType.OnFirstRelease:
                        if (curState.IsKeyUp(Keys[i]) && prevState.IsKeyDown(Keys[i]))
                            return true;
                        break;
                }
            }
            return false;
        }
    }
}
