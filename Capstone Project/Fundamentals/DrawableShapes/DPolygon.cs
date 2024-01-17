using static Capstone_Project.Globals.Globals;
using Capstone_Project.GameObjects.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Capstone_Project.CollisionStuff;
using Capstone_Project.Globals;
using Capstone_Project.CollisionStuff.CollisionShapes;

namespace Capstone_Project.Fundamentals.DrawableShapes
{
    public class DPolygon : DShape
    {
        public bool Active { get; set; } = true;

        public Vector2[] Vertices { get; protected set; }

        public DPolygon(Vector2 centre, Vector2[] vertices, bool scanned = true) : base(centre)
        {
            Scanned = scanned;
            Vertices = vertices;

            BoundingBox = Utility.GenerateBoundingBox(Vertices);

            Outline = GraphicalMethods.GenerateOutline(Vertices);
            GenerateFillMode(scanned);
        }

        public DPolygon(Vector2 centre, Vector2[] vertices, Color colour, float layer = 0.004f, bool scanned = true) 
            : base(centre, colour, layer, scanned)
        {
            Vertices = vertices.Select(vertex => vertex + Centre).ToArray();

            BoundingBox = Utility.GenerateBoundingBox(Vertices);

            Outline = GraphicalMethods.GenerateOutline(Vertices);
            GenerateFillMode(scanned);
        }

        public DPolygon(CPolygon poly, bool scanned = true) : base(poly.Centre)
        {
            Scanned = scanned;
            Vertices = poly.Vertices;

            BoundingBox = poly.BoundingBox;
            
            Outline = GraphicalMethods.GenerateOutline(Vertices);
            GenerateFillMode(scanned);
        }

        public DPolygon(CPolygon poly, Color colour, float layer = 0.004f, bool scanned = true) : base(poly.Centre, colour, layer, scanned)
        {
            Vertices = poly.Vertices;

            BoundingBox = poly.BoundingBox;

            Outline = GraphicalMethods.GenerateOutline(Vertices);
            GenerateFillMode(scanned);
        }

        public override void MoveTo(Vector2 target, float rotation = 0f)
        {
            Vector2 displacement = target - Centre;

            if (rotation != 0f)
                Vertices = Rotate(Vertices, rotation, Centre);
            if (target != Centre)
                Vertices = Vertices.Select(vertex => vertex + displacement).ToArray();
            Centre = target;
            BoundingBox = Utility.GenerateBoundingBox(Vertices);

            Outline = GraphicalMethods.GenerateOutline(Vertices);
            GenerateFillMode(Scanned);
        }

        protected override void GenerateFillMode(bool scanned)
        {
            if (scanned)
                ScanLines = GraphicalMethods.GenerateLineFill(Outline, BoundingBox.Top, BoundingBox.Bottom);
            else
                Pixels = GraphicalMethods.GeneratePixelFill(Outline, BoundingBox.Top, BoundingBox.Bottom);
        }

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
        public static Vector2[] SemiCircle => new Vector2[] { new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.9510565f, 0.309017f), new Vector2(0.809017f, 0.5877852f), new Vector2(0.5877852f, 0.809017f), new Vector2(0.309017f, 0.9510565f), new Vector2(0f, 1f), new Vector2(-0.309017f, 0.9510565f), new Vector2(-0.5877854f, 0.8090169f), new Vector2(-0.8090171f, 0.5877852f), new Vector2(-0.9510565f, 0.309017f), new Vector2(-1f, 0f) };
        // SemiCircle but missing one vertex on either side (plus (0,0) for the origin)
        public static Vector2[] WideArc => new Vector2[] { new Vector2(0f, 0f), new Vector2(0.9510565f, 0.309017f), new Vector2(0.809017f, 0.5877852f), new Vector2(0.5877852f, 0.809017f), new Vector2(0.309017f, 0.9510565f), new Vector2(0f, 1f), new Vector2(-0.309017f, 0.9510565f), new Vector2(-0.5877854f, 0.8090169f), new Vector2(-0.8090171f, 0.5877852f), new Vector2(-0.9510565f, 0.309017f) };
        // SemiCircle but missing three vertices on either side (plus (0,0) for the origin)
        public static Vector2[] NarrowArc => new Vector2[] { new Vector2(0f, 0f), new Vector2(0.5877852f, 0.809017f), new Vector2(0.309017f, 0.9510565f), new Vector2(0f, 1f), new Vector2(-0.309017f, 0.9510565f), new Vector2(-0.5877854f, 0.8090169f) };


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
