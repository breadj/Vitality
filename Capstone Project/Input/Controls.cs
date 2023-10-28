using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Capstone_Project.Input
{
    public class Controls
    {
        private KeyboardState KeyboardState { get; set; }
        private Action[] Actions { get; set; }

        public List<Action> ActivatedActions { get; private set; }

        public Controls()
        {
            ActivatedActions = new List<Action>();

            // default Actions and Keys
            Action Up = new Action("Up", new Keys[] { Keys.W, Keys.Up }, ActionType.OnPress);
            Action Down = new Action("Down", new Keys[] { Keys.S, Keys.Down }, ActionType.OnPress);
            Action Left = new Action("Left", new Keys[] { Keys.A, Keys.Left }, ActionType.OnPress);
            Action Right = new Action("Right", new Keys[] { Keys.D, Keys.Right }, ActionType.OnPress);
            // TODO: add interaction, attack, etc Actions

            Actions = new Action[] { Up, Down, Left, Right };
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState prevState = KeyboardState;
            KeyboardState = Keyboard.GetState();

            // Adds all Action objects that are activated to the ActivatedActions list
            ActivatedActions = Actions.Where(action => action.Activated(gameTime, prevState, KeyboardState)).ToList();
        }
    }
}
