using Capstone_Project.CollisionStuff;
using Capstone_Project.Globals;
using Microsoft.Xna.Framework;

namespace Capstone_Project.Fundamentals
{
    public struct LinearEquation
    {
        // referring to y = mx + c
        public float M { get; init; }
        public float C { get; init; }

        public LinearEquation(float m, float c)
        {
            M = m;
            C = c;
        }

        public LinearEquation(Vector2 point, Vector2 direction)
        {
            M = direction.Y / direction.X;
            C = M * point.X - point.Y;
        }

        public LinearEquation(Ray2D ray)
        {
            M = ray.Ray.Y / ray.Ray.X;
            C = M * ray.Start.X - ray.Start.Y;
        }

        public bool IsPointOnLine(Vector2 point)
        {
            return Utility.NearlyEquals(M * point.X - C, point.Y);
        }

        public static bool IsPointOnLine(LinearEquation eq, Vector2 point)
        {
            return Utility.NearlyEquals(eq.M * point.X - eq.C, point.Y);
        }
    }
}
