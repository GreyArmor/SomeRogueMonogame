using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Multimedia;
using SharpDX.RawInput;
using System.Diagnostics;
using SharpDX.Windows;
using System.Windows.Forms;
using SharpDX.D3DCompiler;
using Device = SharpDX.Direct3D11.Device;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace NamelessRogue.Engine.Infrastructure
{
    public class Window                  // 256 lines
    {
        // Properties.
        public bool VerticalSyncEnabled { get; set; }
        public int VideoCardMemory { get; private set; }
        public string VideoCardDescription { get; private set; }
        public SwapChain SwapChain { get; set; }
        public SharpDX.Direct3D11.Device Device { get; private set; }
        public DeviceContext DeviceContext { get; private set; }
        public  RenderTargetView RenderTargetView { get; set; }
        public Texture2D DepthStencilBuffer { get; set; }
        public DepthStencilState DepthStencilState { get; set; }
        public DepthStencilView DepthStencilView { get; set; }
        public RasterizerState RasterState { get; set; }

        public MouseState MouseState { get; set; } = new MouseState();

        public KeyboardState KeyboardState { get; set; } = new KeyboardState();
        public bool MouseStateChanged { get; set; } = false;
        public Viewport Viewport { get; set; }

        
        // Constructor
        public Window() { }
        // Methods
        public bool Initialize(WindowConfiguration configuration, IntPtr windowHandle, RenderForm form)
        {
            try
            {
                // Store the vsync setting.
                VerticalSyncEnabled = WindowConfiguration.VerticalSyncEnabled;

                // Create a DirectX graphics interface factory.
                var desc = new SwapChainDescription()
                {
                    BufferCount = 1,
                    ModeDescription =
                                   new ModeDescription(form.ClientSize.Width, form.ClientSize.Height,
                                                       new Rational(60, 1), Format.R8G8B8A8_UNorm),
                    IsWindowed = true,
                    OutputHandle = form.Handle,
                    SampleDescription = new SampleDescription(1, 0),
                    SwapEffect = SwapEffect.Discard,
                    Usage = Usage.RenderTargetOutput
                };

                // Create Device and SwapChain
                Device device;
                SwapChain swapChain;
                Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc, out device, out swapChain);
                SwapChain = swapChain;
                DeviceContext = device.ImmediateContext;
                Device = device;
                // Ignore all windows events
                var factory = swapChain.GetParent<Factory>();
                factory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);

                // New RenderTargetView from the backbuffer
                var backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
                this.RenderTargetView = new RenderTargetView(device, backBuffer);

                DeviceContext.OutputMerger.SetTargets(RenderTargetView);


                form.MouseMove += MouseInput;
                form.MouseClick += MouseInput;
                

              //  SharpDX.RawInput.Device.KeyboardInput += KeyboardInput;

                return true;
            }
            catch
            {
                return false;
            }
        }


        private void KeyboardInput(object sender, KeyboardInputEventArgs e)
        {
            KeyboardState.Keys.Add(e.Key);
        }

        public void ClearStates()
        {
            MouseState = new MouseState();
            KeyboardState = new KeyboardState();
            MouseStateChanged = false;
        }

        private void MouseInput(object sender, MouseEventArgs e)
        {
            MouseState = new MouseState
            {
                X = e.X,
                Y = e.Y,
                LeftPressed = e.Button == MouseButtons.Left,
                RightPressed = e.Button == MouseButtons.Right,
                MiddlePressed = e.Button == MouseButtons.Middle,
                MouseWheelDelta = e.Delta
            };
            MouseStateChanged = true;

            Debug.WriteLine($@"X={e.X} Y={e.Y} mouseflags = {e.Button}");
        }

       

        public void ShutDown()
        {
            // Before shutting down set to windowed mode or when you release the swap chain it will throw an exception.
            SwapChain?.SetFullscreenState(false, null);

            RasterState?.Dispose();
            RasterState = null;
            DepthStencilView?.Dispose();
            DepthStencilView = null;
            DepthStencilState?.Dispose();
            DepthStencilState = null;
            DepthStencilBuffer?.Dispose();
            DepthStencilBuffer = null;
            RenderTargetView?.Dispose();
            RenderTargetView = null;
            DeviceContext?.Dispose();
            DeviceContext = null;
            Device?.Dispose();
            Device = null;
            SwapChain?.Dispose();
            SwapChain = null;
        }
        public void BeginScene(float red, float green, float blue, float alpha)
        {
            // Clear the depth buffer.
            DeviceContext.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth, 1, 0);

            // Clear the back buffer.   Color.Transparent.ToColor4()
            DeviceContext.ClearRenderTargetView(RenderTargetView, new Color4(red, green, blue, alpha));
        }
        public void EndScene()
        {
            // Present the back buffer to the screen since rendering is complete.
            if (VerticalSyncEnabled)
                SwapChain.Present(1, 0); // Lock to screen refresh rate.
            else
                SwapChain.Present(0, 0); // Present as fast as possible.
        }
    }
}
