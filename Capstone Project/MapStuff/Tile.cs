﻿using static Capstone_Project.Globals.Globals;
using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Capstone_Project.CollisionStuff;
using Capstone_Project.CollisionStuff.CollisionShapes;
using Capstone_Project.Fundamentals.DrawableShapes;
using System.Diagnostics;

namespace Capstone_Project.MapStuff
{
    public class Tile : ITexturable, ICollidable
    {
        public bool Visible { get; set; } = true;
        public string SpriteName { get; } = string.Empty;
        public Subsprite Subsprite { get; init; }
        public Rectangle Destination { get; init; }
        public Color Colour { get; set; } = Color.White;
        public float Rotation => 0f;
        public Vector2 Origin { get; init; }            // Tiles have their positions as the top-left of their sprite
        public float Layer { get; set; } = 0.001f;

        public bool Active { get; set; } = false;
        public CShape Collider { get; init; }

        public Point Position { get; init; }
        private int size { get; init; }


        public Tile(Subsprite subsprite, Point position, int size, bool isWall = false)
        {
            Subsprite = subsprite;
            Destination = new Rectangle(position, new Point(size));
            Origin = Vector2.Zero;

            Active = isWall;
            Collider = new CRectangle(Destination.Center.ToVector2(), (size, size), false);

            Position = position;
            this.size = size;
        }

        public void Draw()
        {
            // assuming the destination Rectangle is the same as the Hitbox BoundingBox
            spriteBatch.Draw(Subsprite.SpriteSheet, Destination, Subsprite.Source, Color.White, 
                Rotation, Origin, SpriteEffects.None, Layer);

            //spriteBatch.DrawString(Globals.Globals.DebugFont, IsWall.ToString(), Position.ToVector2(), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.2f);
            /*if (Active)
                spriteBatch.Draw(BLANK, Hitbox, null, new Color(Color.Pink, 0.4f), 0f, Vector2.Zero, SpriteEffects.None, 0.2f);*/
        }

        // this shouldn't actually ever have to be called, since it will always be 'other' that checks and handles collision with a Tile
        public bool CollidesWith(ICollidable other, out CollisionDetails cd)
        {
            throw new System.NotImplementedException();
        }
    }
}
