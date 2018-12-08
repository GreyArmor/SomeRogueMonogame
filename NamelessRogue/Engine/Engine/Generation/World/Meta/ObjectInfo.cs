using Microsoft.Xna.Framework;

namespace NamelessRogue.Engine.Engine.Generation.World.Meta
{
    public class ObjectInfo
    {
        public string Name { get; set; }
        public ProductionValue ProductionModifier { get; set; }
        public Point MapPosition { get; }
    }
}
