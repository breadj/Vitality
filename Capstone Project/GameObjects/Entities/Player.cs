using static Capstone_Project.Globals.Globals;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.CollisionStuff;
using System.Linq;
using Capstone_Project.Fundamentals.DrawableShapes;
using System.Diagnostics;
using Capstone_Project.Globals;

namespace Capstone_Project.GameObjects.Entities
{
    public class Player : Agent
    {
        #region Default Attributes
        public static readonly new string DefaultSpriteName = "Player";

        public static readonly int DefaultMaxStamina = 20;
        #endregion Default Attributes

        public Agent LockOnTarget { get; private set; } = null;
        public DCircle LockOnHighlight { get; private set; } = null;

        public int MaxStamina { get; private set; }
        public float Stamina
        {
            get => stamina;
            set => stamina = value < 0 ? 0 : value > MaxStamina ? MaxStamina : value;
        }               // can only be between 0-MaxStamina
        private float stamina = 0f;
        public Timer StaminaRegenCD { get; private set; } = new Timer(1.5f);
        public float StaminaRegenAmount { get; private set; } = 2f;     // how much stamina is recovered per second

        #region Stamina Costs
        private readonly int dashStaminaCost = 5;
        private readonly int attackStaminaCost = 7;
        #endregion

        public Player(bool? visible = null, string spriteName = null, Color? colour = null, float? rotation = null, float? layer = null,
            bool? active = null, Vector2? position = null, Vector2? direction = null, Vector2? velocity = null, float? speed = null,
            int? size = null, bool? dead = null, Comparer<(ICollidable, CollisionDetails)> collisionsComparer = null,
            Vector2? orientation = null, float? leakPercentage = null, int? maxVitality = null, int? vitality = null, float? defence = null,
            float? damage = null, float? windupTime = null, float? lingerTime = null, float? cooldownTime = null, float? attackRange = null,
            float? dashTime = null, float? dashSpeedModifier = null, float? invincibilityTime = null,
            int? maxStamina = null)
            : base(0, visible, spriteName ?? DefaultSpriteName, colour, rotation, layer, active, position, direction, velocity, speed,
                  size, dead, collisionsComparer, orientation, leakPercentage, maxVitality, vitality, defence, damage, windupTime, 
                  lingerTime, cooldownTime, attackRange, dashTime, dashSpeedModifier, invincibilityTime)
        {
            MaxStamina = maxStamina ?? DefaultMaxStamina;
            Stamina = MaxStamina;
        }
        
        public override void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            Attack();
            Look();
            Move();

            Invincibility.Update(gameTime);
        }

        private readonly Rectangle staminaBar = new Rectangle(0, 0, 1, 30);
        private readonly Vector2 vitalityIconLocation = new Vector2(60f, ScreenBounds.Y - 60f);     // bottom left of the screen
        private DCircle vitalityIcon = new DCircle(Vector2.Zero, 60f, Color.Red, 0.99f, true);
        public override void Draw()
        {
            base.Draw();

            if (Strike.Cooldown.Active)
                spriteBatch.Draw(Pixel, new Rectangle(Collider.BoundingBox.Left, Collider.BoundingBox.Bottom, 
                    (int)(Size * Strike.Cooldown.Percentage), 10), null, new Color(Color.DarkGray, 0.9f), 0f, 
                    Vector2.Zero, SpriteEffects.None, Strike.Layer);

            // Vitality icon stuff
            vitalityIcon.MoveTo(Game1.Camera.ScreenToWorld(vitalityIconLocation));
            vitalityIcon.Draw();
            vitalityIcon.DrawOutline(Color.Black);

            Vector2 vitalityIconTextLocation = vitalityIcon.Centre - (3f * DebugFont.MeasureString(Vitality.ToString()) / 2f);
            spriteBatch.DrawString(DebugFont, Vitality.ToString(), vitalityIconTextLocation, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);

            // Stamina bar stuff
            Rectangle curStaminaBar = staminaBar with { Location = Game1.Camera.ScreenToWorld(Vector2.Zero).ToPoint().Round(), Width = (int)(30 * Stamina) };   // 30 pixels
            spriteBatch.Draw(Pixel, curStaminaBar, null, Color.DarkOliveGreen, 0f, Vector2.Zero, SpriteEffects.None, 0.99f);
            spriteBatch.DrawString(DebugFont, $"{(int)Stamina}/{MaxStamina}", curStaminaBar.Location.ToVector2(), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);

            LockOnHighlight?.DrawOutline(Color.White);

            //spriteBatch.DrawString(DebugFont, $"MV: {MaxVitality}\nV: {Vitality}", Position, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0.6f);
            //spriteBatch.Draw(BLANK, PathCollider, null, new Color(Color.MediumPurple, 0.5f), 0f, Vector2.Zero, SpriteEffects.None, 0.9f);
            //spriteBatch.DrawString(DebugFont, Position.ToString(), Position, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
            //spriteBatch.Draw(BLANK, new Rectangle(Position.ToPoint(), new Point(Size)), null, new Color(Color.Pink, 0.5f), MathHelper.PiOver4, BLANK.Bounds.Size.ToVector2() / 2f, SpriteEffects.None, 0.05f);
            //spriteBatch.Draw(BLANK, Collider.BoundingBox, null, new Color(Color.Blue, 0.5f), 0f, Vector2.Zero, SpriteEffects.None, 0.9f);
        }

