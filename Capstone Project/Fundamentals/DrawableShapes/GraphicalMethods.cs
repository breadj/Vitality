using Capstone_Project.Globals;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Capstone_Project.Fundamentals.DrawableShapes
{
    public static class GraphicalMethods
    {
        #region Rasterising

        // AKA: Bresenham's Line Algorithm
        private static List<Point> RasteriseLine(Point a, Point b)
        {
            List<Point> points = new List<Point>();

            (int x, int y) d = (Math.Abs(b.X - a.X), -Math.Abs(b.Y - a.Y));
            (int x, int y) s = (Utility.Sign(b.X - a.X), Utility.Sign(b.Y - a.Y));
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

        public static List<Point> GenerateOutline(Vector2[] vertices)
        {
            List<Point> outline = new List<Point>();

            for (int i = 0; i < vertices.Length; i++)
                outline.AddRange(RasteriseLine(vertices[i].ToPoint(), vertices[(i + 1) % vertices.Length].ToPoint()));

            return outline;
        }

        // AKA: Jesko's Method
        public static List<Point> GenerateOutline(Vector2 centre, float radius)
        {
            List<Point> outline = new List<Point>();

            Point c = new Point(Utility.Round(centre.X), Utility.Round(centre.Y));
            int r = (int)radius;

            int x = r, y = 0;
            int t1 = r >> 4;
            int t2;

            while (x >= y)
            {
                PlaceInOctants(ref outline, (c.X, c.Y), (x, y));

                t1 += ++y;
                t2 = t1 - x;
                if (t2 >= 0)
                {
                    t1 = t2;
                    x--;
                }
            }

            // get rid of the duplicates (deterministically duplicates since this will always produce an odd-diameter circle)
            outline.RemoveRange(outline.Count - 5, 4);
            outline.RemoveRange(0, 2);
            outline.RemoveRange(3, 2);

            return outline;
        }

        private static void PlaceInOctants(ref List<Point> points, (int x, int y) centre, (int x, int y) pixel)
        {
            points.Add(new Point(centre.x + pixel.x, centre.y + pixel.y));
            points.Add(new Point(centre.x - pixel.x, centre.y + pixel.y));
            points.Add(new Point(centre.x + pixel.x, centre.y - pixel.y));
            points.Add(new Point(centre.x - pixel.x, centre.y - pixel.y));
            points.Add(new Point(centre.x + pixel.y, centre.y + pixel.x));
            points.Add(new Point(centre.x - pixel.y, centre.y + pixel.x));
            points.Add(new Point(centre.x + pixel.y, centre.y - pixel.x));
            points.Add(new Point(centre.x - pixel.y, centre.y - pixel.x));
        }

        #endregion

        #region Scan Conversion

        public static List<Rectangle> GenerateLineFill(Rectangle rect)
        {
            List<Rectangle> lines = new List<Rectangle>(rect.Height);

            for (int i = rect.Top; i <= rect.Bottom; i++)
                lines.Add(new Rectangle(rect.Left, i, rect.Width, 1));

            return lines;
        }

        public static List<Rectangle> GenerateCircleLineFill(Point centre, List<Point> outline, int minY, int maxY)
        {
            int height = maxY - minY;
            int[] minXs = FindSymmetricalMinXBounds(outline, minY, height);

            List<Rectangle> lines = new List<Rectangle>(height);

            for (int i = 0; i < minXs.Length; i++)
            {
                int trueY = i + minY;
                int lineWidth = centre.X + (centre.X - minXs[trueY]);
                lines[i] = new Rectangle(minXs[i], trueY, lineWidth, 1);
            }

            return lines;
        }

        public static List<Rectangle> GenerateLineFill(List<Point> outline, int minY, int maxY)
        {
            int height = maxY - minY;
            (int[] minXs, int[] maxXs) = FindXBounds(outline, minY, height);

            List<Rectangle> lines = new List<Rectangle>(height);

            for (int i = 0; i < height; i++)
            {
                int trueY = i + minY;
                lines[i] = new Rectangle(minXs[i], trueY, maxXs[i] - minXs[i], 1);
            }

            return lines;
        }

        public static List<Point> GeneratePixelFill(Rectangle rect)
        {
            List<Point> pixels = new List<Point>(rect.Width * rect.Height);

            for (int y = rect.Top; y <= rect.Bottom; y++)
                for (int x = rect.Left; x <= rect.Right; x++)
                    pixels.Add(new Point(x, y));

            return pixels;
        }

        public static List<Point> GenerateCirclePixelFill(Point centre, List<Point> outline, int minY, int maxY)
        {
            int height = maxY - minY;
            int[] minXs = FindSymmetricalMinXBounds(outline, minY, height);

            List<Point> pixels = new List<Point>();

            for (int y = 0; y < minXs.Length; y++)
            {
                int trueY = y + minY;
                int lineWidth = centre.X + (centre.X - minXs[trueY]);
                for (int x = 0; x < lineWidth; x++)
                    pixels.Add(new Point(minXs[y] + x, trueY));
            }

            return pixels;
        }

        public static List<Point> GeneratePixelFill(List<Point> outline, int minY, int maxY)
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

        private static int[] FindSymmetricalMinXBounds(List<Point> outline, int minY, int height)
        {
            int[] minXs = new int[height];
            Array.Fill(minXs, int.MaxValue);

            foreach (Point p in outline)
            {
                int trueIndex = p.Y - minY;
                if (p.X < minXs[trueIndex])
                    minXs[trueIndex] = p.X;
            }

            return minXs;
        }

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

        #endregion
    }
}
