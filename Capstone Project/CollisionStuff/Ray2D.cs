using Capstone_Project.Fundamentals;
using Microsoft.Xna.Framework;
using System;

namespace Capstone_Project.CollisionStuff
{
    public struct Ray2D
    {
        public readonly Vector2 Start;
        public readonly Vector2 Direction;
        public readonly float Length;
        public readonly Vector2 End;

        public readonly Vector2 Ray;

        /// <summary>
        /// Instantiates a Ray2D object using Direction and Length
        /// </summary>
        /// <param name="start">Point from which the Ray2D originates</param>
        /// <param name="direction">Normalised direction the line follows from the Start</param>
        /// <param name="length">Length of the line from the Start to the End points</param>
        public Ray2D(Vector2 start, Vector2 direction, float length)
        {
            Start = start;
            
            if (direction == Vector2.Zero) 
                throw new ArgumentException("Direction cannot be (0,0)");

            Direction = direction;
            Length = length;
            End = start + direction * length;

            Ray = End - start;
        }

        /// <summary>
        /// Instantiates a Ray2D object using the Start and End points
        /// </summary>
        /// <param name="start">Point from which the Ray2D originates</param>
        /// <param name="end">Point from which the Ray2D ends</param>
        public Ray2D(Vector2 start, Vector2 end)
        {
            if (end == start)
                throw new ArgumentException("Start cannot be the same as End");

            Start = start;
            End = end;

            Ray = end - start;

            Length = Ray.Length();
            Direction = Ray / Length;        // normalising
        }

        public readonly bool IsPointOnRay(Vector2 point)
        {
            return new LinearEquation(this).IsPointOnLine(point);
        }
    }
}