        public override void Move()
        {
            if (Strike.Lock)
                return;

            bool bufActExists = !Game1.Controls.ActionBuffer.IsEmpty;
            bool prevDashActive = Dash.Active;

            if (bufActExists && !PerformingAction && Game1.Controls.ActionBuffer.Peek().Name == "dash")
            {
                Game1.Controls.ActionBuffer.Remove();

                if (Stamina > dashStaminaCost)
                {
                    if (Direction == Vector2.Zero)
                        Direction = Orientation;

                    Dash.Start();
                    Stamina -= dashStaminaCost;
                    StaminaRegenCD.Start();
                }
            }

            if (Dash.Active)
            {
                Dash.Update(gameTime);

                Speed = Dash.Speed;
                Direction = Dash.Direction;
            }

            if (!Dash.Active)
            {
                if (prevDashActive)     // if this is the first frame since ending a dash, revert speed
                    Speed = BaseSpeed;

                Direction = Movement(Game1.Controls.ActivatedActions);
            }

            if (StaminaRegenCD.Active)
            {
                StaminaRegenCD.Update(gameTime);
            }
            else if (StaminaRegenCD.Done && Stamina < MaxStamina)
            {
                Stamina += StaminaRegenAmount * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            base.Move();
        }

        public override void Look()
        {
            Vector2 worldMousePos = Game1.Camera.ScreenToWorld(Game1.Controls.MousePos.ToVector2());

            if (Game1.Controls.ActivatedActions.Any(action => action.Name == "lock-on"))
            {
                LockOn(worldMousePos);
            }

            if (LockOnTarget != null && LockOnTarget.Dead)
            {
                LockOnTarget = null;
                LockOnHighlight = null;
            }
            LockOnHighlight?.MoveTo(LockOnTarget.Position);

            if (!(Strike.Lock || Dash.Active))
            {
                if (LockOnTarget != null)
                {
                    Orientation = LookAt(LockOnTarget.Position);
                }
                else
                {
                    Orientation = LookAt(worldMousePos);
                }
            }

            base.Look();
        }

        public void LockOn(Vector2 worldMousePos)
        {
            // toggles lock-on
            if (LockOnTarget != null)
            {
                LockOnTarget = null;
                LockOnHighlight = null;
            }
            else
            {
                Agent closestAgent = (Agent)Game1.SimulatedEntities.Where(entity => entity is Agent && entity is not Player).MinBy(agent => (agent.Position - worldMousePos).LengthSquared());

                // lock-on target needs to be at least 200px away from the mouse pos
                if (closestAgent != null && (worldMousePos - closestAgent.Position).LengthSquared() <= 200 * 200)
                {
                    LockOnTarget = closestAgent;
                    LockOnHighlight = new DCircle(closestAgent.Position, closestAgent.Size / 2f * 1.2f, Color.White, 0.02f);
                }
            }
        }

        public override void Attack()
        {
            bool bufActExists = !Game1.Controls.ActionBuffer.IsEmpty;

            if (Strike.Active || Strike.OnCD)
                Strike.Update(gameTime);
            else if (bufActExists && !PerformingAction && Game1.Controls.ActionBuffer.Peek().Name == "attack")
            {
                Game1.Controls.ActionBuffer.Remove();

                if (Stamina > attackStaminaCost)
                {
                    Swing();
                    Stamina -= attackStaminaCost;
                    StaminaRegenCD.Start();
                }
            }
        }

        private static Vector2 Movement(List<Input.Action> relevantActions)
        {
            Vector2 tempDirection = Vector2.Zero;
            foreach (var action in relevantActions)
            {
                switch (action.Name)
                {
                    case "up":
                        tempDirection.Y -= 1;
                        break;
                    case "down":
                        tempDirection.Y += 1;
                        break;
                    case "left":
                        tempDirection.X -= 1;
                        break;
                    case "right":
                        tempDirection.X += 1;
                        break;
                }
            }

            if (tempDirection == Vector2.Zero)
                return Vector2.Zero;
            return Vector2.Normalize(tempDirection);
        }

        // returns normalised Vector2 of the direction from this to the target
        private Vector2 LookAt(Vector2 target)
        {
            return Vector2.Normalize(target - Position);
        }
    }
}
