using Capstone_Project.Globals;
using Capstone_Project.CollisionStuff.CollisionShapes;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Capstone_Project.GameObjects.Interfaces;
using System.Diagnostics;
using Capstone_Project.Fundamentals.DrawableShapes;

namespace Capstone_Project.CollisionStuff
{
    public enum CShapes { None, Rectangle, Circle, Polygon };

    public static class Collision
    {
        public static CShapes GetShape(CShape shape) => shape switch
        {
            CRectangle => CShapes.Rectangle,
            CCircle => CShapes.Circle,
            CPolygon => CShapes.Polygon,
            _ => CShapes.None
        };

        public static Rectangle GeneratePathCollider(Rectangle current, Rectangle target)
        {
            int x = Math.Min(current.Left, target.Left);
            int y = Math.Min(current.Top, target.Top);
            int width = Math.Max(current.Right, target.Right) - x;
            int height = Math.Max(current.Bottom, target.Bottom) - y;

            return new Rectangle(x, y, width, height);
        }

        #region Collision Detection

        #region Proper Collision Detection

        public static bool PrelimCheck(CShape a, CShape b)
        {
            return Rectangular(a.BoundingBox, b.BoundingBox);
        }

        public static bool Colliding(CShape a, CShape b)
        {
            return Colliding(a, b, out _);
        }

        public static bool Colliding(CShape a, CShape b, out CollisionDetails cd)
        {
            if (!PrelimCheck(a, b))
            {
                cd = new CollisionDetails();
                return false;
            }

            CShapes aType = GetShape(a);
            CShapes bType = GetShape(b);

            if (aType == bType)
            {
                switch (aType)
                {
                    case CShapes.Rectangle:
                        return Rectangular(a as CRectangle, b as CRectangle, out cd);
                    case CShapes.Circle:
                        return Circular(a as CCircle, b as CCircle, out cd);
                    case CShapes.Polygon:
                        return Polygonal(a as CPolygon, b as CPolygon, out cd);
                }
            }
            else
            {
                bool swapped, collided;
                // makes sure that a is always the Polygon
                if (swapped = bType == CShapes.Polygon)
                {
                    Utility.Swap(ref aType, ref bType);
                    Utility.Swap(ref a, ref b);
                }
                
                switch (aType)
                {
                    case CShapes.Polygon:
                        collided = PolygonOnNonPolygon(a as CPolygon, bType, b, out cd);
                        if (swapped)
                            cd.SwapAB();
                        return collided;
                    case CShapes.Circle:
                        collided = RectangleOnCircle(b as CRectangle, a as CCircle, out cd);
                        cd.SwapAB();
                        return collided;
                    case CShapes.Rectangle:
                        return RectangleOnCircle(a as CRectangle, b as CCircle, out cd);
                }
            }

            // the code SHOULD NEVER reach this
            cd = new CollisionDetails();
            return false;
        }

        private static bool PolygonOnNonPolygon(CPolygon poly, CShapes shapeType, CShape shape, out CollisionDetails cd)
        {
            switch (shapeType)
            {
                case CShapes.Rectangle:
                    bool collided = Polygonal(poly, new CPolygon(shape.Centre, Utility.GenerateVertices(shape.BoundingBox)), out cd);
                    cd.BType = CShapes.Rectangle;
                    cd.B = shape as CRectangle;
                    return collided;
                case CShapes.Circle:
                    return PolygonOnCircle(poly, shape as CCircle, out cd);
            }

            // the code SHOULD NEVER reach this
            cd = new CollisionDetails();
            return false;
        }

        #endregion

        #region AABB Collision

        public static bool Rectangular(CRectangle a, CRectangle b, out CollisionDetails cd)
        {
            cd = new CollisionDetails(CShapes.Rectangle, a, CShapes.Rectangle, b);

            if (Rectangular(a.BoundingBox, b.BoundingBox, out Rectangle intersection))
            {
                cd.Collided = true;
                cd.Intersection = intersection;

                FindRectangleCollisionDetails(a, b, intersection, out Vector2 aNormal, out Vector2 bNormal, out float depth);
                cd.ANormal = aNormal;
                cd.BNormal = bNormal;
                cd.Depth = depth;
            }

            return cd.Collided;
        }

