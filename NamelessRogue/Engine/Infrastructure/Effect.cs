using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System;
using System.Linq;

namespace NamelessRogue.Engine.Infrastructure
{
    public class Effect
    {
        public Effect(Device device, string filePath, string vertexEntryPoint = "", string pixelEntrypoint = "", string hullEntryPoint = "", string geometryEntryPoint = "") {
            if (filePath == null || !filePath.Any())
            {
                throw new ArgumentNullException("filePath");
            }
            ShaderName = filePath;
            CompilationResult _loadShader(string entryPoint, string profile)
            {
                if (entryPoint == null || !entryPoint.Any()) { return null; }
               
                return ShaderBytecode.CompileFromFile(filePath, entryPoint, profile, ShaderFlags.Debug, EffectFlags.None);
            }
            var bytecode = _loadShader(vertexEntryPoint, "vs_4_0");
            if (bytecode != null)
            { 
                VertexShader = new VertexShader(device, bytecode); 
            }
            bytecode = _loadShader(pixelEntrypoint, "ps_4_0");
            if (bytecode != null)
            {
                PixelShader = new PixelShader(device, bytecode);
            }
            bytecode = _loadShader(hullEntryPoint, "");
            if (bytecode != null)
            {
                HullShader = new HullShader(device, bytecode);
            }
            bytecode = _loadShader(geometryEntryPoint, "");
            if (bytecode != null)
            {
                GeometryShader = new GeometryShader(device, bytecode);
            }
        }

        public string ShaderName { get; set; }

        public VertexShader VertexShader { get; set; } = null;
        public PixelShader PixelShader { get; set; } = null;
        public HullShader HullShader { get; set; } = null;
        public GeometryShader GeometryShader { get; set; } = null;

        public void Apply(DeviceContext context)
        {
            context.PixelShader.Set(PixelShader);
            context.VertexShader.Set(VertexShader);
            context.HullShader.Set(HullShader);
            context.GeometryShader.Set(GeometryShader);
        }
    }
}
