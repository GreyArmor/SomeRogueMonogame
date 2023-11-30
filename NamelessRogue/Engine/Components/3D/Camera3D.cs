using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace NamelessRogue.Engine.Components._3D
{
    class Camera3D : Component
    {
        public Vector3 Position { get; set; }
        public Matrix Projection { get; set; }
		public Matrix View { get; set; } = Matrix.Identity;
        public Vector3 Look { get; set; } = new Vector3(1, 0, 0);
        public float LeftrightRot { get; set; } = MathHelper.ToRadians(-90f);
        public float UpdownRot { get; set; } = MathHelper.ToRadians(-45);
		public Vector3 Up { get; internal set; }
        public Vector3 Right { get { return Vector3.Cross(Up,Look); } }

		public float RotationSpeed = 0.3f;
		public float MoveSpeed = 0.3f;
		public Camera3D(Game game) 
        {
            this.Position = new Vector3(0f, 0f, 0.2f); ;
            this.Projection = 
                Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(60), game.GraphicsDevice.Viewport.AspectRatio,
                0.001f, 10000.0f);
		}


    }
}
