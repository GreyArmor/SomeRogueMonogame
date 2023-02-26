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
		Vector3 direction;
		Matrix lightsViewProjectionMatrix;
		Vector4 lightColor;
		int frustrumHeight;
		int frustrumWidth;
		float lightPower;
		float ambient;
		public LightSource(GraphicsDevice device,int frustrumWidth, int frustrumHeight, Vector3 position, Vector3 direction, Vector4 lightColor)
		{
			shadowMapRenderTarget = new RenderTarget2D(device, frustrumWidth, frustrumHeight, true, device.DisplayMode.Format, DepthFormat.Depth24);
			this.position = position;
			this.direction = direction;
			this.lightColor = lightColor;
			this.frustrumHeight = frustrumHeight;
			this.frustrumWidth = frustrumWidth;
			Matrix view = Matrix.CreateLookAt(position, direction, new Vector3(0, 0, 1));
			Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1f, 0.001f, 10000f);
			lightsViewProjectionMatrix = view * projection;
		}

		public Vector3 Position { get => position; set => position = value; }
		public Vector3 Direction { get => direction; set => direction = value; }
		public RenderTarget2D ShadowMapRenderTarget { get => shadowMapRenderTarget; set => shadowMapRenderTarget = value; }
		public float LightPower { get => lightPower; set => lightPower = value; }
		public float Ambient { get => ambient; set => ambient = value; }
		public Matrix LightsViewProjectionMatrix { get => lightsViewProjectionMatrix; set => lightsViewProjectionMatrix = value; }
		public Vector4 LightColor { get => lightColor; set => lightColor = value; }

		public void RecalculateMatrix()
		{
			Matrix projection = Matrix.CreateLookAt(position, new Vector3(-2, 3, -10), new Vector3(0, 1, 0));
			Matrix view = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1f, 0f, 100f);
			lightsViewProjectionMatrix = view * projection;
		}

	}
}
