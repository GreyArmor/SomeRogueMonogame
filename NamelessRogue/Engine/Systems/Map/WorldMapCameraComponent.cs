using NamelessRogue.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Map
{
    public class WorldMapCameraComponent : Component
    {
        int zoom;

        public Vector2 Position { get; }
        public int Zoom { get => zoom; set => zoom = value; }

        public WorldMapCameraComponent(Vector2 position, int zoom)
        {
            Position = position;
            Zoom = zoom;
        }

        public override IComponent Clone()
        {
            return new WorldMapCameraComponent(Position, Zoom);
        }

    }
}
