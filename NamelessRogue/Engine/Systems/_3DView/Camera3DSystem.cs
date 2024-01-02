using SharpDX;

using NamelessRogue.Engine.Components._3D;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Text;
using NamelessRogue.Engine.Infrastructure;

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
   //         KeyboardState keyState = Keyboard.GetState();
   //         if (keyState.IsKeyDown(Keys.F1))
   //         {
   //             stopCapturing = true;
   //         }
   //         if (keyState.IsKeyDown(Keys.F2))
   //         {
   //             stopCapturing = false;
   //         }
   //         if (stopCapturing)
   //         {
   //             return;
   //         }

   //         while (NamelessGame.Commander.DequeueCommand(out MoveCamera3dCommand command))
   //         {
			//	Vector3 moveVector = new Vector3(0, 0, 0);

			//	if (command.MovesToMake.Contains(MoveType.Forward))
			//		moveVector += new Vector3(1, 0, 0);
			//	if (command.MovesToMake.Contains(MoveType.Backward))
			//		moveVector += new Vector3(-1, 0, 0);
			//	if (command.MovesToMake.Contains(MoveType.Right))
			//		moveVector += new Vector3(0, -1, 0);
			//	if (command.MovesToMake.Contains(MoveType.Left))
			//		moveVector += new Vector3(0, 1, 0);
			//	//if (keyState.IsKeyDown(Keys.Q))
			//	//	moveVector += new Vector3(0, 0, 1);
			//	//if (keyState.IsKeyDown(Keys.Z))
			//	//	moveVector += new Vector3(0, 0, -1);
			//	AddToCameraPosition(moveVector * amount);
			//}
            
            
   //         //TODO leaving mouse capture here for now, even if its not correct
   //         MouseState currentMouseState = Mouse.GetState();
   //         if (currentMouseState != originalMouseState)
   //         {
   //             float xDifference = currentMouseState.X - originalMouseState.X;
   //             float yDifference = currentMouseState.Y - originalMouseState.Y;
   //             camera.LeftrightRot -= camera.RotationSpeed * xDifference * amount;
   //             camera.UpdownRot -= camera.RotationSpeed * yDifference * amount;
   //             Mouse.SetPosition(NamelessGame.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2);
   //             UpdateViewMatrix();
   //         }
        }


        private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            Matrix cameraRotation = Matrix.RotationY(-camera.UpdownRot) * Matrix.RotationZ(camera.LeftrightRot);
            Vector3 rotatedVector = (Vector3)Vector3.Transform(vectorToAdd, cameraRotation);
            camera.Position += camera.MoveSpeed * rotatedVector;
            UpdateViewMatrix();
        }

        private void UpdateViewMatrix()
        {
            var cameraRotationUpDown = Matrix.RotationY(-camera.UpdownRot);
            var rotationLeftRight = Matrix.RotationZ(camera.LeftrightRot);

            Vector3 cameraOriginalTarget = new Vector3(1, 0, 0);
            Vector3 cameraOriginalUpVector = new Vector3(0, 0, 1);

            Vector3 cameraRotatedTarget = (Vector3)Vector3.Transform(cameraOriginalTarget, cameraRotationUpDown * rotationLeftRight);
            Vector3 cameraFinalTarget = camera.Position + cameraRotatedTarget;

            Vector3 cameraRotatedUpVector = (Vector3)Vector3.Transform(cameraOriginalUpVector, cameraRotationUpDown * rotationLeftRight);
            camera.Look = cameraRotatedTarget;
            camera.View = Matrix.LookAtLH(camera.Position, cameraFinalTarget, cameraRotatedUpVector);
            camera.Up = cameraRotatedUpVector;
        }
    }
}
