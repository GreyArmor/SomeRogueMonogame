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
        public int FlowFieldId { get; internal set; }
        public List<MacroNode> Neighbors { get; internal set; }
        public NodeType Type { get; set; } = NodeType.None;
    }
    public class MacroNavigator
    {
        private NamelessGame game;

        static Dictionary<NodeType, NodeType> nodeTypeMappingsHorizontal = new Dictionary<NodeType, NodeType>() {
            { NodeType.PedestrianEntranceNW, NodeType.PedestrianEntranceSW },
            { NodeType.PedestrianEntranceNE, NodeType.PedestrianEntranceSE },
            { NodeType.PedestrianEntranceSW, NodeType.PedestrianEntranceNW },
            { NodeType.PedestrianEntranceSE, NodeType.PedestrianEntranceNE },
        };
        static Dictionary<NodeType, NodeType> nodeTypeMappingsVertical  = new Dictionary<NodeType, NodeType>() {
            { NodeType.PedestrianEntranceNW, NodeType.PedestrianEntranceNE },
            { NodeType.PedestrianEntranceNE, NodeType.PedestrianEntranceNW },
            { NodeType.PedestrianEntranceSW, NodeType.PedestrianEntranceSE },
            { NodeType.PedestrianEntranceSE, NodeType.PedestrianEntranceSW },
        };
        public List<MacroLocation> Locations = new List<MacroLocation>();
        public List<MacroLocation> Crossings = new List<MacroLocation>();

        public MacroNavigator(NamelessGame game)
        {
            this.game = game;
        }    
        public void AnalyzeAndConnectLocations()
        {
            foreach (var location in Locations)
            {
                //link them in a loop
                for (int i = 0; i < location.InternalNodes.Count(); i++)
                {
                    var currentNode = location.InternalNodes[i];
                    var nextNode = location.InternalNodes[(i + 1) % location.InternalNodes.Count()];
                    var previousNode = location.InternalNodes[(i - 1 + location.InternalNodes.Count()) % location.InternalNodes.Count()];
                    currentNode.Neighbors = new List<MacroNode>() { previousNode, nextNode };
                }

            }

            //foreach (var crossing in Crossings)
            //{
            //    List<MacroLocation> overlappingLocations = new List<MacroLocation>();
            //    foreach (var otherLocation in Locations)
            //    {
            //        if (crossing == otherLocation) continue;
            //        if (crossing.BoundingBox.Intersects(otherLocation.BoundingBox))
            //        {
            //            if (otherLocation.InternalNodes.Any())
            //            {
            //                overlappingLocations.Add(otherLocation);
            //            }
            //        }
            //    }
            //    // Connect the crossing to the overlapping locations
            //    foreach (var overlappingLocation in overlappingLocations)
            //    {
            //        foreach (var node in overlappingLocation.InternalNodes)
            //        {
            //            var nodeTypeMappings = crossing.Type == LocationType.CrossingVertical ? nodeTypeMappingsVertical : nodeTypeMappingsHorizontal;

            //            if (nodeTypeMappings.TryGetValue(node.Type, out NodeType complementaryType))
            //            {
            //                var matchingNode = overlappingLocation.InternalNodes.FirstOrDefault(n => n.Type == complementaryType);
            //                if (matchingNode != null)
            //                {
            //                    // Connect the nodes
            //                    node.Neighbors.Add(matchingNode);
            //                    matchingNode.Neighbors.Add(node);
            //                }
            //            }
            //        }
            //    }
            //}

            foreach (var location in Locations)
            {
                foreach (var node in location.InternalNodes)
                {
                    if (node.Neighbors.Any())
                    {
                        var chSize = Constants.ChunkSize;
                        var boundingBox = location.BoundingBox;
                        var min = new Point((int)(boundingBox.Min.X/ chSize), (int)(boundingBox.Min.Y / chSize));
                        var max = new Point((int)(boundingBox.Max.X / chSize), (int)(boundingBox.Max.Y / chSize));
                        var flowId = game.PathfindingController.CalculateToForArea(node.RealityPosition.ToPoint(), min, max);
                        node.FlowFieldId = flowId;
                    }
                }
            }
        }
            
    }
}
