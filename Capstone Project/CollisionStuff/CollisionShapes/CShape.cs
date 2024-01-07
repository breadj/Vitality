﻿using Microsoft.Xna.Framework;

namespace Capstone_Project.CollisionStuff.CollisionShapes
{
    public abstract class CShape
    {
        public abstract bool Dynamic { get; init; }
        public Rectangle BoundingBox { get; protected set; }
        public Vector2 Centre { get; protected set; }

        public CShape(Vector2 centre)
        {
            Centre = centre;
        }

        public void MoveTo(Vector2 target)
        {
            if (!Dynamic)
                throw new System.Exception("Cannot move a {typeof(this)} with Dynamic = false");

            BoundingBox = GenerateBoundingBox();
        }

        protected abstract Rectangle GenerateBoundingBox();
    }
}
