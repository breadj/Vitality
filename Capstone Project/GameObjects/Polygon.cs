using static Capstone_Project.Globals.Globals;
using Capstone_Project.GameObjects.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Transactions;

namespace Capstone_Project.GameObjects
{
    public class Polygon : Interfaces.IDrawable, IUpdatable, ICollidable
    {
        public bool Visible { get; set; } = true;

        public bool Active { get; set; } = true;
        public Rectangle Hitbox { get; init; }
        public bool IsCircle => false;
        public float Radius => 0f;

        public Vector2 Centre { get; init; }
        public Vector2[] Vertices { get; init; }
        public Color Colour { get; set; }

        private List<Point> outline { get; init; }
        private Rectangle[] scanLines { get; init; }
        private List<Point> pixels { get; init; }

        public Polygon(Vector2[] vertices, Color colour, Vector2? centre = null)
        {
            Centre = centre ?? Vector2.Zero;
            Vertices = vertices.Select(vertex => vertex + Centre).ToArray();
            Colour = colour;

            outline = GenerateWireframe(Vertices);
            Hitbox = GenerateHitbox(Vertices);
            scanLines = GenerateLineFill(outline, Hitbox.Top, Hitbox.Bottom);
            pixels = GeneratePixelFill(outline, Hitbox.Top, Hitbox.Bottom);
        }

        public void Update(GameTime gameTime)
        {
            if (!Active)
                return;
        }

        public void Draw()
        {
            if (!Visible)
                return;

            foreach (Point px in outline)
            {
                spriteBatch.Draw(Pixel, px.ToVector2(), null, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.005f);
            }

            // to draw via scanlines
            foreach (Rectangle line in scanLines)
                spriteBatch.Draw(Pixel, line, null, Colour, 0f, Vector2.Zero, SpriteEffects.None, 0.004f);

            // to draw via individual pixels
            /*foreach (Point px in pixels)
                spriteBatch.Draw(Pixel, px.ToVector2(), null, Colour, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.004f);*/

            // draw hitbox (debug only)
            //spriteBatch.Draw(BLANK, Hitbox, null, Colour, 0f, Vector2.Zero, SpriteEffects.None, 0.004f);
        }

        public CollisionDetails CollidesWith(ICollidable other)
        {
            throw new NotImplementedException();
        }

        private static Rectangle GenerateHitbox(Vector2[] vertices)
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

        #region Line-Rasterising & Wireframe Algorithms
        // AKA: Bresenham's Line Algorithm
        private static List<Point> RasteriseLine(Point a, Point b)
        {
            List<Point> points = new List<Point>();

            (int x, int y) d = (Math.Abs(b.X - a.X), -Math.Abs(b.Y - a.Y));
            (int x, int y) s = (Globals.Utility.Sign(b.X - a.X), Globals.Utility.Sign(b.Y -a.Y));
            int e = d.x + d.y;

            Point current = new Point(a.X, a.Y);
            while (true)
            {
                if (current == b)
                    break;

                int e2 = e * 2;

                if (e2 >= d.y)
                {
                    if (current.X == b.X)
                        break;
                    e += d.y;
                    current.X += s.x;
                }
                if (e2 <= d.x)
                {
                    if (current.Y == b.Y)
                        break;
                    e += d.x;
                    current.Y += s.y;
                }

                points.Add(current);
            }

            if (!points.Any())
                points.Add(b);

            return points;          // doesn't include the Point a, which works better for wireframing
        }

        private static List<Point> GenerateWireframe(Vector2[] vertices)
        {
            List<Point> points = new List<Point>();

            for (int i = 0; i < vertices.Length; i++)
                points.AddRange(RasteriseLine(vertices[i].ToPoint(), vertices[(i + 1) % vertices.Length].ToPoint()));

            return points;
        }
        #endregion

