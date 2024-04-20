using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Capstone_Project.Fundamentals
{
    public class Camera
    {
        public Rectangle Bounds { get; init; }
        public Vector2 Position { get; private set; }   // position of centre of screen
        public float Zoom { get; private set; }

        public Rectangle VisibleArea { get; private set; }
        public Rectangle SimulationArea { get; private set; }       // 1.5x the width and height of VisibleArea

        // how far horizontally (X) and vertically (Y) a player can move before the camera follows (in px)
        //private Point allowedPlayerDeviation = new Point(220, 150);
        private Point allowedPlayerDeviation = new Point(150, 110);
        private bool moved = false;


        // honestly, I'm not too sure what either of these do, but everywhere says I need them and to do them like this
        public Matrix TransformMatrix
        {
            get
            {   // no need for rotation so it's not included
                return Matrix.CreateTranslation(new(-Position.X, -Position.Y, 0)) *
                    Matrix.CreateScale(Zoom) *
                    Matrix.CreateTranslation(new(Bounds.Width / 2f, Bounds.Height / 2f, 0));
            }
        }

        public Camera(Viewport viewport)
        {
            Bounds = viewport.Bounds;
            //Position = PtoV(viewport.Bounds.Center);
            Position = viewport.Bounds.Center.ToVector2();
            Zoom = 1f;

            // Zoom will always be initialised to 1 so no need to make zoomedBounds
            /*VisibleArea = new Rectangle((int)(playerPosition.X - (viewport.Bounds.Width / 2f)), (int)(playerPosition.Y - (viewport.Bounds.Height / 2f)), 
                viewport.Bounds.Width, viewport.Bounds.Height);
            SimulationArea = new Rectangle((int)(playerPosition.X - (1.5f * viewport.Bounds.Width / 2f)), (int)(playerPosition.Y - (viewport.Bounds.Height / 2f)), 
                (int)(1.5f * viewport.Bounds.Width), (int)(1.5f * viewport.Bounds.Height));*/
        }

        public void Update(Vector2 playerPosition)
        {
            PlayerDeviation(playerPosition);

            if (moved)
            {
                UpdateVisibleArea();
                UpdateSimulationArea();
            }
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, TransformMatrix);
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(TransformMatrix));
        }

        public void UpdateVisibleArea()
        {
            Vector2 zoomedBounds = new Vector2(Bounds.Width / Zoom, Bounds.Height / Zoom);
            VisibleArea = new Rectangle((Position - zoomedBounds / 2f).ToPoint(), zoomedBounds.ToPoint());
        }

        public void UpdateSimulationArea()
        {
            // simulation bounds are 1.5x wider and higher than VisibleArea
            Vector2 simBounds = new Vector2(1.5f * Bounds.Width / Zoom, 1.5f * Bounds.Height / Zoom);
            SimulationArea = new Rectangle((Position - simBounds / 2).ToPoint(), simBounds.ToPoint());
        }

        private void PlayerDeviation(Vector2 playerPosition)
        {
            Vector2 playerDeviation = playerPosition - Position;    // how far from the centre of the camera the player is
            if (playerDeviation == Vector2.Zero)                    // if the player hasn't moved then no need to do all the calculations
            {
                moved = false;
                return;
            }
            Vector2 tempPosition = Position;                        // I dunno why I need this but I do. Something about CS1612 and non-variables

            float deviationX = Math.Abs(playerDeviation.X) - allowedPlayerDeviation.X;
            if (deviationX > 0)                                     // if the player has gone out of the allowed deviation range
                tempPosition.X += playerDeviation.X >= 0 ? deviationX : -deviationX;        // tempPosition moves towards the player by the amount the player was out of the allowed deviation range

            float deviationY = Math.Abs(playerDeviation.Y) - allowedPlayerDeviation.Y;
            if (deviationY > 0)
                tempPosition.Y += playerDeviation.Y >= 0 ? deviationY : -deviationY;

            if (Position == tempPosition)                           // if the player hasn't moved out of the allowed deviation zone, mark camera as unmoved
            {
                moved = false;
                return;
            }
            moved = true;
            Position = tempPosition;                                // updating the actual position
        }
    }
}
