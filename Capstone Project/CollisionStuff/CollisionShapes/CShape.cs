﻿using Microsoft.Xna.Framework;

namespace Capstone_Project.CollisionStuff.CollisionShapes
{
    public abstract class CShape
    {
        public bool Dynamic { get; init; }
        public Rectangle BoundingBox { get; protected set; }
        public Vector2 Centre { get; protected set; }

        public CShape(Vector2 centre)
        {
            Centre = centre;
        }

        public virtual void MoveTo(Vector2 target, float rotation = 0f)
        {
            if (!Dynamic)
                throw new System.Exception($"Cannot move a {this.GetType()} with Dynamic set to false");

            Centre = target;
            BoundingBox = GenerateBoundingBox();
        }

        protected abstract Rectangle GenerateBoundingBox();
    }
}
