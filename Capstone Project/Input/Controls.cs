using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Capstone_Project.Input
{
    public enum Button { None, Left, Right, Middle, Mouse4, Mouse5 };

    public class Controls
    {
        private InputState InputState { get; set; }
        private Action[] Actions { get; set; }

        public Point MousePos => InputState.MouseState.Position;    // remember that this is screen position, not world position
        public List<Action> ActivatedActions { get; private set; }
        public Action Exit { get; private set; }

        public Controls()
        {
            InputState = new InputState();
            ActivatedActions = new List<Action>();
            Exit = new Action("Exit", new List<Input> { Keys.Escape }, ActionType.OnPress);

            // movement Actions and Keys
            Action Up = new Action("Up", new List<Input> { Keys.W, Keys.Up }, ActionType.WhenDown);
            Action Down = new Action("Down", new List<Input> { Keys.S, Keys.Down }, ActionType.WhenDown);
            Action Left = new Action("Left", new List<Input> { Keys.A, Keys.Left }, ActionType.WhenDown);
            Action Right = new Action("Right", new List<Input> { Keys.D, Keys.Right }, ActionType.WhenDown);

            // other Actions
            Action Attack = new Action("Attack", new List<Input> { Button.Left }, ActionType.OnRelease);        // TODO: change this for mouse input
            // TODO: add interaction, attack, etc

            Actions = new Action[] { Exit, Up, Down, Left, Right, Attack };
        }

        public void Update(GameTime gameTime)
        {
            InputState prevState = InputState;
            InputState = new InputState();
            // not too sure why I had to get rid of the Update() function and replace it with a new instantiation, but it works now at least

            // Adds all Action objects that are activated to the ActivatedActions list
            ActivatedActions = Actions.Where(action => action.Activated((float)gameTime.ElapsedGameTime.TotalSeconds, prevState, InputState)).ToList();
        }
    }
}
