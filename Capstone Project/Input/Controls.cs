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
        private Action[] MovementActions { get; set; }
        private Action[] NonBufferedActions { get; set; }
        private Action[] BufferedActions { get; set; }

        public Point MousePos => InputState.MouseState.Position;    // remember that this is screen position, not world position
        public List<Action> ActivatedActions { get; private set; }
        public ActionBuffer ActionBuffer { get; init; }
        public bool ExitFlag { get; private set; } = false;
        public Action Exit { get; private set; }

        public Controls()
        {
            InputState = new InputState();
            ActionBuffer = new ActionBuffer(3);

            ActivatedActions = new List<Action>();
            Exit = new Action("exit", new List<Input> { Keys.Escape }, ActionType.OnPress);

            // movement Actions
            Action Up = new Action("up", new List<Input> { Keys.W, Keys.Up }, ActionType.WhenDown);
            Action Down = new Action("down", new List<Input> { Keys.S, Keys.Down }, ActionType.WhenDown);
            Action Left = new Action("left", new List<Input> { Keys.A, Keys.Left }, ActionType.WhenDown);
            Action Right = new Action("right", new List<Input> { Keys.D, Keys.Right }, ActionType.WhenDown);

            MovementActions = new Action[] { Up, Down, Left, Right };

            // non-buffered Actions
            Action LockOn = new Action("lock-on", new List<Input> { Button.Middle }, ActionType.OnPress);
            Action LevelUp = new Action("level-up", new List<Input> { Keys.L }, ActionType.OnPress);

            NonBufferedActions = new Action[] { LockOn, LevelUp };

            // buffered Actions
            Action Attack = new Action("attack", new List<Input> { Button.Left }, ActionType.OnRelease);
            Action Dash = new Action("dash", new List<Input> { Keys.LeftShift }, ActionType.OnPress);

            BufferedActions = new Action[] { Attack, Dash };
        }

        public void Update(GameTime gameTime)
        {
            InputState prevState = InputState;
            InputState = new InputState();
            // not too sure why I had to get rid of the Update() function and replace it with a new instantiation, but it works now at least

            if (ExitFlag = Exit.Activated((float)gameTime.ElapsedGameTime.TotalSeconds, prevState, InputState))
                return;

            ActivatedActions.Clear();

            // Adds all Action objects that are activated to the ActivatedActions list
            ActivatedActions.AddRange(MovementActions.Where(action => action.Activated((float)gameTime.ElapsedGameTime.TotalSeconds, prevState, InputState)));
            ActivatedActions.AddRange(NonBufferedActions.Where(action => action.Activated((float)gameTime.ElapsedGameTime.TotalSeconds, prevState, InputState)));

            // Adds all Action objects that are bufferable and are activated to the ActionBuffer
            foreach (Action action in BufferedActions.Where(action => action.Activated((float)gameTime.ElapsedGameTime.TotalSeconds, prevState, InputState)))
                ActionBuffer.Add(action);
        }
    }
}
