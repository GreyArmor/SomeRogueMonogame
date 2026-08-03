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
        CrossingTop,
        CrossingBottom,
        CrossingLeft,
        CrossingRight,
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
        public List<MacroConnection> NeighborConnectionPaths { get; internal set; } = new List<MacroConnection>();
        public NodeType Type { get; set; } = NodeType.None;

        public bool IsCrossingNode()
        {
            return Type == NodeType.CrossingTop || Type == NodeType.CrossingBottom || Type == NodeType.CrossingLeft || Type == NodeType.CrossingRight;
        }

        public List<MacroConnection> GetCrossingConnections()
        {
            return NeighborConnectionPaths.Where(x => x.Node.IsCrossingNode()).ToList();
        }
    }

    public class MacroConnection
    {
        public MacroNode Node { get; set; }
        public int PathId { get; set; }
        public int Distance { get; set; }
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

            //first connect the internal nodes of locations to each other
            ConnectLocationIntialInternalNodes(chSize);

            //then connect crossings to each other
            ConnectCrossings();

            //then to locations
            ConnectCrossingsToLocations();

            //connect internal nodes of locations
            ConnectLocationInternalNodes(chSize);
            //connect corners of overlapping locations
            ConnectOverlappingLocations();
        }

        private void ConnectCrossingsToLocations()
        {
            foreach (var crossing in Crossings)
            {
                List<MacroLocation> overlappingLocations = new List<MacroLocation>();
                //fist get all overlapping locations for this crossing
                foreach (var otherLocation in Locations)
                {
                    if (crossing == otherLocation) continue;
                    if (crossing.BoundingBox.Intersects(otherLocation.BoundingBox))
                    {
                        if (otherLocation.InternalNodes.Any())
                        {
                            overlappingLocations.Add(otherLocation);
                        }
                    }
                }

                var crossingCenter = (crossing.BoundingBox.Min + crossing.BoundingBox.Max) / 2;

                if (crossing.Type == LocationType.CrossingHorizontal)
                {
                    foreach (var overlappingLocation in overlappingLocations)
                    {
                        //first, we pick nodes to copy from the crossing, based on the location's position relative to the crossing
                        var locationCentre = (overlappingLocation.BoundingBox.Min + overlappingLocation.BoundingBox.Max) / 2;
                        var displacementVector = new Point();
                        var crossingNodes = new List<MacroNode>();
                        string connectionId;
                        if (crossing.Type == LocationType.CrossingHorizontal)
                        {
                            //if location is left of crossing, connect right side of location to left side of crossing
                            if (locationCentre.X < crossingCenter.X)
                            {
                                displacementVector = new Point(-1, 0);
                                crossingNodes = crossing.InternalNodes.Where(x => x.Type == NodeType.CrossingLeft).ToList();
                                connectionId = "crossingConnectorLeft";
                            }
                            else
                            {
                                displacementVector = new Point(1, 0);
                                crossingNodes = crossing.InternalNodes.Where(x => x.Type == NodeType.CrossingRight).ToList();
                                connectionId = "crossingConnectorRight";
                            }
                        }
                        else if (crossing.Type == LocationType.CrossingVertical)
                        {
                            //if location is above crossing, connect bottom side of location to top side of crossing
                            if (locationCentre.Y < crossingCenter.Y)
                            {
                                displacementVector = new Point(0, -1);
                                crossingNodes = crossing.InternalNodes.Where(x => x.Type == NodeType.CrossingTop).ToList();
                                connectionId = "crossingConnectorTop";
                            }
                            else
                            {
                                displacementVector = new Point(0, 1);
                                crossingNodes = crossing.InternalNodes.Where(x => x.Type == NodeType.CrossingBottom).ToList();
                                connectionId = "crossingConnectorBottom";
                            }
                        }
                        //then each node is copied to the building, and a connection is made between the copied node and the original node in the crossing
                        var newLocationNodes = new List<MacroNode>();
                        foreach (var crossingNode in crossingNodes)
                        {
                            var newLocationNode = new MacroNode
                            {
                                Id = "crossingConnectorLeft",
                                //move to the left
                                RealityPosition = new Vector3Int(crossingNode.RealityPosition.X + displacementVector.X, crossingNode.RealityPosition.Y, crossingNode.RealityPosition.Z),
                                Type = NodeType.CrossingLeft
                            };

                            newLocationNodes.Add(newLocationNode);

                            crossingNode.NeighborConnectionPaths.Add(new MacroConnection
                            {
                                Node = newLocationNode,
                                PathId = game.PathfindingController.CalculateToPointAStar(crossingNode.RealityPosition.ToPoint(), newLocationNode.RealityPosition.ToPoint()),
                                Distance = 1
                            });

                            newLocationNode.NeighborConnectionPaths.Add(new MacroConnection
                            {
                                Node = crossingNode,
                                PathId = game.PathfindingController.CalculateToPointAStar(newLocationNode.RealityPosition.ToPoint(), crossingNode.RealityPosition.ToPoint()),
                                Distance = 1
                            });
                            overlappingLocation.InternalNodes.Add(newLocationNode);
                        }
                    }
                }
            }
        }

        //nodes before we connect them to crossings
        private void ConnectLocationIntialInternalNodes(int chSize)
        {
            foreach (var location in Locations)
            {
                //link them in a loop
                for (int i = 0; i < location.InternalNodes.Count(); i++)
                {
                    var currentNode = location.InternalNodes[i];
                    var nextNode = location.InternalNodes[(i + 1) % location.InternalNodes.Count()];
                    var previousNode = location.InternalNodes[(i - 1 + location.InternalNodes.Count()) % location.InternalNodes.Count()];
                    currentNode.NeighborConnectionPaths = new List<MacroConnection>
                    {
                        new MacroConnection { Node = nextNode, PathId = -1 , Distance = Vector3Int.Distance(currentNode.RealityPosition, nextNode.RealityPosition)},
                        new MacroConnection { Node = previousNode, PathId = -1 , Distance = Vector3Int.Distance(currentNode.RealityPosition, previousNode.RealityPosition)},
                    };
                }
            }
        }

        //connects the internal nodes of locations to each other
        private void ConnectLocationInternalNodes(int chSize)
        {
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
                        //look at connections, and if the connected node is in the same location, then we can set the path id for that connection
                        foreach (var neighbor in node.NeighborConnectionPaths)
                        {
                            var connection = neighbor.Node.NeighborConnectionPaths.FirstOrDefault(x => x.Node == node);
                            if(connection != null && location.InternalNodes.Contains(connection.Node))
                            {
                                connection.PathId = flowId;
                            }
                        }
                    }
                }
            }
        }

        private void ConnectCrossings()
        {
            foreach (var crossing in Crossings)
            {
                List<MacroLocation> overlappingCrossings = new List<MacroLocation>();
                foreach (var overlappingCrossing in Crossings)
                {
                    if (crossing == overlappingCrossing) continue;
                    if (crossing.BoundingBox.Intersects(overlappingCrossing.BoundingBox))
                    {
                        overlappingCrossings.Add(overlappingCrossing);
                    }
                }

                foreach (var overlappingCrossing in overlappingCrossings)
                {
                    if (crossing.Type == LocationType.CrossingHorizontal && overlappingCrossing.Type == LocationType.CrossingHorizontal)
                    {
                        ConnectCrossingPair(crossing, overlappingCrossing);
                    }
                }

            }
        }

        private void ConnectOverlappingLocations()
        {
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

                            if (distance <= 2)
                            {
                                var pathId = game.PathfindingController.CalculateToPointAStar(node.RealityPosition.ToPoint(), overlappingNode.RealityPosition.ToPoint());
                                node.NeighborConnectionPaths.Add(new MacroConnection { Node = overlappingNode, PathId = pathId, Distance = distance });
                            }
                        }
                    }
                }
            }
        }

        private void ConnectCrossingPair(MacroLocation locationA, MacroLocation locationB)
        {
            foreach (var nodeA in locationA.InternalNodes)
            {
                foreach (var nodeB in locationB.InternalNodes)
                {
                    //we use float specifically here because we want only to connect nodes that are aligned on grid
                    var distance = Vector3.Distance(nodeA.RealityPosition.ToVector3(), nodeB.RealityPosition.ToVector3());
                    if (distance == 1)
                    {
                        var pathId = game.PathfindingController.CalculateToPointAStar(nodeA.RealityPosition.ToPoint(), nodeB.RealityPosition.ToPoint());
                        nodeA.NeighborConnectionPaths.Add(new MacroConnection { Node = nodeB, PathId = pathId, Distance = (int)distance });
                    }
                }
            }
        }

        private void ConnectLocationPair(MacroLocation locationA, MacroLocation locationB)
        {
            foreach (var nodeA in locationA.InternalNodes)
            {
                foreach (var nodeB in locationB.InternalNodes)
                {
                    var distance = Vector3Int.Distance(nodeA.RealityPosition, nodeB.RealityPosition);
                    if (distance <= 2)
                    {
                        var pathId = game.PathfindingController.CalculateToPointAStar(nodeA.RealityPosition.ToPoint(), nodeB.RealityPosition.ToPoint());
                        nodeA.NeighborConnectionPaths.Add(new MacroConnection { Node = nodeB, PathId = pathId, Distance = distance });
                    }
                }
            }
        }
    }
}
