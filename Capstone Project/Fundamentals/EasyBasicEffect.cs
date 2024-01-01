using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Capstone_Project.Fundamentals
{
    public class EasyBasicEffect : BasicEffect
    {
        public EasyBasicEffect(GraphicsDevice device) : base(device)
        {
            VertexColorEnabled = true;

            Projection = Game1.Camera.TransformMatrix;
        }

        protected EasyBasicEffect(BasicEffect cloneSource) : base(cloneSource)
        {
        }
    }
}