        public static bool Rectangular(Rectangle a, Rectangle b, out Rectangle intersection)
        {
            return !(intersection = Rectangle.Intersect(a, b)).IsEmpty;
        }

        public static bool Rectangular(Rectangle a, Rectangle b)
        {
            return Rectangular(a, b, out _);
        }

        private static void FindRectangleCollisionDetails(CRectangle a, CRectangle b, Rectangle intersection, 
            out Vector2 aNormal, out Vector2 bNormal, out float depth)
        {
            FindRectangleCollisionDetails(a.BoundingBox, b.BoundingBox, intersection, out aNormal, out bNormal, out depth);
        }

        private static void FindRectangleCollisionDetails(Rectangle a, Rectangle b, Rectangle intersection,
            out Vector2 aNormal, out Vector2 bNormal, out float depth)
        {
            // calculates the normal b-to-a (aNormal)
            Vector2 displacement = (a.Center - b.Center).ToVector2();
            float absDiffX = MathF.Abs(displacement.X);
            float absDiffY = MathF.Abs(displacement.Y);

            // |X| > |Y| means the collision is on the x-axis (and vice versa)
            // |X| == |Y| means just shove them out from the corner
            if (absDiffX > absDiffY)
            {
                aNormal = new Vector2(Utility.Sign(displacement.X), 0);
                depth = intersection.Width;
            }
            else if (absDiffY > absDiffX)
            {
                aNormal = new Vector2(0, Utility.Sign(displacement.Y));
                depth = intersection.Height;
            }
            else    // if absDiffX == absDiffY
            {
                aNormal = Vector2.Normalize(displacement);
                depth = displacement.Length();
            }

            bNormal = -aNormal;
        }

        #endregion AABB Collision

        #region Rectangle-on-Circle

        public static bool RectangleOnCircle(CRectangle rect, CCircle circ, out CollisionDetails cd)
        {
            cd = new CollisionDetails(CShapes.Rectangle, rect, CShapes.Circle, circ);

            if (RectangleOnCircle(rect.BoundingBox, (circ.Centre, circ.Radius), out Vector2 rectNormal, out Vector2 circNormal, out float depth))
            {
                cd.Collided = true;
                cd.ANormal = rectNormal;
                cd.BNormal = circNormal;
                cd.Depth = depth;
            }

            return cd.Collided;
        }

        // checks if the square and circle collide by creating localised versions of each around the Rectangle's center being at the origin
        // note: localised rect has side lengths (rect.Width, rect.Height) and top-left (0, 0)
        public static bool RectangleOnCircle(Rectangle rect, (Vector2 centre, float radius) circ, 
            out Vector2 rectNormal, out Vector2 circNormal, out float depth)
        {
            rectNormal = Vector2.Zero;
            circNormal = Vector2.Zero;
            depth = 0;

            // gets the absolute position of the circle translated as if the centre of the rect was (0,0)
            Vector2 localCirc = new Vector2(MathF.Abs(circ.centre.X - rect.Center.X), MathF.Abs(circ.centre.Y - rect.Center.Y));
            Vector2 localCircSubLocalRect = localCirc - (rect.Size.ToVector2() / 2f);

            // tests if any of the (local) circ's cardinal points are within the bounds of the (local) rect
            if (localCircSubLocalRect.X < circ.radius && localCirc.Y < rect.Height / 2f
                || localCircSubLocalRect.Y < circ.radius && localCirc.X < rect.Width / 2f)
            {
                Rectangle circBB = new Rectangle((int)(circ.centre.X - circ.radius), (int)(circ.centre.Y - circ.radius), (int)(circ.radius * 2), (int)(circ.radius * 2));
                FindRectangleCollisionDetails(rect, circBB, Rectangle.Intersect(rect, circBB), out rectNormal, out circNormal, out depth);

                return true;
            }

            // tests if any of the corners of the (local) rect are within the radius of the (local) circle
            depth = circ.radius - localCircSubLocalRect.Length();
            if (depth > 0)
            {
                rectNormal = rect.Center.ToVector2() - circ.centre;
                rectNormal.Normalize();
                circNormal = -rectNormal;
                
                return true;
            }

            return false;
        }

