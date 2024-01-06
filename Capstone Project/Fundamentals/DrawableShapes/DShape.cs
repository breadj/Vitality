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
        public Vector2 Centre { get; set; }
        protected abstract List<Point> outline { get; set; }
        protected abstract List<Rectangle> scanLines { get; set; }
        protected abstract List<Point> pixels { get; set; }

        public DShape(Vector2 centre)
        {
            Centre = centre;
        }

        public DShape(Vector2 centre, Color colour, float layer = 0.005f)
        {
            Centre = centre;
            Colour = colour;
            Layer = layer;
        }

        public abstract void Draw();

        public static Rectangle GenerateBoundingBox(Vector2[] vertices)
        {
            float minX = float.PositiveInfinity, minY = float.PositiveInfinity, maxX = float.NegativeInfinity, maxY = float.NegativeInfinity;

            foreach (Vector2 vertex in vertices)
            {
                if (vertex.X < minX)
                    minX = vertex.X;
                if (vertex.Y < minY)
                    minY = vertex.Y;

                if (vertex.X > maxX)
                    maxX = vertex.X;
                if (vertex.Y > maxY)
                    maxY = vertex.Y;
            }

            return new Rectangle((int)minX, (int)minY, (int)(maxX - minX) + 1, (int)(maxY - minY) + 1);
        }
    }
}
