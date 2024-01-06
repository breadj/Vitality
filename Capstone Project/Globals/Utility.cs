using Microsoft.Xna.Framework;
using System;

namespace Capstone_Project.Globals
{
    public static class Utility
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public static int Round(float x) => (int)(x + 0.5f);
        public static Point IndexToCoord(int index, int width, int height) => new Point(index % width, index / width);
        public static int Sign(float x) => x < 0 ? -1 : 1;
        public static int Sign(int x) => x < 0 ? -1 : 1;
        public static Vector2 AngleToVector(float angle) => new Vector2(-MathF.Sin(angle), MathF.Cos(angle));       // 0 degrees is down
        public static Vector2 RotateVector(Vector2 vector, float angle) =>          // assumes rotation around (0,0)
            new Vector2(vector.X * MathF.Cos(angle) - vector.Y * MathF.Sin(angle), vector.X * MathF.Sin(angle) + vector.Y * MathF.Cos(angle));
        public static float VectorToAngle(Vector2 vector) => MathF.Atan2(-vector.X, vector.Y);
        public static float AngleTowards(Vector2 from, Vector2 to) => VectorToAngle(to - from);
        public static Vector2 FindNormal(Vector2 start, Vector2 end) => new Vector2(-(end.Y - start.Y), end.X - start.X);
        public static Vector2 FindNormal(Vector2 edge) => new Vector2(-edge.Y, edge.X);

        
        public static Vector2[] GenerateVertices(Rectangle rect)    // generates vertices in a clockwise manner
        {
            return new Vector2[]
            {
                new Vector2(rect.Left, rect.Top),
                new Vector2(rect.Right, rect.Top),
                new Vector2(rect.Right, rect.Bottom),
                new Vector2(rect.Left, rect.Bottom)
            };
        }

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
