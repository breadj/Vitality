using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Capstone_Project.Globals
{
    public static class Globals
    {
        public static GameTime gameTime = null;
        public static Texture2D BLANK = null;
        public static Texture2D Pixel = null;
        public static SpriteFont DebugFont = null;
        public static GraphicsDevice GraphicsDevice = null;
        public static SpriteBatch spriteBatch = null;
        public static readonly float Epsilon = 0.000001f;
    }
}
