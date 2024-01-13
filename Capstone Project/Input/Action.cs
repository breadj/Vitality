
using System.Collections.Generic;

namespace Capstone_Project.Input
{
    public enum ActionType { WhenDown, WhenUp, OnHold, OnPress, OnRelease }

    public class Action
    {
        public readonly string Name;
        public List<Input> Inputs { get; set; }
        private readonly ActionType actionType;
        private readonly float holdTime;       // if it is of ActionType OnHold, how long it needs to be held before performing the action

        public Action(string action, List<Input> inputs, ActionType actionType, float holdTime = 0)
        {
            Name = action;
            Inputs = inputs;
            this.actionType = actionType;
            this.holdTime = holdTime;
        }

        private float holdTimer = 0;
        public bool Activated(float deltaTime, InputState prevState, InputState curState)
        {
            foreach (Input input in Inputs)
            {
                switch (actionType)
                {
                case ActionType.WhenDown:
                    if (curState.IsDown(input))
                        return true;
                    break;
                case ActionType.WhenUp:
                    if (curState.IsUp(input))
                        return true;
                    break;
                case ActionType.OnHold:
                    if (prevState.IsUp(input) && curState.IsDown(input))      // if the player has only just started holding, start the timer from 0
                        holdTimer = 0;
                    if (curState.IsDown(input))
                        holdTimer += deltaTime;
                    if (holdTimer >= holdTime)
                        return true;
                    break;
                case ActionType.OnPress:
                    if (curState.IsDown(input) && prevState.IsUp(input))
                        return true;
                    break;
                case ActionType.OnRelease:
                    if (curState.IsUp(input) && prevState.IsDown(input))
                        return true;
                    break;
                }
            }
            return false;
        }
    }
}
