using Assimp;
using ImGuiNET;
using NamelessRogue.Engine.Components;
using NamelessRogue.shell;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static NamelessRogue.Engine.Systems.Ingame.RenderingSystem3D;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace NamelessRogue.Engine.Infrastructure
{


    [StructLayout(LayoutKind.Sequential)]
    internal struct GUIConstantBuffer
    {
        public Matrix xProjection;
    }

    public class ImGuiRenderer
	{

        Buffer CreateConstantBuffer(SharpDX.Direct3D11.Device device)
        {
            return new SharpDX.Direct3D11.Buffer(device, Utilities.SizeOf<GUIConstantBuffer>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }

        private NamelessGame _game;

		// Graphics
		private SharpDX.Direct3D11.Device _graphicsDevice;
        private DeviceContext _graphicsDeviceContext;
        private Window _window;

        private Effect _effect;
		private RasterizerState _rasterizerState;
		private RasterizerStateDescription _rasterizerStateDescription;

		private byte[] _vertexData;
		private Buffer _vertexBuffer;
		private int _vertexBufferSize;

		private byte[] _indexData;
		private Buffer _indexBuffer;
		private int _indexBufferSize;

		Buffer _constantBuffer;

		// Textures
		private Dictionary<IntPtr, Texture2D> _loadedTextures;

		private int _textureId;
		private IntPtr? _fontTextureId;

		// Input
		private int _scrollWheelValue;

		private List<int> _keys = new List<int>();

		public ImGuiRenderer(NamelessGame game)
		{
			var context = ImGui.CreateContext();
			ImGui.SetCurrentContext(context);

			_game = game ?? throw new ArgumentNullException(nameof(game));
			_graphicsDevice = game.GraphicsDevice;
			_graphicsDeviceContext = game.Window.DeviceContext;
			_window = game.Window;
            _loadedTextures = new Dictionary<IntPtr, Texture2D>();

			_constantBuffer = CreateConstantBuffer(_graphicsDevice);

            var rasterDesc = new RasterizerStateDescription()
            {
                IsAntialiasedLineEnabled = false,
                CullMode = CullMode.None,
                DepthBias = 0,
                DepthBiasClamp = .0f,
                IsDepthClipEnabled = true,
                FillMode = FillMode.Solid,
                IsFrontCounterClockwise = false,
                IsMultisampleEnabled = false,
                IsScissorEnabled = true,
                SlopeScaledDepthBias = .0f
            };

			_rasterizerState = new RasterizerState(game.GraphicsDevice, rasterDesc);

			SetupInput();
		}

		#region ImGuiRenderer

		/// <summary>
		/// Creates a texture and loads the font data from ImGui. Should be called when the <see cref="GraphicsDevice" /> is initialized but before any rendering is done
		/// </summary>
		public virtual unsafe void RebuildFontAtlas()
		{
			// Get font texture from ImGui
			var io = ImGui.GetIO();
			io.Fonts.GetTexDataAsRGBA32(out byte* pixelData, out int width, out int height, out int bytesPerPixel);

			// Copy the data to a managed array
			var pixels = new byte[width * height * bytesPerPixel];
			unsafe { Marshal.Copy(new IntPtr(pixelData), pixels, 0, pixels.Length); }

            // Create and register the texture as an XNA texture
            var textureDescription = new Texture2DDescription()
            {
                Width = width,
                Height = height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.R8G8B8A8_UInt,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };

            DataStream s = DataStream.Create(pixels, true, true);
            DataRectangle rect = new DataRectangle(s.DataPointer, width * 4);

            var tex2d = new Texture2D(_graphicsDevice, textureDescription, rect);

			// Should a texture already have been build previously, unbind it first so it can be deallocated
			if (_fontTextureId.HasValue) UnbindTexture(_fontTextureId.Value);

			// Bind the new texture to an ImGui-friendly id
			_fontTextureId = BindTexture(tex2d);

			// Let ImGui know where to find the texture
			io.Fonts.SetTexID(_fontTextureId.Value);
			io.Fonts.ClearTexData(); // Clears CPU side texture data
			s.Dispose();
			
		}

		/// <summary>
		/// Creates a pointer to a texture, which can be passed through ImGui calls such as <see cref="ImGui.Image" />. That pointer is then used by ImGui to let us know what texture to draw
		/// </summary>
		public virtual IntPtr BindTexture(Texture2D texture)
		{
			var id = new IntPtr(_textureId++);

			_loadedTextures.Add(id, texture);

			return id;
		}

		/// <summary>
		/// Removes a previously created texture pointer, releasing its reference and allowing it to be deallocated
		/// </summary>
		public virtual void UnbindTexture(IntPtr textureId)
		{
			_loadedTextures.Remove(textureId);
		}

		/// <summary>
		/// Sets up ImGui for a new frame, should be called at frame start
		/// </summary>
		public virtual void BeforeLayout(GameTime gameTime)
		{
			ImGui.GetIO().DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

			UpdateInput();

			ImGui.NewFrame();
		}

		/// <summary>
		/// Asks ImGui for the generated geometry data and sends it to the graphics pipeline, should be called after the UI is drawn using ImGui.** calls
		/// </summary>
		public virtual void AfterLayout()
		{
			ImGui.Render();

			unsafe { RenderDrawData(ImGui.GetDrawData()); }
		}

		#endregion ImGuiRenderer

		#region Setup & Update

		/// <summary>
		/// Maps ImGui keys to XNA keys. We use this later on to tell ImGui what keys were pressed
		/// </summary>
		protected virtual void SetupInput()
		{
			var io = ImGui.GetIO();

			_keys.Add(io.KeyMap[(int)ImGuiKey.Tab] = (int)Keys.Tab);
			_keys.Add(io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left);
			_keys.Add(io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right);
			_keys.Add(io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up);
			_keys.Add(io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down);
			_keys.Add(io.KeyMap[(int)ImGuiKey.PageUp] = (int)Keys.PageUp);
			_keys.Add(io.KeyMap[(int)ImGuiKey.PageDown] = (int)Keys.PageDown);
			_keys.Add(io.KeyMap[(int)ImGuiKey.Home] = (int)Keys.Home);
			_keys.Add(io.KeyMap[(int)ImGuiKey.End] = (int)Keys.End);
			_keys.Add(io.KeyMap[(int)ImGuiKey.Delete] = (int)Keys.Delete);
			_keys.Add(io.KeyMap[(int)ImGuiKey.Backspace] = (int)Keys.Back);
			_keys.Add(io.KeyMap[(int)ImGuiKey.Enter] = (int)Keys.Enter);
			_keys.Add(io.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape);
			_keys.Add(io.KeyMap[(int)ImGuiKey.Space] = (int)Keys.Space);
			_keys.Add(io.KeyMap[(int)ImGuiKey.A] = (int)Keys.A);
			_keys.Add(io.KeyMap[(int)ImGuiKey.C] = (int)Keys.C);
			_keys.Add(io.KeyMap[(int)ImGuiKey.V] = (int)Keys.V);
			_keys.Add(io.KeyMap[(int)ImGuiKey.X] = (int)Keys.X);
			_keys.Add(io.KeyMap[(int)ImGuiKey.Y] = (int)Keys.Y);
			_keys.Add(io.KeyMap[(int)ImGuiKey.Z] = (int)Keys.Z);

			// MonoGame-specific //////////////////////
			_game.RenderForm.KeyPress += (s, a) =>
			{
				if(!_game.IsActive) { return; }
				if (a.KeyChar == '\t') return;

				io.AddInputCharacter(a.KeyChar);
			};
			///////////////////////////////////////////

			// FNA-specific ///////////////////////////
			//TextInputEXT.TextInput += c =>
			//{
			//    if (c == '\t') return;

			//    ImGui.GetIO().AddInputCharacter(c);
			//};
			///////////////////////////////////////////
			var fontAtlas = ImGui.GetIO().Fonts;
			fontAtlas.AddFontDefault();
			ImGUI_FontLibrary.AnonymousPro_Regular32 = fontAtlas.AddFontFromFileTTF(@"Content\Fonts\AnonymousPro-Regular.ttf", 32);
			ImGUI_FontLibrary.AnonymousPro_Regular24 = fontAtlas.AddFontFromFileTTF(@"Content\Fonts\AnonymousPro-Regular.ttf", 24);
			ImGUI_FontLibrary.AnonymousPro_Regular16 = fontAtlas.AddFontFromFileTTF(@"Content\Fonts\AnonymousPro-Regular.ttf", 16);
			ImGUI_FontLibrary.AnonymousPro_Regular8 = fontAtlas.AddFontFromFileTTF(@"Content\Fonts\AnonymousPro-Regular.ttf", 8);

            _effect = new Effect(_graphicsDevice, "Content\\GUI.fx", "GUI_VS", "GUI_PS");
        }

		/// <summary>
		/// Updates the <see cref="Effect" /> to the current matrices and texture
		/// </summary>
		protected virtual void UpdateEffect(Texture2D texture)
		{		
            var io = ImGui.GetIO();
			var constantBuffer = new GUIConstantBuffer() { xProjection = Matrix.OrthoOffCenterLH(0f, io.DisplaySize.X, io.DisplaySize.Y, 0f, -1f, 1f) };
            _game.Window.DeviceContext.UpdateSubresource(ref constantBuffer, _constantBuffer);
		}

		/// <summary>
		/// Sends XNA input state to ImGui
		/// </summary>
		protected virtual void UpdateInput()
		{

			if (!_game.IsActive) { return; }

			var io = ImGui.GetIO();

			MouseState mouse = _window.MouseState;
		//	var keyboard = Keyboard.GetState();

			for (int i = 0; i < _keys.Count; i++)
			{
			//	io.KeysDown[_keys[i]] = keyboard.IsKeyDown((Keys)_keys[i]);
			}

			//io.KeyShift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
			//io.KeyCtrl = keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);
			//io.KeyAlt = keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt);
			//io.KeySuper = keyboard.IsKeyDown(Keys.LeftWindows) || keyboard.IsKeyDown(Keys.RightWindows);

			io.DisplaySize = new System.Numerics.Vector2(_game.GetActualWidth(), _game.GetActualHeight());
			io.DisplayFramebufferScale = new System.Numerics.Vector2(1f, 1f);

			io.MousePos = new System.Numerics.Vector2(mouse.X, mouse.Y);

			io.MouseDown[0] = mouse.LeftPressed;
			io.MouseDown[1] = mouse.RightPressed;
			io.MouseDown[2] = mouse.MiddlePressed;

			if(mouse.LeftPressed)
			{
				mouse.ToString();
			}

			var scrollDelta = mouse.MouseWheelDelta;
			io.MouseWheel = scrollDelta > 0 ? 1 : scrollDelta < 0 ? -1 : 0;
		}

		#endregion Setup & Update

		#region Internals

		/// <summary>
		/// Gets the geometry as set up by ImGui and sends it to the graphics device
		/// </summary>
		private void RenderDrawData(ImDrawDataPtr drawData)
		{
			// Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled, vertex/texcoord/color pointers
			var lastViewport = _graphicsDeviceContext.Rasterizer.GetViewports<RawViewportF>().Last();
		//	var scissorsRects = _graphicsDeviceContext.Rasterizer.GetScissorRectangles<RawRectangle>();
          //  var lastScissorBox = scissorsRects.Last();

			var blendStateDesc = new BlendStateDescription() { };

			_graphicsDeviceContext.OutputMerger.BlendFactor = Color.White;
			_graphicsDeviceContext.OutputMerger.BlendState = new BlendState(_graphicsDevice, blendStateDesc);
			_graphicsDeviceContext.Rasterizer.State = _rasterizerState;
			//     _graphicsDeviceContext.OutputMerger.DepthStencilState = DepthStencilState.DepthRead;

			// Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
			drawData.ScaleClipRects(ImGui.GetIO().DisplayFramebufferScale);

			// Setup projection
			_graphicsDeviceContext.Rasterizer.SetViewport(new RawViewportF() { X = 0, Y = 0, Width = _game.GetActualWidth(), Height = _game.GetActualHeight(), });

			UpdateBuffers(drawData);

			RenderCommandLists(drawData);

			// Restore modified state
			_graphicsDeviceContext.Rasterizer.SetViewport(lastViewport);
		//	_graphicsDeviceContext.Rasterizer.SetScissorRectangle(lastScissorBox.Left, lastScissorBox.Top, lastScissorBox.Right, lastScissorBox.Bottom);
		}

		private unsafe void UpdateBuffers(ImDrawDataPtr drawData)
		{
			if (drawData.TotalVtxCount == 0)
			{
				return;
			}

			// Expand buffers if we need more room
			if (drawData.TotalVtxCount > _vertexBufferSize)
			{
				_vertexBuffer?.Dispose();

				_vertexBufferSize = (int)(drawData.TotalVtxCount * 1.5f);
                _vertexData = new byte[_vertexBufferSize * DrawVertDeclaration.Size];
                _vertexBuffer = Buffer.Create(_graphicsDevice, BindFlags.VertexBuffer, _vertexData);
            }

			if (drawData.TotalIdxCount > _indexBufferSize)
			{
				_indexBuffer?.Dispose();

				_indexBufferSize = (int)(drawData.TotalIdxCount * 1.5f);
                _indexData = new byte[_indexBufferSize * sizeof(ushort)];
                _indexBuffer = Buffer.Create(_graphicsDevice, BindFlags.IndexBuffer, _indexData);              
			}

			// Copy ImGui's vertices and indices to a set of managed byte arrays
			int vtxOffset = 0;
			int idxOffset = 0;

			for (int n = 0; n < drawData.CmdListsCount; n++)
			{
				ImDrawListPtr cmdList = drawData.CmdListsRange[n];

				fixed (void* vtxDstPtr = &_vertexData[vtxOffset * DrawVertDeclaration.Size])
				fixed (void* idxDstPtr = &_indexData[idxOffset * sizeof(ushort)])
				{
                    System.Buffer.MemoryCopy((void*)cmdList.VtxBuffer.Data, vtxDstPtr, _vertexData.Length, cmdList.VtxBuffer.Size * DrawVertDeclaration.Size);
					System.Buffer.MemoryCopy((void*)cmdList.IdxBuffer.Data, idxDstPtr, _indexData.Length, cmdList.IdxBuffer.Size * sizeof(ushort));
				}

				vtxOffset += cmdList.VtxBuffer.Size;
				idxOffset += cmdList.IdxBuffer.Size;
			}

			// Copy the managed byte arrays to the gpu vertex- and index buffers
			_vertexBuffer = Buffer.Create(_graphicsDevice, BindFlags.VertexBuffer, _vertexData);
			_indexBuffer = Buffer.Create(_graphicsDevice, BindFlags.IndexBuffer, _indexData);

        }

		private unsafe void RenderCommandLists(ImDrawDataPtr drawData)
		{
           

            int vtxOffset = 0;
			int idxOffset = 0;

			for (int n = 0; n < drawData.CmdListsCount; n++)
			{
				ImDrawListPtr cmdList = drawData.CmdListsRange[n];

				for (int cmdi = 0; cmdi < cmdList.CmdBuffer.Size; cmdi++)
				{
					ImDrawCmdPtr drawCmd = cmdList.CmdBuffer[cmdi];

					if (drawCmd.ElemCount == 0)
					{
						continue;
					}

					if (!_loadedTextures.ContainsKey(drawCmd.TextureId))
					{
						throw new InvalidOperationException($"Could not find a texture with id '{drawCmd.TextureId}', please check your bindings");
					}
					var rect = new Rectangle(
						(int)drawCmd.ClipRect.X,
						(int)drawCmd.ClipRect.Y,
						(int)(drawCmd.ClipRect.Z - drawCmd.ClipRect.X),
						(int)(drawCmd.ClipRect.W - drawCmd.ClipRect.Y));

                    _graphicsDeviceContext.Rasterizer.SetScissorRectangle(rect.Left,rect.Top, rect.Right, rect.Bottom);

					UpdateEffect(_loadedTextures[drawCmd.TextureId]);


					_effect.Apply(_graphicsDeviceContext);

                    _game.Window.DeviceContext.VertexShader.SetConstantBuffer(0, _constantBuffer);
                    _game.Window.DeviceContext.PixelShader.SetConstantBuffer(0, _constantBuffer);
                    _game.Window.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, sizeof(Matrix), 0));
                    _game.Window.DeviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, SharpDX.DXGI.Format.R32_SInt, 0);

                    _graphicsDeviceContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

                    _graphicsDeviceContext.DrawIndexed(cmdList.IdxBuffer.Size, (int)drawCmd.IdxOffset + idxOffset, (int)drawCmd.VtxOffset + vtxOffset);


     //               foreach (var pass in effect.CurrentTechnique.Passes)
					//{
					//	pass.Apply();

						
//#pragma warning disable CS0618 // // FNA does not expose an alternative method.
//                        _graphicsDevice.DrawIndexedPrimitives(
//							primitiveType: PrimitiveType.TriangleList,
//							baseVertex: (int)drawCmd.VtxOffset + vtxOffset,
//							minVertexIndex: 0,
//							numVertices: cmdList.VtxBuffer.Size,
//							startIndex: (int)drawCmd.IdxOffset + idxOffset,
//							primitiveCount: (int)drawCmd.ElemCount / 3
//						);
//#pragma warning restore CS0618
					//}
				}

				vtxOffset += cmdList.VtxBuffer.Size;
				idxOffset += cmdList.IdxBuffer.Size;
			}
		}

		#endregion Internals
	}
}