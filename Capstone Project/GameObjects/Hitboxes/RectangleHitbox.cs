using static Capstone_Project.Globals.Utility;
using Microsoft.Xna.Framework;
using System;

namespace Capstone_Project.GameObjects.Hitboxes
{
    public class RectangleHitbox : IHitbox
    {
        public Vector2 Centre { get; init; }
        public Vector2 Size { get; init; }
        public Rectangle BoundingBox { get; private set; }

        public RectangleHitbox(Vector2 centre, Vector2 size)
        {
            BoundingBox = new Rectangle(VtoP(Centre - Size / 2f), VtoP(Size));
            Centre = centre;
            Size = size;
        }

        public bool Intersects(IHitbox other)
        {
            if (other is CircleHitbox circleHitbox)
                return Intersects(circleHitbox);
            if (other is RectangleHitbox rectangleHitbox)
                return BoundingBox.Intersects(rectangleHitbox.BoundingBox);
            return false;
        }

        private bool Intersects(CircleHitbox circle)
        {
            /// please see Explanations > RectangleHitbox.Intersects(CircleHitbox) to really understand wtf is happening here

            // if the centre of the circle is inside the BoundingBox of the rectangle
            if (BoundingBox.Contains(circle.Centre))
                return true;

            // creates temporary translations of the circle centre and rectangle, with the centre of the original rectangle at (0,0)
            Vector2 localCircleCentre = new Vector2(Math.Abs(circle.Centre.X - Centre.X), Math.Abs(circle.Centre.Y - Centre.Y));
            Rectangle localRectangle = new Rectangle(new(0), VtoP(Size / 2 + new Vector2(circle.Radius)));

            if (!localRectangle.Contains(localCircleCentre))
                return false;

            Rectangle smallerlocalRectangle = new Rectangle(VtoP(Size / 2), new((int)circle.Radius));
            if (!smallerlocalRectangle.Contains(localCircleCentre))
                return true;

            return (localCircleCentre - PtoV(smallerlocalRectangle.Location)).Length() < circle.Radius;
        }
    }
}
