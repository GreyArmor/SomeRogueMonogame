using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems._3DView
{
    internal class LightSource
    {
        //TextureView shadowMap;
        //TextureView shadowMapRenderTargetTexture;
        //Vector3 position;
        //Vector3 lookAtPoint;
        //Matrix4x4 lightsViewProjectionMatrix;
        //Vector4 lightColor;
        //int frustrumHeight;
        //int frustrumWidth;
        //float lightPower;
        //float ambient;
        //public LightSource(NamelessGame game, int frustrumWidth, int frustrumHeight, Vector3 position, Vector3 lookAtPoint, Vector4 lightColor)
        //{
        //    var textureDescription = new TextureViewDescription()
        //    {
        //        Height = frustrumHeight,
        //        Width = frustrumWidth,
        //        CpuAccessFlags = CpuAccessFlags.Read,
        //        Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm_SRgb,
        //        Usage = ResourceUsage.Default,
        //        ArraySize = 1,
        //        BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
        //        OptionFlags = ResourceOptionFlags.None,
        //        MipLevels = 0,
        //        SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0)
        //    };

        //    shadowMapRenderTargetTexture = new TextureView(game.GraphicsDevice, textureDescription);
        //    RenderTargetViewDescription renderTargetViewDesc = new RenderTargetViewDescription()
        //    {
        //        Format = textureDescription.Format,
        //        Dimension = RenderTargetViewDimension.TextureView,               
        //    };

        //    ShadowMapRenderTargetView = new RenderTargetView(game.Window.Device, shadowMapRenderTargetTexture, renderTargetViewDesc);
        //    ShadowMapResourceView = new ShaderResourceView(game.Window.Device, shadowMapRenderTargetTexture);

        //    this.position = position;
        //    this.LookAtPoint = lookAtPoint;
        //    this.lightColor = lightColor;
        //    this.frustrumHeight = frustrumHeight;
        //    this.frustrumWidth = frustrumWidth;
        //    Matrix4x4 view = Matrix4x4.LookAtLH(position, lookAtPoint, new Vector3(0, 0, 1));
        //    Matrix4x4 projection = Matrix4x4.PerspectiveFovLH(MathUtil.DegreesToRadians(45f), 1f, 0.001f, 100f);
        //    lightsViewProjectionMatrix4x4 = view * projection;
        //}

        //public Vector3 Position { get => position; set => position = value; }
        //public Vector3 LookAtPoint { get => lookAtPoint; set => lookAtPoint = value; }
        //public TextureView ShadowMapRenderTargetTexture { get => shadowMapRenderTargetTexture; set => shadowMapRenderTargetTexture = value; }
        //public RenderTargetView ShadowMapRenderTargetView { get; set; }
        //public ShaderResourceView ShadowMapResourceView { get; set; }
        //public float LightPower { get => lightPower; set => lightPower = value; }
        //public float Ambient { get => ambient; set => ambient = value; }
        //public Matrix4x4 LightsViewProjectionMatrix4x4 { get => lightsViewProjectionMatrix; set => lightsViewProjectionMatrix4x4 = value; }
        //public Vector4 LightColor { get => lightColor; set => lightColor = value; }

        //public void RecalculateMatrix()
        //{
        //    var dir = -position;
        //    dir.Y = 2;
        //    dir.Normalize();
        //    Matrix4x4 view = Matrix4x4.LookAtLH (position, LookAtPoint, new Vector3(0, 0, 1));
        //    Matrix4x4 projection = Matrix4x4.PerspectiveFovLH(MathUtil.DegreesToRadians(45f), 1f, 0.001f, 100f);
        //    lightsViewProjectionMatrix4x4 = view * projection;
        //}

    }
}