        #endregion Rectangle-on-Circle

        #region Circle Collision

        public static bool Circular(CCircle a, CCircle b, out CollisionDetails cd)
        {
            cd = new CollisionDetails(CShapes.Circle, a, CShapes.Circle, b);

            if (Circular((a.Centre, a.Radius), (b.Centre, b.Radius), out float depth))
            {
                cd.Collided = true;
                cd.ANormal = Vector2.Normalize(a.Centre - b.Centre);
                cd.BNormal = -cd.ANormal;
                cd.Depth = depth;
            }

            return cd.Collided;
        }

        public static bool Circular((Vector2 centre, float r) a, (Vector2 centre, float r) b, out float depth)
        {
            depth = a.r + b.r - (a.centre - b.centre).Length();
            return depth > 0;
        }

        #endregion Circle Collision

        #region Polygon Collision (+Polygon-on-Circle, +Polygon-on-Rectangle)
        // AKA: Separating Axis Theorem

        public static bool PolygonOnCircle(CPolygon poly, CCircle circ, out CollisionDetails cd)
        {
            cd = new CollisionDetails(CShapes.Polygon, poly, CShapes.Circle, circ);

            if (PolygonOnCircle(poly.Vertices, (circ.Centre, circ.Radius), out Vector2 polyNormal, out Vector2 circNormal, out float depth))
            {
                cd.Collided = true;
                cd.ANormal = polyNormal;
                cd.BNormal = circNormal;
                cd.Depth = depth;
            }

            return cd.Collided;
        }

        public static bool PolygonOnCircle(Vector2[] poly, (Vector2 centre, float radius) circ, 
            out Vector2 polyNormal, out Vector2 circNormal, out float depth)
        {
            polyNormal = Vector2.Zero;
            circNormal = Vector2.Zero;
            depth = float.PositiveInfinity;

            // compiler complains if not for this
            float polyMin, polyMax, circMin, circMax, overlap;

            for (int i = 0; i < poly.Length; i++)
            {
                Vector2 axis = Utility.FindNormal(poly[i], poly[(i + 1) % poly.Length]);
                ProjectVertices(axis, poly, out polyMin, out polyMax);
                ProjectCircle(axis, circ, out circMin, out circMax);

                overlap = MathF.Min(circMax - polyMin, polyMax - circMin);
                if (overlap >= 0)
                    return false;

                if (overlap < depth)
                {
                    depth = overlap;
                    polyNormal = overlap == circMax - polyMin ? axis : -axis;
                }
            }

            // now testing the axis from the closest vertex to the circle centre
            Vector2 closestVertex = FindClosestVertex(circ.centre, poly);
            Vector2 cvAxis = closestVertex - circ.centre;
            ProjectVertices(cvAxis, poly, out polyMin, out polyMax);
            ProjectCircle(cvAxis, circ, out circMin, out circMax);

            overlap = MathF.Min(circMax - polyMin, polyMax - circMin);
            if (overlap >= 0)
                return false;

            if (overlap < depth)
            {
                depth = overlap;
                polyNormal = overlap == circMax - polyMin ? cvAxis : -cvAxis;
            }

            // normalising depth
            depth /= polyNormal.Length();
            polyNormal.Normalize();
            circNormal = -polyNormal;

            return true;
        }

        private static Vector2 FindClosestVertex(Vector2 target, Vector2[] vertices)
        {
            Vector2 closest = vertices[0];
            float closestLengthSquared = float.PositiveInfinity;

            foreach (Vector2 vertex in vertices.Skip(1))
            {
                float lengthSquared = (target - vertex).LengthSquared();
                if (lengthSquared < closestLengthSquared)
                {
                    closest = vertex;
                    closestLengthSquared = lengthSquared;
                }
            }

            return closest;
        }

