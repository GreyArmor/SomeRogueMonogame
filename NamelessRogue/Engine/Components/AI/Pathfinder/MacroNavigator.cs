using Microsoft.CodeAnalysis;
using NamelessRogue.Engine.Components.AI.Pathfinder;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace NamelessRogue.Engine.Components.AI.Pathfinder
{
    public enum NodeType
    {
        None,
        PedestrianEntranceNW,
        PedestrianEntranceNE,
        PedestrianEntranceSW,
        PedestrianEntranceSE,
        StairsUp,
        StairsDown,
    }

    public enum LocationType
    {
        None,
        Building,
        CrossingVertical,
        CrossingHorizontal,
    }

    public class MacroLocation
    {
        public string Id { get; set; }
        public Vector3Int BuildingPosition { get; set; }
        public Microsoft.Xna.Framework.BoundingBox BoundingBox { get; set; }
        public Vector3Int RealityPosition { get; set; } = Vector3Int.Zero;
        public Vector3Int ChunkPosition { get; set; } = Vector3Int.Zero;
        public List<MacroNode> InternalNodes { get; set; } = new List<MacroNode>();
        public LocationType Type { get; internal set; }
    }

    public class MacroNode
    {
        public string Id { get; set; }
        public Vector3Int RealityPosition { get; set; } = Vector3Int.Zero;
        public int LocationPathId { get; internal set; }
        public Dictionary<MacroNode, int> NeighborConnectionPaths { get; internal set; }
        public NodeType Type { get; set; } = NodeType.None;
    }


    public class MacroNavigator
    {
        private NamelessGame game;
        public List<MacroLocation> Locations = new List<MacroLocation>();
        public List<MacroLocation> Crossings = new List<MacroLocation>();

        public MacroNavigator(NamelessGame game)
        {
            this.game = game;
        }    
        public void AnalyzeAndConnectLocations()
        {


            var chSize = Constants.ChunkSize;

            foreach (var location in Locations)
            {
                //link them in a loop
                for (int i = 0; i < location.InternalNodes.Count(); i++)
                {
                    var currentNode = location.InternalNodes[i];
                    var nextNode = location.InternalNodes[(i + 1) % location.InternalNodes.Count()];
                    var previousNode = location.InternalNodes[(i - 1 + location.InternalNodes.Count()) % location.InternalNodes.Count()];
                    currentNode.NeighborConnectionPaths = new Dictionary<MacroNode, int>
                    {
                        { nextNode, -1 },
                        { previousNode, -1 }
                    };
                }
            }


            foreach (var location in Locations)
            {
                foreach (var node in location.InternalNodes)
                {
                    if (node.NeighborConnectionPaths.Any())
                    {
                        var boundingBox = location.BoundingBox;
                        var min = new Point((int)(boundingBox.Min.X / chSize), (int)(boundingBox.Min.Y / chSize)) + new Point(-1, -1);
                        var max = new Point((int)(boundingBox.Max.X / chSize), (int)(boundingBox.Max.Y / chSize)) + new Point(1, 1);
                        var flowId = game.PathfindingController.CalculateToForArea(node.RealityPosition.ToPoint(), min, max);
                        node.LocationPathId = flowId;
                        foreach (var neighbor in node.NeighborConnectionPaths.Keys.ToList())
                        {
                            neighbor.NeighborConnectionPaths[node] = flowId;
                        }
                    }
                }
            }

            //connect corners of overlapping locations
            foreach (var location in Locations)
            {
                List<MacroLocation> overlappingLocations = new List<MacroLocation>();
                foreach (var otherLocation in Locations)
                {
                    if (location == otherLocation) continue;
                    if (location.BoundingBox.Intersects(otherLocation.BoundingBox))
                    {
                        if (otherLocation.InternalNodes.Any())
                        {
                            overlappingLocations.Add(otherLocation);
                        }
                    }
                }
                foreach (var overlappingLocation in overlappingLocations)
                {
                    foreach (var node in location.InternalNodes)
                    {
                        foreach (var overlappingNode in overlappingLocation.InternalNodes)
                        {
                            var distance = Vector3Int.Distance(node.RealityPosition, overlappingNode.RealityPosition);

                            if (distance <= 5)
                            {
                                var pathId = game.PathfindingController.CalculateToPointAStar(node.RealityPosition.ToPoint(), overlappingNode.RealityPosition.ToPoint());
                                node.NeighborConnectionPaths[overlappingNode] = pathId;
                            }
                        }
                    }
                }            
            }
        }
    }
}
