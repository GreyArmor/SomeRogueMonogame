using NamelessRogue.Engine.Components._3D;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Text;
using NamelessRogue.Engine.Infrastructure;
using System.Numerics;
using Veldrid;

namespace NamelessRogue.Engine.Systems._3DView
{
    ///TODO: COMPLETELY REFACTOR THIS TEST SYSTEM
	class Camera3DSystem : BaseSystem
	{
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

       
        MouseState originalMouseState;
        NamelessGame game;
        Camera3D camera;
        public Camera3DSystem(NamelessGame game)
        {
            originalMouseState = new MouseState() {X = game.GetActualWidth() / 2, Y = game.GetActualHeight() / 2 };
            this.game = game;
        }
        bool firstTime = true;
        public override void Update(GameTime gameTime, NamelessGame game)
		{
            if (firstTime)
            {
                System.Windows.Forms.Cursor.Position = new System.Drawing.Point( game.GetActualWidth() / 2, game.GetActualHeight() / 2);
               // Mouse.SetPosition(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2);
                firstTime = false;
            }
            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            camera = game.PlayerEntity.GetComponentOfType<Camera3D>();
            ProcessInput(timeDifference, game);
        }
        bool stopCapturing = false;
        private void ProcessInput(float amount, NamelessGame game)
        {
            //TODO for debug, remove later
            KeyboardState keyState = new KeyboardState(game.Input);
            if (keyState.Keys.Contains(Key.F1))
            {
                    stopCapturing = true;
            }
            if (keyState.Keys.Contains(Key.F2))
            {
                stopCapturing = false;
            }
            if (stopCapturing)
            {
                return;
            }

            while (game.Commander.DequeueCommand(out MoveCamera3dCommand command))
            {
                Vector3 moveVector = new Vector3(0, 0, 0);

                if (command.MovesToMake.Contains(MoveType.Forward))
                    moveVector += new Vector3(1, 0, 0);
                if (command.MovesToMake.Contains(MoveType.Backward))
                    moveVector += new Vector3(-1, 0, 0);
                if (command.MovesToMake.Contains(MoveType.Right))
                    moveVector += new Vector3(0, -1, 0);
                if (command.MovesToMake.Contains(MoveType.Left))
                    moveVector += new Vector3(0, 1, 0);
                //if (keyState.IsKeyDown(Key.Q))
                //	moveVector += new Vector3(0, 0, 1);
                //if (keyState.IsKeyDown(Key.Z))
                //	moveVector += new Vector3(0, 0, -1);
                AddToCameraPosition(moveVector * amount);
            }


            //TODO leaving mouse capture here for now, even if its not correct
            MouseState currentMouseState = new MouseState(game.Input);
            if (currentMouseState != originalMouseState)
            {
                float xDifference = currentMouseState.X - originalMouseState.X;
                float yDifference = currentMouseState.Y - originalMouseState.Y;
                camera.LeftrightRot -= camera.RotationSpeed * xDifference * amount;
                camera.UpdownRot -= camera.RotationSpeed * yDifference * amount;
                game.Window.SetMousePosition(game.GetActualWidth() / 2, game.GetActualHeight() / 2);
                UpdateViewMatrix();
            }
        }


        private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            Matrix4x4 cameraRotation = Matrix4x4.CreateRotationY(-camera.UpdownRot) * Matrix4x4.CreateRotationZ(camera.LeftrightRot);
            Vector3 rotatedVector = (Vector3)Vector3.Transform(vectorToAdd, cameraRotation);
            camera.Position += camera.MoveSpeed * rotatedVector;
            UpdateViewMatrix();
        }

        private void UpdateViewMatrix()
        {
            var cameraRotationUpDown = Matrix4x4.CreateRotationY(-camera.UpdownRot);
            var rotationLeftRight = Matrix4x4.CreateRotationZ(camera.LeftrightRot);

            Vector3 cameraOriginalTarget = new Vector3(1, 0, 0);
            Vector3 cameraOriginalUpVector = Vector3.UnitZ;

            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotationUpDown * rotationLeftRight);
            Vector3 cameraFinalTarget = camera.Position + cameraRotatedTarget;

            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotationUpDown * rotationLeftRight);

           // Matrix4x4.CreateLookAt(Vector3.UnitZ * 2.5f, Vector3.Zero, Vector3.UnitY));


            camera.Look = cameraRotatedTarget;
            camera.View = Matrix4x4.CreateLookAt(camera.Position, cameraFinalTarget, cameraOriginalUpVector);
            camera.Up = cameraRotatedUpVector;
        }
    }
}
