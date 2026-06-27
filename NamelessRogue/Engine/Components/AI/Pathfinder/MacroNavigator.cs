using NamelessRogue.Engine.Components.AI.Pathfinder;
using NamelessRogue.Engine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.AI.Pathfinder
{
    public class MacroLocation
    {
        public string Id { get; set; }
        public Vector3Int BuildingPosition { get; set; }
        public Microsoft.Xna.Framework.BoundingBox BoundingBox { get; set; }
        public List<MacroLocation> Neighbors { get; set; } = new List<MacroLocation>();
        public Vector3Int RealityPosition { get; set; } = Vector3Int.Zero;
        public Vector3Int ChunkPosition { get; set; } = Vector3Int.Zero;
        public List<MacroNode> Nodes { get; set; } = new List<MacroNode>();
    }

    public class MacroNode
    {
        public string Id { get; set; }
        public Vector3Int RealityPosition { get; set; } = Vector3Int.Zero;
        public int FlowFieldId { get; internal set; }
        public List<MacroNode> Neighbors { get; internal set; }
    }
    public class MacroNavigator
    {
        public List<MacroLocation> Locations = new List<MacroLocation>();
    }
}
