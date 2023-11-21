using static Capstone_Project.Globals.Globals;
using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.MapStuff;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using System;

namespace Capstone_Project.GameObjects
{
    public abstract class Entity : Interfaces.IDrawable, IUpdatable, ICollidable, IMovable
    {
        public bool Visible { get; set; } = true;
        public Subsprite Subsprite { get; init; }
        public Rectangle Destination => Hitbox;
        public Vector2 Origin => Subsprite.Source.Size.ToVector2() / 2f;        // Entities have their positions as the centre of the sprite
        public float Layer { get; set; } = 0.01f;

        public bool Active { get; set; } = true;

        public Rectangle Hitbox => new Rectangle((int)(Position.X - (Size / 2f)), (int)(Position.Y - (Size / 2f)), Size, Size);
        public Vector2 Position { get; protected set; }
        public Vector2 Direction { get; protected set; } = Vector2.Zero;
        public Vector2 Velocity { get; protected set; } = Vector2.Zero;
        public int Speed { get; protected set; } = 0;

        public int Size { get; init; }
        public bool Dead { get; set; } = false;

        public Entity(Subsprite subsprite, Vector2 position, int size = 0) 
        {
            Subsprite = subsprite;

            Position = position;
            
            Size = size;
        }

        public virtual void Update(GameTime gameTime)
        {
            Velocity = Direction * Speed;
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public virtual void Draw()
        {
            if (Visible && !Destination.IsEmpty)
                spriteBatch.Draw(Subsprite.SpriteSheet, Destination, Subsprite.Source, Color.White, 0f, Origin, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, Layer);
        }

        public void ClampToMap(Rectangle mapBounds)
        {
            // checks if the Hitbox Rectangle is fully contained within the mapBounds Rectangle
            if (mapBounds.Contains(Hitbox))
                return;

            Vector2 offset = Vector2.Zero;

            // I'm not too sure why I need to add the '+1', but if I don't, the Entity will be able to go 1px past the edges in the top-left
            if (mapBounds.Left > Hitbox.Left)
                offset.X = mapBounds.Left - Hitbox.Left + 1;
            else if (mapBounds.Right < Hitbox.Right)
                offset.X = mapBounds.Right - Hitbox.Right;

            if (mapBounds.Top > Hitbox.Top)
                offset.Y = mapBounds.Top - Hitbox.Top + 1;
            else if (mapBounds.Bottom < Hitbox.Bottom)
                offset.Y = mapBounds.Bottom - Hitbox.Bottom;

            Position += offset;
        }

        /*public virtual void HandleCollision(Entity other, GameTime gameTime)
        {
            CollisionDetails cd = CollidesWith(other);
            if (cd)
                CollideWith(cd, other, gameTime);       // handles collision with both Entities, so no need to call it on 'other'
        }

        public virtual void HandleCollision(Tile tile, GameTime gameTime)
        {
            if (!tile.IsWall)
                return;

            CollisionDetails cd = CollidesWith(tile);
            if (cd)
                CollideWith(cd, tile, gameTime);
        }

        public virtual CollisionDetails CollidesWith(Entity other)
        {
            return Hitbox.Intersects(other.Hitbox);
        }

        public virtual CollisionDetails CollidesWith(Tile tile)
        {
            return Hitbox.Intersects(tile.Hitbox);
        }

        public virtual void CollideWith(CollisionDetails cd, Entity other, GameTime gameTime)
        {
            
        }

        public virtual void CollideWith(CollisionDetails cd, Tile tile, GameTime gameTime)
        {
            Func<float, int> sign = x => x < 0 ? -1 : 1;

            // if the Hitbox is a CircleHitbox but only colliding via cardinal directions, or it's a RectangleHitbox
            if (!cd.CornerCollision || Hitbox is RectangleHitbox)
            {
                Vector2 displacement = Hitbox.Centre - tile.Hitbox.Centre;

                float absDiffX = MathF.Abs(displacement.X);
                float absDiffY = MathF.Abs(displacement.Y);

                // if the absolute difference has X > Y, then that means it's an East-West-wise collision (on x-axis) and vice versa
                // (displacement.X < 0 ? -1 : 1) is just retrieving the sign of X to multiply by, same with Y
                float newX = absDiffX < absDiffY ? Position.X : Position.X + (sign(displacement.X) * cd.Intersection.Width);
                float newY = absDiffY < absDiffX ? Position.Y : Position.Y + (sign(displacement.Y) * cd.Intersection.Height);

                Position = new Vector2(newX, newY);

                // Direction and Velocity updated for assurance
                Direction = Vector2.Normalize(lastPosition - Position);
                Velocity = (lastPosition - Position) / (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (Hitbox is CircleHitbox ch && cd.CornerCollision)
            {
                Vector2 displacement = ch.Centre - tile.Hitbox.Centre;

                // finding which corner of the rectangle intersects; (-x, -y) = top-left, (-x, +y) = bottom-left, et cetera
                int signX = sign(displacement.X);
                int signY = sign(displacement.Y);

                // push initialised as a bottom-right-facing (+x, +y) version of Direction, then rotates it by signX and signY.
                // Since it's already normalised (Direction should always be normalised), multiplying by IntersectionDepth will
                // push it away from the Tile enough to escape collision
                Vector2 pushDirection = new Vector2(Math.Abs(Direction.X) * signX, Math.Abs(Direction.Y) * signY) * cd.IntersectionDepth;
                Position += pushDirection;
            }
        }*/
    }
}
