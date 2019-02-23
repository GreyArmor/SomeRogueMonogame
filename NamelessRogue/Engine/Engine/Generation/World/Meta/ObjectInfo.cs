using Microsoft.Xna.Framework;

namespace NamelessRogue.Engine.Engine.Generation.World.Meta
{
    public class ObjectInfo
    {
        public string Name { get; set; }
        public ProductionValue ProductionModifier { get; set; } = new ProductionValue(0,0,0,0,0,0);
        public Point MapPosition { get; set; }
    }
}
