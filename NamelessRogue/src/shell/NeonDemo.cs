using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

/// <summary>
/// Minimal "neon sign" effect for MonoGame using additive blending layers
/// (no custom shaders needed). Draws the sign texture multiple times at
/// increasing scale / decreasing alpha to fake a glow halo, plus a
/// flicker animation on brightness.
///
/// Swap in your own sign texture at "signTexture" (a PNG with the tube
/// shape drawn in a bright, saturated color on a transparent background
/// works best).
/// </summary>
public class NeonDemo : Game
{
	private GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;

	private Texture2D _signTexture;   // your neon tube shape (transparent bg)
	private Vector2 _signPosition;
	private Vector2 _signOrigin;

	private SpriteFont _font;
	private string _signText = "NamelessRogueAlpha"; // change to whatever your sign should say

	private Random _rng = new Random();
	private float _flickerBrightness = 1f;
	private float _flickerTimer = 0f;
	private float _nextFlickerEvent = 0f;

	// Core neon color - keep it saturated
	private Color _neonColor = new Color(255, 40, 180); // hot pink

	public NeonDemo()
	{
		_graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
	}

	protected override void LoadContent()
	{
		_spriteBatch = new SpriteBatch(GraphicsDevice);

		// Load the font (compiled via Content Pipeline from NeonFont.spritefont)
		_font = Content.Load<SpriteFont>("ryga");

		// Render the sign text to its own transparent texture at runtime.
		// Text is drawn white here — actual color is applied later when
		// compositing the glow layers, so the same texture can be recolored freely.
		_signTexture = NeonTextGenerator.GenerateTextTexture(
			GraphicsDevice,
			_font,
			_signText,
			Color.White,
			padding: 60);

		_signOrigin = new Vector2(_signTexture.Width / 2f, _signTexture.Height / 2f);
		_signPosition = new Vector2(
			GraphicsDevice.Viewport.Width / 2f,
			GraphicsDevice.Viewport.Height / 2f);

		ScheduleNextFlicker();
	}

	protected override void Update(GameTime gameTime)
	{
		float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
		UpdateFlicker(dt);
		base.Update(gameTime);
	}

	private void ScheduleNextFlicker()
	{
		// Random time until the next flicker "event"
		_nextFlickerEvent = (float)(_rng.NextDouble() * 2.0 + 0.5);
		_flickerTimer = 0f;
	}

	private void UpdateFlicker(float dt)
	{
		_flickerTimer += dt;

		if (_flickerTimer >= _nextFlickerEvent)
		{
			// Trigger a brief dip/stutter in brightness
			_flickerBrightness = (float)(_rng.NextDouble() * 0.4 + 0.1); // dip to 10-50%
			ScheduleNextFlicker();
		}
		else
		{
			// Recover smoothly back to full brightness between events
			_flickerBrightness = MathHelper.Lerp(_flickerBrightness, 1f, dt * 8f);
		}

		// Small constant jitter so it never looks perfectly static
		_flickerBrightness += (float)(_rng.NextDouble() - 0.5) * 0.02f;
		_flickerBrightness = MathHelper.Clamp(_flickerBrightness, 0f, 1f);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(new Color(8, 8, 14)); // dark background sells neon contrast

		DrawNeonSign();

		base.Draw(gameTime);
	}

	private void DrawNeonSign()
	{
		// --- Glow halo layers (additive, back to front, big+faint to small+bright) ---
		_spriteBatch.Begin(blendState: BlendState.Additive, samplerState: SamplerState.LinearClamp);

		int glowLayers = 4;
		for (int i = glowLayers; i >= 1; i--)
		{
			float scale = 1f + i * 0.15f;               // outer layers bigger
			float alpha = (0.10f / i) * _flickerBrightness; // outer layers fainter

			_spriteBatch.Draw(
				_signTexture,
				_signPosition,
				null,
				_neonColor * alpha,
				0f,
				_signOrigin,
				scale,
				SpriteEffects.None,
				0f);
		}

		_spriteBatch.End();

		// --- Sharp bright core on top (normal alpha blend, near-white hot center) ---
		_spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.LinearClamp);

		Color coreColor = Color.Lerp(_neonColor, Color.White, 0.5f) * _flickerBrightness;
		_spriteBatch.Draw(
			_signTexture,
			_signPosition,
			null,
			coreColor,
			0f,
			_signOrigin,
			1f,
			SpriteEffects.None,
			0f);

		_spriteBatch.End();
	}

	/// <summary>
	/// Renders a text string to a standalone transparent Texture2D at runtime,
	/// so it can be fed into the same additive-glow pipeline used for shapes.
	/// Requires a SpriteFont (see NeonFont.spritefont) loaded via the normal
	/// Content pipeline.
	/// </summary>
	public static class NeonTextGenerator
	{
		/// <summary>
		/// Renders text into a texture sized to fit it plus padding
		/// (padding matters: the glow layers scale outward, so without
		/// extra transparent margin the glow gets clipped at the texture edge).
		/// </summary>
		public static Texture2D GenerateTextTexture(
			GraphicsDevice device,
			SpriteFont font,
			string text,
			Color color,
			int padding = 40)
		{
			Vector2 textSize = font.MeasureString(text);

			int width = (int)textSize.X + padding * 2;
			int height = (int)textSize.Y + padding * 2;

			var renderTarget = new RenderTarget2D(
				device, width, height, false,
				SurfaceFormat.Color, DepthFormat.None, 0,
				RenderTargetUsage.PreserveContents);

			var spriteBatch = new SpriteBatch(device);

			device.SetRenderTarget(renderTarget);
			device.Clear(Color.Transparent);

			spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.LinearClamp);
			spriteBatch.DrawString(
				font,
				text,
				new Vector2(padding, padding),
				color);
			spriteBatch.End();

			device.SetRenderTarget(null);
			spriteBatch.Dispose();

			return renderTarget;
		}
	}


}
