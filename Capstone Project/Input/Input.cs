using Microsoft.Xna.Framework.Input;

namespace Capstone_Project.Input
{
    public class Input
    {
        private Keys Key { get; init; } = Keys.None;
        private Button Button { get; init; } = Button.None;

        public bool IsKey => Key != Keys.None;
        public bool IsButton => Button != Button.None;

        public Input(Keys key)
        {
            Key = key;
        }

        public Input(Button button)
        {
            Button = button;
        }

        public static explicit operator Keys(Input input) => input.Key;
        public static explicit operator Button(Input input) => input.Button;

        public static implicit operator Input(Keys key) => new Input(key);
        public static implicit operator Input(Button button) => new Input(button);
    }
}
