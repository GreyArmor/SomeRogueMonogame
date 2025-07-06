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
        public int zoomIndex = 6;
        public Vector2 Position { get; }
        public int Zoom { get => zoom; set => zoom = value; }
        public int ZoomIndex
        {
            get => zoomIndex;
            
            set
            {
                zoomIndex = value switch
                {
                    > 6 => 6,
                    < 0 => 0,
                    _ => value,
                };
            }
        }

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
