using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using System.Numerics;


namespace NamelessRogue.Engine.Systems._3DView
{
    internal class LightSource
    {
    //    Texture shadowMap;
    //    Framebuffer shadowMapFramBuffer;
    //    Vector3 position;
    //    Vector3 lookAtPoint;
    //    Matrix4x4 lightsViewProjectionMatrix;
    //    Vector4 lightColor;
    //    int frustrumHeight;
    //    int frustrumWidth;
    //    float lightPower;
    //    float ambient;
    //    public LightSource(NamelessGame game, int frustrumWidth, int frustrumHeight, Vector3 position, Vector3 lookAtPoint, Vector4 lightColor)
    //    {
    //        var factory = game.GraphicsDevice.ResourceFactory;

    //        TextureDescription textureDescription = TextureDescription.Texture2D(
    //            (uint)frustrumHeight, (uint)frustrumWidth, 1, 1,
    //             PixelFormat.R8_G8_B8_A8_UNorm, TextureUsage.RenderTarget | TextureUsage.Sampled);

    //        TextureDescription depth = TextureDescription.Texture2D(
    //            (uint)frustrumHeight, (uint)frustrumWidth, 1, 1, PixelFormat.R16_UNorm, TextureUsage.DepthStencil);

    //        var depthTexture = factory.CreateTexture(depth);

    //        shadowMap = factory.CreateTexture(textureDescription);

    //        var framebufferDescription = new FramebufferDescription(depthTexture, shadowMap);

    //        shadowMapFramBuffer = factory.CreateFramebuffer(framebufferDescription);
    //        ShadowMapTextureView = factory.CreateTextureView(shadowMap);

    //        this.position = position;
    //        this.LookAtPoint = lookAtPoint;
    //        this.lightColor = lightColor;
    //        this.frustrumHeight = frustrumHeight;
    //        this.frustrumWidth = frustrumWidth;
    //        Matrix4x4 view = Matrix4x4.CreateLookAt(position, lookAtPoint, new Vector3(0, 0, 1));
    //        Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(MathUtil.DegreesToRadians(45f), 1f, 0.001f, 100f);
    //        lightsViewProjectionMatrix = view * projection;
    //    }

    //    public Vector3 Position { get => position; set => position = value; }
    //    public Vector3 LookAtPoint { get => lookAtPoint; set => lookAtPoint = value; }
    //    public TextureView ShadowMapTextureView { get; set; }
    //    public Framebuffer ShadowMapRenderTargetTexture { get => shadowMapFramBuffer; set => shadowMapFramBuffer = value; }
    //    public float LightPower { get => lightPower; set => lightPower = value; }
    //    public float Ambient { get => ambient; set => ambient = value; }
    //    public Matrix4x4 LightsViewProjectionMatrix4x4 { get => lightsViewProjectionMatrix; set => lightsViewProjectionMatrix = value; }
    //    public Vector4 LightColor { get => lightColor; set => lightColor = value; }

    //    public void RecalculateMatrix()
    //    {
    //        var dir = -position;
    //        dir.Y = 2;
    //        dir = Vector3.Normalize(dir);
    //        Matrix4x4 view = Matrix4x4.CreateLookAt(position, LookAtPoint, new Vector3(0, 0, 1));
    //        Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(MathUtil.DegreesToRadians(45f), 1f, 0.001f, 100f);
    //        lightsViewProjectionMatrix = view * projection;
    //    }

    }
}
