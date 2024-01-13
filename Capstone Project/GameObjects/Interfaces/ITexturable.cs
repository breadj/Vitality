using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;

namespace Capstone_Project.GameObjects.Interfaces
{
    public interface ITexturable : IDrawable
    {
        //public bool Visible { get; }
        public Subsprite Subsprite { get; }
        public Rectangle Destination { get; }
        public Color Colour { get; }
        public float Rotation { get; }
        public Vector2 Origin { get; }
        //public float Layer { get; }
        //public void Draw();
    }
}
