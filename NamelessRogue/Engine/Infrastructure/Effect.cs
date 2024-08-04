using System;
using System.IO;
using System.Linq;
using System.Text;


namespace NamelessRogue.Engine.Infrastructure
{
    public class Effect
    {
        public Effect(object device, string filePath, string vertexEntryPoint = "", string pixelEntrypoint = "", string hullEntryPoint = "", string geometryEntryPoint = "")
        {
            //if (filePath == null || !filePath.Any())
            //{
            //    throw new ArgumentNullException("filePath");
            //}

            //var shaderText = File.ReadAllText(filePath);
            //ShaderName = filePath;

            //Shader _loadShader(string entryPoint, ShaderStages stage)
            //{
            //    if (entryPoint == null || !entryPoint.Any()) { return null; }
            //    ShaderDescription desc = new ShaderDescription(stage, Encoding.UTF8.GetBytes(shaderText), entryPoint, true);
            //    return device.ResourceFactory.CreateShader(desc);
            //}
            //VertexShader = _loadShader(vertexEntryPoint, ShaderStages.Vertex);
            //PixelShader = _loadShader(pixelEntrypoint, ShaderStages.Fragment);
            //HullShader = _loadShader(hullEntryPoint, ShaderStages.TessellationControl);
            //GeometryShader = _loadShader(geometryEntryPoint, ShaderStages.Geometry);

        }

        //public string ShaderName { get; set; } = "";
        //public Shader VertexShader { get; set; } = null;
        //public Shader PixelShader { get; set; } = null;
        //public Shader HullShader { get; set; } = null;
        //public Shader GeometryShader { get; set; } = null;

        ////public void Apply(DeviceContext context)
        ////{
        //// //   context.PixelShader.Set(PixelShader);
        ////  //  context.VertexShader.Set(VertexShader);
        ////    //context.HullShader.Set(HullShader);
        ////    //context.GeometryShader.Set(GeometryShader);
        ////}
    }
}
