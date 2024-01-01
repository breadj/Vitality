using Microsoft.Xna.Framework.Input;

namespace Capstone_Project.Input
{
    public struct InputState
    {
        public KeyboardState KeyboardState { get; set; }
        public MouseState MouseState { get; set; }

        public InputState()
        {
            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();
        }

        public bool IsDown(Input input)
        {
            // if input is a Keyboard Key and it's down
            if (input.IsKey && KeyboardState.IsKeyDown((Keys)input))
                return true;

            // if input is a Mouse Button
            return (Button)input switch
            {
                Button.Left => MouseState.LeftButton == ButtonState.Pressed,
                Button.Right => MouseState.RightButton == ButtonState.Pressed,
                Button.Middle => MouseState.MiddleButton == ButtonState.Pressed,
                Button.Mouse4 => MouseState.XButton1 == ButtonState.Pressed,
                Button.Mouse5 => MouseState.XButton2 == ButtonState.Pressed,
                _ => false,
            };
        }

        public bool IsUp(Input input)
        {
            // if input is a Keyboard Key and it's up
            if (input.IsKey && KeyboardState.IsKeyUp((Keys)input))
                return true;

            // if input isn't a Key, it must be a Mouse Button
            return (Button)input switch
            {
                Button.Left => MouseState.LeftButton == ButtonState.Released,
                Button.Right => MouseState.RightButton == ButtonState.Released,
                Button.Middle => MouseState.MiddleButton == ButtonState.Released,
                Button.Mouse4 => MouseState.XButton1 == ButtonState.Released,
                Button.Mouse5 => MouseState.XButton2 == ButtonState.Released,
                _ => false,
            };
        }
    }
}