        #region Scan Conversion
        private static (int[], int[]) FindXBounds(List<Point> outline, int minY, int height)
        {
            int[] minXs = new int[height];
            int[] maxXs = new int[height];
            Array.Fill(minXs, int.MaxValue);
            Array.Fill(maxXs, int.MinValue);

            foreach (Point p in outline)
            {
                int trueIndex = p.Y - minY;
                if (p.X < minXs[trueIndex])
                    minXs[trueIndex] = p.X;
                if (p.X > maxXs[trueIndex])
                    maxXs[trueIndex] = p.X;
            }

            return (minXs, maxXs);
        }

        private static Rectangle[] GenerateLineFill(List<Point> outline, int minY, int maxY)
        {
            int height = maxY - minY;
            (int[] minXs, int[] maxXs) = FindXBounds(outline, minY, height);

            Rectangle[] lines = new Rectangle[height];

            for (int i = 0; i < height; i++)
            {
                int trueY = i + minY;
                lines[i] = new Rectangle(minXs[i], trueY, maxXs[i] - minXs[i], 1);
            }

            return lines;
        }

        private static List<Point> GeneratePixelFill(List<Point> outline, int minY, int maxY)
        {
            int height = maxY - minY;
            (int[] minXs, int[] maxXs) = FindXBounds(outline, minY, height);

            List<Point> pixels = new List<Point>();

            for (int y = 0; y < height; y++)
            {
                int trueY = y + minY;
                for (int x = 0; x < maxXs[y] - minXs[y]; x++)
                    pixels.Add(new Point(minXs[y] + x, trueY));
            }

            return pixels;
        }
        #endregion

        #region Pre-defined Polygons & Generators
        // assumes vertices are of origin = (0,0)
        public static Vector2[] Rotate(Vector2[] vertices, float rotation)
        {
            return vertices.Select(vertex => Globals.Utility.RotateVector(vertex, rotation)).ToArray();
        }

        public static Vector2[] Rotate(Vector2[] vertices, float rotation, Vector2 origin)
        {
            return vertices.Select(vertex => Globals.Utility.RotateVector(vertex - origin, rotation) + origin).ToArray();
        }

        // generated all of these using an external (self-made) script
        // all are normalised (radius = 1), so scaling by x works better
        // half a 20-sided polygon (11 sides + (0,0))
        public static Vector2[] SemiCircle { get; } = new Vector2[] { new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.9510565f, 0.309017f), new Vector2(0.809017f, 0.5877852f), new Vector2(0.5877852f, 0.809017f), new Vector2(0.309017f, 0.9510565f), new Vector2(0f, 1f), new Vector2(-0.309017f, 0.9510565f), new Vector2(-0.5877854f, 0.8090169f), new Vector2(-0.8090171f, 0.5877852f), new Vector2(-0.9510565f, 0.309017f), new Vector2(-1f, 0f) };
        // SemiCircle but missing one vertex on either side (plus (0,0) for the origin)
        public static Vector2[] WideArc { get; } = new Vector2[] { new Vector2(0f, 0f), new Vector2(0.9510565f, 0.309017f), new Vector2(0.809017f, 0.5877852f), new Vector2(0.5877852f, 0.809017f), new Vector2(0.309017f, 0.9510565f), new Vector2(0f, 1f), new Vector2(-0.309017f, 0.9510565f), new Vector2(-0.5877854f, 0.8090169f), new Vector2(-0.8090171f, 0.5877852f), new Vector2(-0.9510565f, 0.309017f) };
        // SemiCircle but missing three vertices on either side (plus (0,0) for the origin)
        public static Vector2[] NarrowArc { get; } = new Vector2[] { new Vector2(0f, 0f), new Vector2(0.5877852f, 0.809017f), new Vector2(0.309017f, 0.9510565f), new Vector2(0f, 1f), new Vector2(-0.309017f, 0.9510565f), new Vector2(-0.5877854f, 0.8090169f) };


        // unrotated (facing down)
        public static Vector2[] GenerateWideArc(float scale = 1f)
        {
            return WideArc.Select(vertex => vertex * scale).ToArray();
        }

        public static Vector2[] GenerateNarrowArc(float scale = 1f)
        {
            return NarrowArc.Select(vertex => vertex * scale).ToArray();
        }
        #endregion
    }
}
