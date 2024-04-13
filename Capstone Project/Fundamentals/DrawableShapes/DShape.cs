using static Capstone_Project.Globals.Globals;
using Capstone_Project.GameObjects.Interfaces;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Capstone_Project.Fundamentals.DrawableShapes
{
    public abstract class DShape : GameObjects.Interfaces.IDrawable
    {
        public bool Visible { get; set; } = true;
        public float Layer { get; set; } = 0.005f;
        
        public Color Colour { get; set; } = Color.White;
        public Rectangle BoundingBox { get; protected set; }
        public Vector2 Centre { get; protected set; }

        public bool Outlined { get; set; } = false;
        protected List<Point> Outline { get; set; }

        public bool Scanned { get; init; } = true;
        protected List<Rectangle> ScanLines { get; set; }
        protected List<Point> Pixels { get; set; }

        public DShape(Vector2 centre)
        {
            Centre = centre;
        }

        public DShape(Vector2 centre, Color colour, float layer = 0.005f, bool scanned = true)
        {
            Centre = centre;
            Colour = colour;
            Layer = layer;

            Scanned = scanned;
        }

        public void DrawOutline()
        {
            DrawOutline(Color.White);
        }

        public void DrawOutline(Color colour)
        {
            if (!Visible)
                return;

            foreach (var px in Outline)
            {
                spriteBatch.Draw(Pixel, px.ToVector2(), null, colour, 0f, Vector2.Zero, 1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, Layer);
            }
        }

        public void Draw()
        {
            Draw(Colour);
        }

        public void Draw(Color colour)
        {
            if (!Visible)
                return;

            if (Outlined)
                foreach (var px in Outline)
                    spriteBatch.Draw(Pixel, px.ToVector2(), null, Color.Black, 0f, Vector2.Zero, 1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, Layer + 0.0001f);

            if (Scanned)
                foreach (var line in ScanLines)
                    spriteBatch.Draw(Pixel, line, null, colour, 0f, Vector2.Zero, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, Layer);
            else
                foreach (var px in Pixels)
                    spriteBatch.Draw(Pixel, px.ToVector2(), null, colour, 0f, Vector2.Zero, 1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, Layer);
        }

        public abstract void MoveTo(Vector2 target, float rotation = 0f);

        protected abstract void GenerateFillMode(bool scanned);
    }
}
