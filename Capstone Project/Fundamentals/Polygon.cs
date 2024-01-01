using Capstone_Project.GameObjects.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace Capstone_Project.Fundamentals
{
    public class Polygon : GameObjects.Interfaces.IDrawable, IUpdatable
    {
        public bool Visible { get; set; } = false;

        public bool Active { get; set; } = false;

        public Vector2 Centre { get; init; }
        public VertexPositionColor[] SpecialVertices { get; private set; }
        public Vector2[] Vertices { get; init; }
        public short[] Indices { private get; init; }

        public Color Colour { get; private set; } = Color.Red;

        /// <summary>
        /// Create a convex Polygon with the specified local vertices offset by the (global co-ord) centre
        /// </summary>
        /// <param name="vertices">Array of the vertices of the polygon. Should be local coords</param>
        /// <param name="centre">Origin of the poygon</param>
        public Polygon(Vector2[] vertices, Vector2 centre) 
        {
            Centre = centre;
            SpecialVertices = new VertexPositionColor[vertices.Length];
            Vertices = vertices.Select(vertex => vertex + centre).ToArray();

            for (int i = 0; i < vertices.Length; i++)
            {
                SpecialVertices[i].Position = new Vector3(Vertices[i], 0);
                SpecialVertices[i].Color = Colour;
            }

            Indices = TriangulateArcVertices(vertices.Length);
        }

        /// <summary>
        /// Create a convex Polygon with the specified global vertices
        /// </summary>
        /// <param name="vertices">Array of the vertices of the polygon. Should be global coords</param>
        public Polygon(Vector2[] vertices)
        {
            Centre = Vector2.Zero;
            SpecialVertices = new VertexPositionColor[vertices.Length];
            Vertices = vertices;

            for (int i = 0; i < vertices.Length; i++)
            {
                SpecialVertices[i].Position = new Vector3(vertices[i], 0);
                SpecialVertices[i].Color = Colour;
            }

            Indices = TriangulateArcVertices(vertices.Length);
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void Draw()
        {
            foreach (EffectPass effectPass in Globals.Globals.BasicEffect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                Globals.Globals.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, 
                    SpecialVertices, 0, SpecialVertices.Length, Indices, 0, SpecialVertices.Length - 2);
            }
        }

        public void ChangeColour(Color newColour)
        {
            Colour = newColour;
            SpecialVertices.Select(vertex => vertex.Color = newColour);
        }

        // vertices[0] should be the origin
        private static short[] TriangulateArcVertices(int vertexCount)
        {
            // with arcs, (0,0) is always in a triangle
            // the number of triangles is also vertexCount - 2
            short[] indices = new short[3 * (vertexCount - 2)];

            for (int i = 0; i < vertexCount - 2; i++)
            {
                indices[i * 3] = 0;
                indices[i * 3 + 1] = (short)(i + 1);
                indices[i * 3 + 2] = (short)(i + 2);
            }

            return indices;
        }

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
    }
}
