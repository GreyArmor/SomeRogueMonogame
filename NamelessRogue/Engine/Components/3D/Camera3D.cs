
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Windows.Forms;

namespace NamelessRogue.Engine.Components._3D
{
    class Camera3D : Component
    {
        public Vector3 Position { get; set; }
        public Matrix4x4 Projection { get; set; }
		public Matrix4x4 View { get; set; } = Matrix4x4.Identity;
        public Vector3 Look { get; set; } = new Vector3(1, 0, 0);
        public float LeftrightRot { get; set; } = MathUtil.DegreesToRadians(-90f);
        public float UpdownRot { get; set; } = MathUtil.DegreesToRadians(-45);
		public Vector3 Up { get; internal set; }
        public Vector3 Right { get { return Vector3.Cross(Up,Look); } }

		public float RotationSpeed = 0.3f;
		public float MoveSpeed = 0.1f;
        public float NearPlane = 0.001f;
        public float FarPlane = 1000.0f;

        public Camera3D(NamelessGame game) 
        {
            this.Position = new Vector3(0f, 0f, 0.2f); ;
            this.Projection = 
                Matrix4x4.CreatePerspectiveFieldOfView(
                MathUtil.DegreesToRadians(60), game.AspectRatio, NearPlane, FarPlane);
		}


    }
}
