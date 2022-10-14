﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NamelessRogue.Engine.Components._3D;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Systems._3DView
{
    ///TODO: COMPLETELY REFACTOR THIS TEST SYSTEM
	class Camera3DSystem : BaseSystem
	{
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        const float rotationSpeed = 0.3f;
        const float moveSpeed = 3f;
        MouseState originalMouseState;
        NamelessGame game;
        Camera3D camera;
        public Camera3DSystem(NamelessGame game)
        {
            originalMouseState = new MouseState(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2,0,
                ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
            this.game = game;
        }
        bool firstTime = true;
        public override void Update(GameTime gameTime, NamelessGame game)
		{
            if (firstTime)
            {
                Mouse.SetPosition(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2);
                firstTime = false;
            }
            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            camera = game.PlayerEntity.GetComponentOfType<Camera3D>();
            ProcessInput(timeDifference);
        }
        bool stopCapturing = false;
        private void ProcessInput(float amount)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.F1))
            {
                stopCapturing = true;
            }
            if (keyState.IsKeyDown(Keys.F2))
            {
                stopCapturing = false;
            }

            if (stopCapturing)
            {
                return;
            }

            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState != originalMouseState)
            {
                float xDifference = currentMouseState.X - originalMouseState.X;
                float yDifference = currentMouseState.Y - originalMouseState.Y;
                camera.LeftrightRot -= rotationSpeed * xDifference * amount;
                camera.UpdownRot -= rotationSpeed * yDifference * amount;
                Mouse.SetPosition(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2);
                UpdateViewMatrix();
            }

            Vector3 moveVector = new Vector3(0, 0, 0);
        

         

        
            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))
                moveVector += new Vector3(1, 0, 0);
            if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))
                moveVector += new Vector3(-1, 0, 0);
            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
                moveVector += new Vector3(0, -1, 0);
            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
                moveVector += new Vector3(0, 1, 0);
            if (keyState.IsKeyDown(Keys.Q))
                moveVector += new Vector3(0, 0, 1);
            if (keyState.IsKeyDown(Keys.Z))
                moveVector += new Vector3(0, 0, -1);
            AddToCameraPosition(moveVector * amount);
        }


        private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            Matrix cameraRotation = Matrix.CreateRotationY(-camera.UpdownRot) * Matrix.CreateRotationZ(camera.LeftrightRot);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            camera.Position += moveSpeed * rotatedVector;
            UpdateViewMatrix();
        }

        private void UpdateViewMatrix()
        {
            var cameraRotationUpDown = Matrix.CreateRotationY(-camera.UpdownRot);
            var rotationLeftRight = Matrix.CreateRotationZ(camera.LeftrightRot);

            Vector3 cameraOriginalTarget = new Vector3(1, 0, 0);
            Vector3 cameraOriginalUpVector = new Vector3(0, 0, 1);

            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotationUpDown * rotationLeftRight);
            Vector3 cameraFinalTarget = camera.Position + cameraRotatedTarget;

            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotationUpDown * rotationLeftRight);

            camera.View = Matrix.CreateLookAt(camera.Position, cameraFinalTarget, cameraRotatedUpVector);
        }
    }
}
