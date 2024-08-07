﻿
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;

using Num = System.Numerics;

namespace NamelessRogue.Engine.Systems
{
    public class UIRenderSystem : BaseSystem
    {

        //private SpriteBatch spriteBatch;
        //NamelessGame game;
        //private static ImGuiRenderer _imGuiRendererInstance;

        //// private TextureView _xnaTexture;
        //private IntPtr _imGuiTexture;
        //public UIRenderSystem(NamelessGame game)
        //{
        //    this.game = game;
        //    //  spriteBatch = new SpriteBatch(NamelessGame.GraphicsDevice);

        //    if (_imGuiRendererInstance == null)
        //    {
        //        _imGuiRendererInstance = new ImGuiRenderer(game.GraphicsDevice, game.GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription,
        //          (int)game.GraphicsDevice.MainSwapchain.Framebuffer.Width, (int)game.GraphicsDevice.MainSwapchain.Framebuffer.Height);
        //    }
        //}
        //public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        //public override void Update(GameTime gameTime, NamelessGame game)
        //{

        //    _imGuiRendererInstance.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds, game.Input);
        //    game.CurrentContext.ContextScreen.DrawLayout();
        //    _imGuiRendererInstance.Render(game.GraphicsDevice, game.CommandList);
        //}

        //// Direct port of the example at https://github.com/ocornut/imgui/blob/master/examples/sdl_opengl2_example/main.cpp
        //private float f = 0.0f;

        //private bool show_test_window = false;
        //private bool show_another_window = false;
        //private Num.Vector3 clear_color = new Num.Vector3(114f / 255f, 144f / 255f, 154f / 255f);
        //private byte[] _textBuffer = new byte[100];

        //protected virtual void ImGuiLayout()
        //{
        //    // 1. Show a simple window
        //    // Tip: if we don't call ImGui.Begin()/ImGui.End() the widgets appears in a window automatically called "Debug"
        //    {

        //        ImGui.Text("Hello, world!");
        //        ImGui.SliderFloat("float", ref f, 0.0f, 1.0f, string.Empty);
        //        ImGui.ColorEdit3("clear color", ref clear_color);
        //        if (ImGui.Button("Test Window")) show_test_window = !show_test_window;
        //        if (ImGui.Button("Another Window")) show_another_window = !show_another_window;
        //        ImGui.Text(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate, ImGui.GetIO().Framerate));

        //        ImGui.InputText("Text input", _textBuffer, 100);

        //        ImGui.Text("Texture sample");
        //        ImGui.Image(_imGuiTexture, new Num.Vector2(300, 150), Num.Vector2.Zero, Num.Vector2.One, Num.Vector4.One, Num.Vector4.One); // Here, the previously loaded texture is used
        //    }

        //    // 2. Show another simple window, this time using an explicit Begin/End pair
        //    if (show_another_window)
        //    {
        //        ImGui.SetNextWindowSize(new Num.Vector2(200, 100), ImGuiCond.FirstUseEver);
        //        ImGui.Begin("Another Window", ref show_another_window);
        //        ImGui.Text("Hello");
        //        ImGui.End();
        //    }

        //    // 3. Show the ImGui test window. Most of the sample code is in ImGui.ShowTestWindow()
        //    if (show_test_window)
        //    {
        //        ImGui.SetNextWindowPos(new Num.Vector2(650, 20), ImGuiCond.FirstUseEver);
        //        ImGui.ShowDemoWindow(ref show_test_window);
        //    }
        //}

        ////public static TextureView CreateTexture(GraphicsDevice device, int width, int height, Func<int, Color> paint)
        ////{
        ////    //initialize a texture
        ////    var texture = new TextureView(device, width, height);

        ////    //the array holds the color for each pixel in the texture
        ////    Color[] data = new Color[width * height];
        ////    for (var pixel = 0; pixel < data.Length; pixel++)
        ////    {
        ////        //the function applies the color according to the specified pixel
        ////        data[pixel] = paint(pixel);
        ////    }

        ////    //set the color
        ////    texture.SetData(data);

        ////    return texture;
        ////}
        public override HashSet<Type> Signature => new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame game)
        {
            throw new NotImplementedException();
        }
    }
}