        public static bool Polygonal(CPolygon a, CPolygon b, out CollisionDetails cd)
        {
            cd = new CollisionDetails(CShapes.Polygon, a, CShapes.Polygon, b);

            if (Polygonal(a.Vertices, b.Vertices, out Vector2 aNormal, out Vector2 bNormal, out float depth))
            {
                cd.Collided = true;
                cd.ANormal = aNormal;
                cd.BNormal = bNormal;
                cd.Depth = depth;
            }

            return cd.Collided;
        }

        public static bool Polygonal(Vector2[] a, Vector2[] b, out Vector2 aNormal, out Vector2 bNormal, out float depth)
        {
            //aNormal = Vector2.Zero;
            bNormal = Vector2.Zero;
            depth = 0;

            if (TestAgainstShape(a, b, out aNormal, out float aDepth) || TestAgainstShape(b, a, out bNormal, out float bDepth))
                return false;

            // if the minimum depth was found from testing a-to-b, use aNormal and the depth
            if (aDepth < bDepth)
            {
                depth = aDepth;
                bNormal = -aNormal;
            }
            else
            {
                depth = bDepth;
                aNormal = -bNormal;
            }

            return true;
        }

        private static bool TestAgainstShape(Vector2[] a, Vector2[] b, out Vector2 aNormal, out float depth)
        {
            aNormal = Vector2.Zero;
            depth = float.PositiveInfinity;

            for (int i = 0; i < a.Length; i++)
            {
                Vector2 axis = Utility.FindNormal(a[i], a[(i + 1) % a.Length]);
                ProjectVertices(axis, a, out float aMin, out float aMax);
                ProjectVertices(axis, b, out float bMin, out float bMax);

                float overlap = MathF.Min(bMax - aMin, aMax - bMin);
                if (overlap <= 0)
                    return false;

                if (overlap < depth)
                {
                    depth = overlap;
                    aNormal = overlap == bMax - aMin ? axis : -axis;
                }
            }
            depth /= aNormal.Length();       // normlise depth
            aNormal.Normalize();             // normalise normal

            return true;
        }

        // decided against this vvv signature because it's not obvious (float, float) means (min, max)
        //private static (float, float) ProjectVertices(Vector2 axis, Vector2[] vertices)
        private static void ProjectVertices(Vector2 axis, Vector2[] vertices, out float min, out float max)
        {
            min = float.PositiveInfinity;
            max = float.NegativeInfinity;

            for (int i = 0; i < vertices.Length; i++)
            {
                float projection = Vector2.Dot(axis, vertices[i]);
                if (projection < min)
                    min = projection;
                if (projection > max)
                    max = projection;
            }
        }

        private static void ProjectCircle(Vector2 axis, (Vector2 centre, float radius) circle, out float min, out float max)
        {
            // takes the normalised axis and finds the point on the circle's
            // edge that is parallel to the axis (when drawn from the centre)
            Vector2 circumferencePoint = Vector2.Normalize(axis) * circle.radius;

            min = Vector2.Dot(axis, circle.centre + circumferencePoint);
            max = Vector2.Dot(axis, circle.centre - circumferencePoint);
            if (min > max)
                Utility.Swap(ref min, ref max);
        }

        #endregion

        #endregion Collision Detection

        #region Collision Handling

        public static void HandleCollision(ICollidable a, ICollidable b, CollisionDetails cd)
        {
            if (!cd.Collided)
                return;

            if (a is IRespondable aRespondable)
            {
                if (b is IRespondable bRespondable)
                {
                    float moveBy = cd.Depth / 2f;
                    aRespondable.TargetPos += cd.ANormal * moveBy;
                    bRespondable.TargetPos += cd.BNormal * moveBy;
                }
                else    // if b is static (only ICollidable)
                    aRespondable.TargetPos += cd.ANormal * cd.Depth;
            }
            else if (b is IRespondable bRespondable)
                bRespondable.TargetPos += cd.BNormal * cd.Depth;
        }

        #endregion Collision Handling
    }
}
