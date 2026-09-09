using AStarNavigator;
using AStarNavigator.Algorithms;
using AStarNavigator.Providers;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.AI.Pathfinder;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TiledCSPlus;
using static NamelessRogue.Engine.Components.AI.NonPlayerCharacter.AStarPathfinderSimple;
using BoundingBox = NamelessRogue.Engine.Utility.BoundingBox;

namespace NamelessRogue.Engine.Components.AI.Pathfinder
{
    public enum Direction
    {
        None, North, South, East, West, NorthEast, NorthWest, SouthEast, SouthWest

    }
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
        public BoundingBox BoundingBox { get; set; }
        public Vector3Int RealityPosition { get; set; } = Vector3Int.Zero;
        public Vector3Int ChunkPosition { get; set; } = Vector3Int.Zero;
        public List<MacroNode> InternalNodes { get; set; } = new List<MacroNode>();
        public LocationType Type { get; internal set; }
        public BoundingBox CrossingBox { get; internal set; }
    }
    public class MacroNode
    {
        public MacroNode(MacroLocation parent) {
            ParentLocation = parent;
        }
        public string Id { get; set; }
        public Vector3Int RealityPosition { get; set; } = Vector3Int.Zero;
        public int LocationPathId { get; internal set; }
        public List<MacroConnection> NeighborConnectionPaths { get; internal set; } = new List<MacroConnection>();
        public NodeType Type { get; set; } = NodeType.None;
        public MacroLocation ParentLocation { get; set; }

        public bool IsCrossingNode()
        {
            return Type == NodeType.CrossingTop || Type == NodeType.CrossingBottom || Type == NodeType.CrossingLeft || Type == NodeType.CrossingRight;
        }

        public bool IsCrossingNodeVertical()
        {
            return Type == NodeType.CrossingTop || Type == NodeType.CrossingBottom;
        }

        public bool IsCrossingNodeHorizontal()
        {
            return Type == NodeType.CrossingLeft || Type == NodeType.CrossingRight;
        }

        public List<MacroConnection> GetCrossingConnections()
        {
            return NeighborConnectionPaths.Where(x => x.Node.IsCrossingNode()).ToList();
        }

        public static bool IsOppositeCrossing(MacroNode first, MacroNode second)
        {

            bool _isOpposite(MacroNode a, MacroNode b)
            {
                if (a.Type == NodeType.CrossingTop && b.Type == NodeType.CrossingBottom)
                {
                    return true;
                }
                if (a.Type == NodeType.CrossingLeft && b.Type == NodeType.CrossingRight)
                {
                    return true;
                }
                return false;
            }

            if (_isOpposite(first, second) || _isOpposite(second, first))
            {
                return true;
            }

            return false;
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
            ConnectLocationInitialInternalNodes(chSize);

            CreateCrossingBounds();

            CreateCrossingNodes();

            //then to locations
            ConnectCrossingsToLocations();

            //connect internal nodes of locations
            ConnectLocationInternalNodes(chSize);
            //connect corners of overlapping locations
           // ConnectOverlappingLocations();
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
                    if (crossing.BoundingBox.Neighboring(otherLocation.BoundingBox))
                    {
                        if (otherLocation.InternalNodes.Any())
                        {
                            overlappingLocations.Add(otherLocation);
                        }
                    }
                }

                var crossingCenter = (crossing.BoundingBox.Min + crossing.BoundingBox.Max) / 2;      


                foreach (var overlappingLocation in overlappingLocations)
                {
                    //first, we pick nodes to copy from the crossing, based on the location's position relative to the crossing
                    var locationCenter = (overlappingLocation.BoundingBox.Min + overlappingLocation.BoundingBox.Max) / 2;
                    var crossingNodes = new List<MacroNode>();
                    if (crossing.Type == LocationType.CrossingHorizontal)
                    {
                        //if location is left of crossing, connect right side of location to left side of crossing
                        if (locationCenter.X < crossingCenter.X)
                        {
                            crossingNodes = crossing.InternalNodes.Where(x => x.Type == NodeType.CrossingLeft).ToList();
                        }
                        else
                        {
                            crossingNodes = crossing.InternalNodes.Where(x => x.Type == NodeType.CrossingRight).ToList();
                        }
                    }
                    else if (crossing.Type == LocationType.CrossingVertical)
                    {
                        //if location is above crossing, connect bottom side of location to top side of crossing
                        if (locationCenter.Y < crossingCenter.Y)
                        {
                            crossingNodes = crossing.InternalNodes.Where(x => x.Type == NodeType.CrossingTop).ToList();
                        }
                        else
                        {
                            crossingNodes = crossing.InternalNodes.Where(x => x.Type == NodeType.CrossingBottom).ToList();
                        }
                    }
                    
                    foreach (var crossingNode in crossingNodes)
                    {
                        crossingNode.ParentLocation = overlappingLocation;
                    }

                    //add the crossing nodes to the overlapping location's internal nodes, and connect them to the existing internal nodes
                    //foreach (var internalNode in overlappingLocation.InternalNodes)
                    //{
                    //    internalNode.NeighborConnectionPaths.AddRange(crossingNodes.Select(x => new MacroConnection { Node = x, PathId = -1, Distance = Vector3Int.Distance(internalNode.RealityPosition, x.RealityPosition) }));
                    //}
                    //foreach (var crossingNode in crossingNodes)
                    //{
                    //    crossingNode.NeighborConnectionPaths.AddRange(overlappingLocation.InternalNodes.Select(x => new MacroConnection { Node = x, PathId = -1, Distance = Vector3Int.Distance(crossingNode.RealityPosition, x.RealityPosition) }));
                    //}
                    overlappingLocation.InternalNodes.AddRange(crossingNodes);
                    //
                   
                }
            }
        }

        //nodes before we connect them to crossings
        private void ConnectLocationInitialInternalNodes(int chSize)
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
                            if (connection != null && location.InternalNodes.Contains(connection.Node) && !connection.Node.IsCrossingNode())
                            {
                                connection.PathId = flowId;
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

                    if(!nodeA.IsCrossingNode() || !nodeB.IsCrossingNode())
                    {
                        continue;
                    }
                    //we use float specifically here because we want only to connect nodes that are aligned on grid
                    var distance = Vector3.Distance(nodeA.RealityPosition.ToVector3(), nodeB.RealityPosition.ToVector3());
                    if (distance == 1)
                    {
                        var pathId = game.PathfindingController.CalculateToPointStraightLine(nodeA.RealityPosition.ToPoint(), nodeB.RealityPosition.ToPoint());
                        nodeA.NeighborConnectionPaths.Add(new MacroConnection { Node = nodeB, PathId = pathId, Distance = (int)distance });
                    }
                }
            }
        }


        public List<MacroNode> Plot(MacroNode from, MacroNode to)
        {
            List<MacroNode> path = new List<MacroNode>();
               return path;
        }

        public void MergeCrossings()
        {

        }

        private void CreateCrossingBounds()
        {
            List<MacroLocation> mergedCrossings = new List<MacroLocation>();
            List<MacroLocation> processedLocations= new List<MacroLocation>();
            foreach (var crossing in Crossings)
            {
                if(processedLocations.Contains(crossing)) continue;
                List<MacroLocation> overlappingCrossings = new List<MacroLocation>();
                BoundingBox mergedBounds = new BoundingBox();
                BoundingBox mergedCrossing = new BoundingBox();
                foreach (var overlappingCrossing in Crossings)
                {
                    if (crossing == overlappingCrossing) continue;
                    if (crossing.BoundingBox.Neighboring(overlappingCrossing.BoundingBox) && crossing.Type == overlappingCrossing.Type)
                    {
                        overlappingCrossings.Add(overlappingCrossing);
                        processedLocations.Add(overlappingCrossing);
                    }
                }

                mergedBounds = crossing.BoundingBox;
                mergedCrossing = crossing.CrossingBox;
                foreach (var overlappingCrossing in overlappingCrossings)
                {
                    mergedBounds = BoundingBox.CreateMerged(mergedBounds, overlappingCrossing.BoundingBox);
                    mergedCrossing = BoundingBox.CreateMerged(mergedCrossing, overlappingCrossing.CrossingBox);
                }
                var crossingType = crossing.Type;

                //expand for crossing locations later
                mergedBounds = new BoundingBox(mergedBounds.Min - new Point(2, 2), mergedBounds.Max + new Point(2, 2));

                var mergedLocation = new MacroLocation
                {
                    Id = $"mergedCrossing_{crossingType}_{mergedBounds.Min.X}_{mergedBounds.Min.Y}_{mergedBounds.Max.X}_{mergedBounds.Max.Y}",
                    BoundingBox = mergedBounds,
                    CrossingBox = mergedCrossing,
                    Type = crossingType
                };

                mergedCrossings.Add(mergedLocation);
                processedLocations.Add(crossing);
            }
            Crossings = mergedCrossings;
        }

        public void CreateCrossingNodes()
        {
            foreach (var crossing in Crossings)
            {
                var rect = crossing.CrossingBox.ToRectangle();

                var topLeft = new Vector3Int(rect.Left, rect.Top, 0);
                var topRight = new Vector3Int(rect.Right, rect.Top, 0);
                var bottomRight = new Vector3Int(rect.Right, rect.Bottom, 0);
                var bottomLeft = new Vector3Int(rect.Left, rect.Bottom, 0);

                bool vertical = crossing.Type == LocationType.CrossingVertical;
                if (vertical)
                {
                    List<MacroNode> topNodes = new List<MacroNode>();
                    List<MacroNode> bottomNodes = new List<MacroNode>();
                    for (int x = rect.Left; x < rect.Right; x++)
                    {
                        var centerTop = new Vector3Int(x, rect.Top-1, 0);
                        topNodes.Add(new MacroNode(crossing)
                        {
                            Id = $@"pedestrian_crossing_top",
                            RealityPosition = centerTop,
                            LocationPathId = -1,
                            Type = NodeType.CrossingTop
                        });
                    }

                    for (int x = rect.Left; x < rect.Right; x++)
                    {
                        var centerBottom = new Vector3Int(x, rect.Bottom+1, 0);
                        bottomNodes.Add(new MacroNode(crossing)
                        {
                            Id = $@"pedestrian_crossing_bottom",
                            RealityPosition = centerBottom,
                            LocationPathId = -1,
                            Type = NodeType.CrossingBottom
                        });
                    }

                    for (int i = 0; i < topNodes.Count; i++)
                    {
                        var topNode = topNodes[i];
                        var bottomNode = bottomNodes[i];
                        //from bottom to top
                        var flowIdTop = game.PathfindingController.CalculateToPointStraightLine(bottomNode.RealityPosition.ToPoint(), topNode.RealityPosition.ToPoint());
                        //from top to bottom
                        var flowIdBottom = game.PathfindingController.CalculateToPointStraightLine(topNode.RealityPosition.ToPoint(), bottomNode.RealityPosition.ToPoint());
                  
                        var distance = Vector3Int.Distance(topNode.RealityPosition, bottomNode.RealityPosition);
                        topNode.NeighborConnectionPaths.Add(new MacroConnection()
                        {
                            Node = bottomNode,
                            PathId = flowIdBottom,
                            Distance = distance
                        });
                        bottomNode.NeighborConnectionPaths.Add(new MacroConnection()
                        {
                            Node = topNode,
                            PathId = flowIdTop,
                            Distance = distance
                        });

                        crossing.InternalNodes.Add(topNode);
                        crossing.InternalNodes.Add(bottomNode);
                    }
                }
                else
                {
                    List<MacroNode> leftNodes = new List<MacroNode>();
                    List<MacroNode> rightNodes = new List<MacroNode>();
                    for (int y = rect.Top; y < rect.Bottom; y++)
                    {
                        var centerLeft = new Vector3Int(rect.Left-1, y, 0);
                        // var flowIdLeft = namelessGame.PathfindingController.;
                        leftNodes.Add(new MacroNode(crossing)
                        {
                            Id = $@"pedestrian_crossing_left",
                            RealityPosition = centerLeft,
                            LocationPathId = -1,
                            Type = NodeType.CrossingLeft
                        });
                    }

                    for (int y = rect.Top; y < rect.Bottom; y++)
                    {
                        var centerRight = new Vector3Int(rect.Right+1, y, 0);
                        //var flowIdRight = namelessGame.PathfindingController.;
                        rightNodes.Add(new MacroNode(crossing)
                        {
                            Id = $@"pedestrian_crossing_right",
                            RealityPosition = centerRight,
                            LocationPathId = -1,
                            Type = NodeType.CrossingRight
                        });
                    }

                    for (int i = 0; i < leftNodes.Count; i++)
                    {
                        var leftNode = leftNodes[i];
                        var rightNode = rightNodes[i];
          
                        var flowIdLeft = game.PathfindingController.CalculateToPointStraightLine(leftNode.RealityPosition.ToPoint(), rightNode.RealityPosition.ToPoint());
                        
                        var flowIdRight = game.PathfindingController.CalculateToPointStraightLine(rightNode.RealityPosition.ToPoint(), leftNode.RealityPosition.ToPoint());

                        crossing.InternalNodes.Add(leftNode);
                        crossing.InternalNodes.Add(rightNode);
                        var distance = Vector3Int.Distance(leftNode.RealityPosition, rightNode.RealityPosition);
                        leftNode.NeighborConnectionPaths.Add(new MacroConnection()
                        {
                            Node = rightNode,
                            PathId = flowIdLeft,
                            Distance = distance
                        });
                        rightNode.NeighborConnectionPaths.Add(new MacroConnection()
                        {
                            Node = leftNode,
                            PathId = flowIdRight,
                            Distance = distance
                        });

						crossing.InternalNodes.Add(leftNode);
						crossing.InternalNodes.Add(rightNode);
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
                        var pathId = game.PathfindingController.CalculateToPointStraightLine(nodeA.RealityPosition.ToPoint(), nodeB.RealityPosition.ToPoint());
                        nodeA.NeighborConnectionPaths.Add(new MacroConnection { Node = nodeB, PathId = pathId, Distance = distance });
                    }
                }
            }
        }
    }
}
