using static Capstone_Project.Globals.Utility;
using Capstone_Project.MapStuff;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Project.GameObjects.Hitboxes
{
    public class TileHitbox : IHitbox
    {
        private Tile tile { get; init; }
        public Vector2 Centre { get { return PtoV(tile.Position) + Size / 2f; } }
        public Vector2 Size { get; init; }
        public Point PSize { get; init; }
        public Rectangle BoundingBox { get; init; }

        public TileHitbox(Point size)
        {
            PSize = size;
            Size = PtoV(PSize);
            BoundingBox = new Rectangle(tile.Position, size);
        }

        public bool Intersects(IHitbox other)
        {
            if (other is CircleHitbox circleHitbox)
                return Intersects(circleHitbox);
            if (other is RectangleHitbox rectangleHitbox)
                return BoundingBox.Intersects(rectangleHitbox.BoundingBox);
            // if a TileHitbox collides with another TileHitbox... something has gone VERY wrong
            return false;
        }

        private bool Intersects(CircleHitbox circle)
        {
            Vector2 localCirclePos = new Vector2(MathF.Abs(circle.Centre.X - Centre.X), MathF.Abs(circle.Centre.Y - Centre.Y));

            // testing if any of the corners of the Rectangle are in the Circle
            if (((Size / 2) - localCirclePos).LengthSquared() < circle.Radius * circle.Radius)
                return true;

            // check if the local Circle's cardinal points are within the bounds of the local Rectangle
            return localCirclePos.X < PSize.X / 2 && localCirclePos.Y < PSize.Y / 2;        // prefer to use PSize here just for the minor efficiency bonus
        }
    }
}
