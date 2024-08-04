using NamelessRogue.Engine.Systems.Ingame;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;


namespace NamelessRogue.Engine.Components
{
    public class TerrainGeometry3D : Component, IDisposable
    {
        public object Buffer { get; set; }
        public Matrix4x4 WorldOffset { get; set; } = Matrix4x4.Identity;
        public int VerticesCount { get; internal set; }

        public void Dispose()
        {
          //  Buffer?.Dispose();
        }
    }
}
