using static Capstone_Project.Globals.Globals;
using Capstone_Project.GameObjects.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Capstone_Project.CollisionStuff;
using Capstone_Project.Globals;

namespace Capstone_Project.Fundamentals.DrawableShapes
{
    public class DPolygon : DShape
    {
        public bool Active { get; set; } = true;
        public Rectangle Hitbox { get; init; }

        public Vector2[] Vertices { get; init; }

        protected override List<Point> outline { get; set; }
        protected override List<Rectangle> scanLines { get; set; }
        protected override List<Point> pixels { get; set; }

        public DPolygon(Vector2 centre, Vector2[] vertices, Color colour, float layer = 0.004f) : base(centre, colour, layer)
        {
            Vertices = vertices.Select(vertex => vertex + Centre).ToArray();

            outline = GraphicalMethods.GenerateOutline(Vertices);
            Hitbox = Utility.GenerateBoundingBox(Vertices);
            scanLines = GraphicalMethods.GenerateLineFill(outline, Hitbox.Top, Hitbox.Bottom);
            pixels = GraphicalMethods.GeneratePixelFill(outline, Hitbox.Top, Hitbox.Bottom);
        }

        public override void Draw()
        {
            if (!Visible)
                return;

            // draws outline in black
            /*foreach (Point px in outline)
                spriteBatch.Draw(Pixel, px.ToVector2(), null, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.005f);*/

            // to draw via scanlines
            foreach (Rectangle line in scanLines)
                spriteBatch.Draw(Pixel, line, null, Colour, 0f, Vector2.Zero, SpriteEffects.None, 0.004f);

            // to draw via individual pixels
            /*foreach (Point px in pixels)
                spriteBatch.Draw(Pixel, px.ToVector2(), null, Colour, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.004f);*/

            // draw hitbox (debug only)
            //spriteBatch.Draw(BLANK, Hitbox, null, Colour, 0f, Vector2.Zero, SpriteEffects.None, 0.004f);
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
