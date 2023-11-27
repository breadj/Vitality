using static Capstone_Project.Globals.Globals;
using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Capstone_Project.GameObjects
{
    public abstract class Entity : Interfaces.IDrawable, IUpdatable, ICollidable, IMovable
    {
        public bool Visible { get; set; } = true;
        public Subsprite Subsprite { get; init; }
        public Rectangle Destination => Hitbox;
        public Vector2 Origin { get; init; }        // Entities have their positions as the centre of the sprite
        public float Layer { get; set; } = 0.01f;

        public bool Active { get; set; } = true;
        public Rectangle Hitbox => new Rectangle((int)(Position.X - (Size / 2f)), (int)(Position.Y - (Size / 2f)), Size, Size);
        public bool IsCircle { get; init; } = true; // Entities are by default Circles (in terms of collision)
        public float Radius => Size / 2f;

        public Vector2 Position { get; protected set; }
        public Vector2 Direction { get; protected set; } = Vector2.Zero;
        public Vector2 Velocity { get; protected set; } = Vector2.Zero;
        public int Speed { get; protected set; } = 0;

        public int Size { get; init; }              // since Entities are all square, only one axis of 'Size' is needed
        public bool Dead { get; set; } = false;

        protected Vector2 lastPosition { get; set; } = Vector2.Zero;

        public Entity(Subsprite subsprite, Vector2 position, int size = 0, int speed = 0) 
        {
            Subsprite = subsprite;
            //Origin = Subsprite.Source.Size.ToVector2() / 2f;
            Origin = Vector2.Zero;
            Position = position;
            Speed = speed;

            Size = size;
        }

        public virtual void Update(GameTime gameTime)
        {
            // sets lastPosition to Position before Position is changed
            lastPosition = Position;

            Velocity = Direction * Speed;
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public virtual void Draw()
        {
            if (Visible && !Destination.IsEmpty)
                spriteBatch.Draw(Subsprite.SpriteSheet, Destination, Subsprite.Source, Color.White, 0f, Origin, SpriteEffects.None, Layer);
        }

        #region Collision Stuff
        public virtual CollisionDetails CollidesWith(ICollidable other)
        {
            CollisionDetails details = new CollisionDetails();
            details.From = this;
            details.Against = other;
            details.Intersection = Rectangle.Intersect(Hitbox, other.Hitbox);

            // if there is no intersection between the Hitboxes
            if (details.Intersection.IsEmpty)
                return details;
            // continues if there is a tangible intersection
            
            // if both are square
            if (!IsCircle && !other.IsCircle)
            {
                details.Type = CollisionType.RectOnRect;
                return details;
            }

            // knowing that at least one is a circle:
            if (!IsCircle)                              // if this is the square
                return CircOnRect(other, this, details);
            if (!other.IsCircle)                        // if other is the square
                return CircOnRect(this, other, details);
            return CircOnCirc(this, other, details);    // finally, knowing that neither are squares (aka: both are circles)
        }

        protected static CollisionDetails CircOnCirc(ICollidable a, ICollidable b, CollisionDetails details)
        {
            details.CornerCollision = true;

            float intersectionDepth = a.Radius + b.Radius - (a.Hitbox.Center - b.Hitbox.Center).ToVector2().Length();
            if (intersectionDepth > 0)
            {
                details.IntersectionDepth = intersectionDepth;
                details.Type = CollisionType.CircOnCirc;
            }

            return details;
        }

        // checks if the square and circle collide by creating localised versions of each around the square's center being at the origin
        // note: localised square has side lengths (square.Radius, square.Radius) and top-left (0, 0)
        protected static CollisionDetails CircOnRect(ICollidable circle, ICollidable square, CollisionDetails details)
        {
            details.From = circle;
            details.Against = square;

            Vector2 localCirclePos = new Vector2(MathF.Abs(circle.Hitbox.Center.X - square.Hitbox.Center.X), MathF.Abs(circle.Hitbox.Center.Y - square.Hitbox.Center.Y));

            // testing to see if any of the corners of the square are in the circle
            float intersectionDepth = circle.Radius - (localCirclePos - new Vector2(square.Radius)).Length();
            if (intersectionDepth > 0)
            {
                details.IntersectionDepth = intersectionDepth;
                details.Type = CollisionType.CircOnRect;

                // knowing there's already intersection, checks if the intersection is on the corner or just via a cardinal point
                if (localCirclePos.X > square.Radius && localCirclePos.Y > square.Radius)
                    details.CornerCollision = true;

                return details;
            }

            // check if the local circle's cardinal points are within the bounds of the local square
            if ((localCirclePos.X - circle.Radius < square.Radius && localCirclePos.Y < square.Radius)
                || (localCirclePos.Y - circle.Radius < square.Radius && localCirclePos.X < square.Radius))
            {
                details.Type = CollisionType.CircOnRect;
                details.CornerCollision = false;    // not needed because it's false by default, but it doesn't hurt to show it explicitly

                return details;
            }

            return details;
        }
        #endregion

        public void ClampToMap(Rectangle mapBounds)
        {
            // checks if the Hitbox Rectangle is fully contained within the mapBounds Rectangle
            if (mapBounds.Contains(Hitbox))
                return;

            Vector2 clampedPos = Position;
            clampedPos.X = MathHelper.Clamp(Position.X, mapBounds.Left + (Size / 2f), mapBounds.Right - (Size / 2f));
            clampedPos.Y = MathHelper.Clamp(Position.Y, mapBounds.Top + (Size / 2f), mapBounds.Bottom - (Size / 2f));

            Position = clampedPos;
        }
    }
}
