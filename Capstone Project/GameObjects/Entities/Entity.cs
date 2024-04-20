using static Capstone_Project.Globals.Globals;
using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Capstone_Project.CollisionStuff;
using Capstone_Project.CollisionStuff.CollisionShapes;
using System.Diagnostics;

namespace Capstone_Project.GameObjects.Entities
{
    public abstract class Entity : ITexturable, IUpdatable, ICollidable, IMovable, IKillable
    {
        #region Default Attributes
        public static readonly bool DefaultVisibility = true;
        public static readonly string DefaultSpriteName = "MissingTexture";
        public static readonly Color DefaultColour = Color.White;
        public static readonly float DefaultRotation = 0f;                  // facing downwards
        public static readonly float DefaultLayer = 0.01f;

        public static readonly bool DefaultActivity = true;

        public static readonly Vector2 DefaultPosition = Vector2.One;       // default position is (1, 1) - shouldn't be used
        public static readonly Vector2 DefaultDirection = Vector2.Zero;
        public static readonly Vector2 DefaultVelocity = Vector2.Zero;
        public static readonly float DefaultSpeed = 0f;

        public static readonly int DefaultSize = 100;
        public static readonly bool DefaultDeathFlag = false;
        #endregion Default Attributes

        public uint ID { get; init; }

        public bool Visible { get; set; } = true;
        public string SpriteName { get; init; }
        public Subsprite Subsprite { get; init; }
        public Rectangle Destination => new Rectangle((int)Position.X, (int)Position.Y, Size, Size);
        public Color Colour { get; set; } = Color.White;
        public float Rotation { get; protected set; } = 0f;
        public Vector2 Origin { get; init; }        // Entities have their positions as the centre of the sprite
        public float Layer { get; set; } = 0.01f;

        public bool Active { get; set; } = true;
        public CShape Collider { get; init; }

        public Vector2 Position { get; protected set; }
        public Vector2 Direction { get; protected set; } = Vector2.Zero;
        public Vector2 Velocity { get; protected set; } = Vector2.Zero;
        public float Speed { get; protected set; } = 0;

        public int Size { get; init; }              // since Entities are all square, only one axis of 'Size' is needed
        public bool Dead { get; protected set; } = false;
        public IAttacker Killer { get; protected set; } = null;

        protected Vector2 lastPosition { get; set; } = Vector2.Zero;

        public Entity(uint id, bool? visible = null, string spriteName = null, Color? colour = null, float? rotation = null, float? layer = null, 
            bool? active = null, Vector2? position = null, Vector2? direction = null, Vector2? velocity = null, float? speed = null, 
            int? size = null, bool? dead = null)
        {
            ID = id;

            Visible = visible ?? DefaultVisibility;
            SpriteName = spriteName ?? DefaultSpriteName;
            Colour = colour ?? DefaultColour;
            Rotation = rotation ?? DefaultRotation;
            Layer = layer ?? DefaultLayer;

            Active = active ?? DefaultActivity;

            Position = position ?? DefaultPosition;
            Direction = direction ?? DefaultDirection;
            Velocity = velocity ?? DefaultVelocity;
            Speed = speed ?? DefaultSpeed;
            
            Size = size ?? DefaultSize;
            Dead = dead ?? DefaultDeathFlag;

            // special things
            Subsprite = LoadedSprites[SpriteName];
            Origin = Subsprite.Source.Size.ToVector2() / 2f;

            Collider = new CCircle(Position, Size / 2f, true);
        }

        /*public Entity(string spriteName, Subsprite subsprite, Vector2 position, int size = 1, float speed = 0)
        {
            SpriteName = spriteName;
            Subsprite = subsprite;
            //Origin = Subsprite.Source.Size.ToVector2() / 2f;
            Origin = subsprite.Source.Size.ToVector2() / 2f;

            Collider = new CCircle(position, size / 2f, true);

            Position = position;
            Speed = speed;

            Size = size;
        }*/

        public virtual void Update(GameTime gameTime)
        {
            if (!Active) return;

            // sets lastPosition to Position before Position is changed
            lastPosition = Position;

            if (Collider.Dynamic)
                Collider.MoveTo(Position);

            Velocity = Direction * Speed;
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public virtual void Draw()
        {
            if (Visible && !Destination.IsEmpty)
                spriteBatch.Draw(Subsprite.SpriteSheet, Destination, Subsprite.Source, Colour, Rotation, Origin, SpriteEffects.None, Layer);
        }

        public virtual void Kill()
        {
            Dead = true;
        }

        #region Collision Stuff
        
        public virtual bool CollidesWith(ICollidable other, out CollisionDetails cd)
        {
            cd = new CollisionDetails();
            if (!Active || !other.Active)
                return false;

            return Collision.Colliding(Collider, other.Collider, out cd);
        }

        #endregion

        public void ClampToMap(Rectangle mapBounds)
        {
            // checks if the Hitbox Rectangle is fully contained within the mapBounds Rectangle
            if (mapBounds.Contains(Collider.BoundingBox))
                return;

            Vector2 clampedPos = Position;
            clampedPos.X = MathHelper.Clamp(Position.X, mapBounds.Left + Size / 2f, mapBounds.Right - Size / 2f);
            clampedPos.Y = MathHelper.Clamp(Position.Y, mapBounds.Top + Size / 2f, mapBounds.Bottom - Size / 2f);

            Position = clampedPos;
        }
    }
}
