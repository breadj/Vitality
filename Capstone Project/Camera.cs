using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Capstone_Project
{
    public class Camera
    {
        public Rectangle Bounds { get; private set; }
        public Vector2 Position { get; private set; }   // position of centre of screen
        public float Zoom { get; private set; }

        // how far horizontally (X) and vertically (Y) a player can move before the camera follows (in px)
        private Point allowedPlayerDeviation = new Point(384, 256);


        // honestly, I'm not too sure what either of these do, but everywhere says I need them and to do them like this
        public Matrix TransformMatrix { 
            get
            {   // no need for rotation so it's not included
                return Matrix.CreateTranslation(new(-Position.X, -Position.Y, 0)) *
                    Matrix.CreateScale(Zoom) *
                    Matrix.CreateTranslation(new(Bounds.Width / 2f, Bounds.Height / 2f, 0));
            } 
        } 
        /*public Rectangle VisibleArea
        {
            get
            {
                
            }
        }*/

        public Camera(Viewport viewport, Vector2 playerPosition)
        {
            Bounds = viewport.Bounds;
            //Position = PtoV(viewport.Bounds.Center);
            Position = playerPosition;
            Zoom = 1f;
        }

        public void Update(Vector2 playerPosition)
        {
            PlayerDeviation(playerPosition);
        }

        public Vector2 WorldToScreen(Vector2 worldPosition) 
        {
            return Vector2.Transform(worldPosition, TransformMatrix);
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(TransformMatrix));
        }

        private void PlayerDeviation(Vector2 playerPosition) 
        {
            Vector2 playerDeviation = playerPosition - Position;    // how far from the centre of the camera the player is
            Vector2 tempPosition = Position;                        // I dunno why I need this but I do. Something about CS1612 and non-variables

            float deviationX = Math.Abs(playerDeviation.X) - allowedPlayerDeviation.X;
            if (deviationX > 0)                                     // if the player has gone out of the allowed deviation range
                tempPosition.X += playerDeviation.X >= 0 ? deviationX : -deviationX;        // tempPosition moves towards the player by the amount the player was out of the allowed deviation range

            float deviationY = Math.Abs(playerDeviation.Y) - allowedPlayerDeviation.Y;
            if (deviationY > 0)
                tempPosition.Y += playerDeviation.Y >= 0 ? deviationY : -deviationY;

            Position = tempPosition;                                // updating the actual position
        }
    }
}
