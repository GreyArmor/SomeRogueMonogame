using SharpDX;
using NamelessRogue.Engine.Systems.Ingame;
using System.Collections.Generic;
using System.Text;
using SharpDX.Direct3D11;
using System;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace NamelessRogue.Engine.Components
{
    public class TerrainGeometry3D : Component, IDisposable
    {
        public Buffer Buffer { get; set; }
        public Matrix WorldOffset { get; set; } = Matrix.Identity;
        public int VerticesCount { get; internal set; }

        public void Dispose()
        {
            Buffer?.Dispose();
        }
    }
}
