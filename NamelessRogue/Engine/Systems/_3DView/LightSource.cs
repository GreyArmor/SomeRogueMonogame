using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.shell;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NamelessRogue.Engine.Systems._3DView
{
	internal class LightSource
	{
		//Texture2D shadowMap;
		RenderTarget2D shadowMapRenderTarget;
		Vector3 position;
		Vector3 lookAtPoint;
		Matrix lightsViewProjectionMatrix;
		Vector4 lightColor;
		int frustrumHeight;
		int frustrumWidth;
		float lightPower;
		float ambient;
		public LightSource(GraphicsDevice device,int frustrumWidth, int frustrumHeight, Vector3 position, Vector3 lookAtPoint, Vector4 lightColor)
		{
			shadowMapRenderTarget = new RenderTarget2D(device, frustrumWidth, frustrumHeight, true, SurfaceFormat.Bgr32SRgb, DepthFormat.Depth24);
			this.position = position;
			this.LookAtPoint = lookAtPoint;
			this.lightColor = lightColor;
			this.frustrumHeight = frustrumHeight;
			this.frustrumWidth = frustrumWidth;
			Matrix view = Matrix.CreateLookAt(position, lookAtPoint, new Vector3(0, 0, 1));
			Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), 1f, 0.001f, 100f);
			lightsViewProjectionMatrix = view * projection;
		}

		public Vector3 Position { get => position; set => position = value; }
		public Vector3 LookAtPoint { get => lookAtPoint; set => lookAtPoint = value; }
		public RenderTarget2D ShadowMapRenderTarget { get => shadowMapRenderTarget; set => shadowMapRenderTarget = value; }
		public float LightPower { get => lightPower; set => lightPower = value; }
		public float Ambient { get => ambient; set => ambient = value; }
		public Matrix LightsViewProjectionMatrix { get => lightsViewProjectionMatrix; set => lightsViewProjectionMatrix = value; }
		public Vector4 LightColor { get => lightColor; set => lightColor = value; }

		public void RecalculateMatrix()
		{
			var dir = -position;
			dir.Y = 2;
			dir.Normalize();
			Matrix view = Matrix.CreateLookAt(position, LookAtPoint, new Vector3(0, 0, 1));
			Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), 1f, 0.001f, 100f);
			lightsViewProjectionMatrix = view * projection;
		}

	}
}
