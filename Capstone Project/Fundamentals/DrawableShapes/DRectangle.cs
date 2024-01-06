using static Capstone_Project.Globals.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Capstone_Project.Fundamentals.DrawableShapes
{
    public class DRectangle : DShape
    {
        public Rectangle BoundingBox { get; set; }

        public DRectangle(Vector2 centre, Rectangle boundingBox) : base(centre)
        {
            BoundingBox = boundingBox;
        }

        public DRectangle(Vector2 centre, Rectangle boundingBox, Color colour, float layer = 0.005f) : base(centre, colour, layer)
        {
            BoundingBox = boundingBox;
        }

        public override void Draw()
        {
            if (!Visible)
                return;

            spriteBatch.Draw(Pixel, BoundingBox, null, Colour, 0f, Vector2.Zero, SpriteEffects.None, Layer);
        }
    }
}
