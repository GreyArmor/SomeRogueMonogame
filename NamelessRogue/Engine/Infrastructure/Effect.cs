using System;
using System.Linq;
using Veldrid;

namespace NamelessRogue.Engine.Infrastructure
{
    public class Effect
    {
        public Effect(GraphicsDevice device, string filePath, string vertexEntryPoint = "", string pixelEntrypoint = "", string hullEntryPoint = "", string geometryEntryPoint = "") {
            //if (filePath == null || !filePath.Any())
            //{
            //    throw new ArgumentNullException("filePath");
            //}
            //ShaderName = filePath;
            //CompilationResult _loadShader(string entryPoint, string profile)
            //{
            //    if (entryPoint == null || !entryPoint.Any()) { return null; }
               
            //    return ShaderBytecode.CompileFromFile(filePath, entryPoint, profile, ShaderFlags.Debug, EffectFlags.None);
            //}
            //var bytecode = _loadShader(vertexEntryPoint, "vs_4_0");
            //if (bytecode != null)
            //{
            //    VSBytecode = bytecode;
            //    VertexShader = new VertexShader(device, bytecode); 
            //}
            //bytecode = _loadShader(pixelEntrypoint, "ps_4_0");
            //if (bytecode != null)
            //{
            //    PixelShader = new PixelShader(device, bytecode);
            //}
            //bytecode = _loadShader(hullEntryPoint, "");
            //if (bytecode != null)
            //{
            //    HullShader = new HullShader(device, bytecode);
            //}
            //bytecode = _loadShader(geometryEntryPoint, "");
            //if (bytecode != null)
            //{
            //    GeometryShader = new Shader(device, bytecode);
            //}
        }

        public string ShaderName { get; set; }

        public Shader VSBytecode { get;set; }
        public Shader VertexShader { get; set; } = null;
        public Shader PixelShader { get; set; } = null;
        public Shader HullShader { get; set; } = null;
        public Shader GeometryShader { get; set; } = null;

        //public void Apply(DeviceContext context)
        //{
        //    context.PixelShader.Set(PixelShader);
        //    context.VertexShader.Set(VertexShader);
        //    //context.HullShader.Set(HullShader);
        //    //context.GeometryShader.Set(GeometryShader);
        //}
    }
}
