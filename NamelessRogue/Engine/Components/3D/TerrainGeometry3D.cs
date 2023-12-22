using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.Engine.Systems.Ingame;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Components
{
    public class TerrainGeometry3D : Component, IDisposable
    {
        public VertexBuffer Buffer { get; set; }
        public Matrix WorldOffset { get; set; } = Matrix.Identity;
        public int VerticesCount { get; internal set; }

        public void Dispose()
        {
            Buffer?.Dispose();
        }
    }
}
