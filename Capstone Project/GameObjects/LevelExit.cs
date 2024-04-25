using Capstone_Project.CollisionStuff;
using Capstone_Project.CollisionStuff.CollisionShapes;
using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.MapStuff.Parser;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace Capstone_Project.GameObjects
{
    public class LevelExit : ICollidable, ITexturable
    {
        public bool Active { get; set; } = true;
        public CShape Collider { get; init; }

        public string SpriteName { get; init; }
        public Subsprite Subsprite { get; init; }
        public Rectangle Destination { get; init; }
        public Color Colour { get; } = Color.White;
        public float Rotation { get; init; }
        public Vector2 Origin { get; init; }
        public bool Visible { get; set; } = true;
        public float Layer { get; init; } = 0.0011f;      // just above the Tile draw layer

        public Point Tile { get; init; }
        public Vector2 Position { get; init; }
        public string DestinationLevel { get; init; }
        public Point? DestinationTile { get; init; }

        public bool SpawnBlocked { get; set; } = false;

        public LevelExit(Point tile, Vector2 position, string destinationLevel, Point? destinationTile, string spriteName, Subsprite subsprite, Rectangle destination, CShape collider, float rotation, bool active = true)
        {
            Active = active;
            Collider = collider;

            SpriteName = spriteName;
            Subsprite = subsprite;
            Destination = destination;
            Rotation = rotation;

            Tile = tile;
            Position = position;
            DestinationLevel = destinationLevel;
            DestinationTile = destinationTile;

            Origin = Subsprite.Source.Center.ToVector2();
        }

        public bool CollidesWith(ICollidable other, out CollisionDetails cd)
        {
            cd = new CollisionDetails();
            return Active && other.Active && Collision.Colliding(Collider, other.Collider, out cd);
        }

        public void Draw()
        {
            Globals.Globals.spriteBatch.Draw(Subsprite.SpriteSheet, Destination, Subsprite.Source, Colour, Rotation, Origin, 
                Microsoft.Xna.Framework.Graphics.SpriteEffects.None, Layer);

            /*if (SpawnBlocked)
                Globals.Globals.spriteBatch.Draw(Globals.Globals.Pixel, Destination, null, Color.Purple, 0f, Vector2.Zero, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0.99f);*/
            /*Globals.Globals.spriteBatch.Draw(Globals.Globals.Pixel, Destination, null, new Color(Color.Red, 0.5f), 0f, Vector2.Zero,
                Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0.9f);*/
            /*Globals.Globals.spriteBatch.Draw(Globals.Globals.Pixel, Collider.BoundingBox, null, Color.Red, 0f, Vector2.Zero,
                Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0.9f);*/
        }
    }
}
