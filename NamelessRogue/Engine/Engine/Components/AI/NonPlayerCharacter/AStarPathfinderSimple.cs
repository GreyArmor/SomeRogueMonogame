using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;

namespace NamelessRogue.Engine.Engine.Components.AI.NonPlayerCharacter
{
    public class AStarPathfinderSimple {

        public class SearchNode
        {
            public SearchNode Parent;
            public Point NodePosition;
            public int DinstanceToStartingNode;
            public int DinstanceToDestinationNode;

            public SearchNode(SearchNode parent, Point nodePosition, Point destination) {
                Parent = parent;
                NodePosition = nodePosition;
                if(parent==null) {
                    DinstanceToStartingNode = 0;
                }
                else
                {
                    DinstanceToStartingNode = parent.DinstanceToStartingNode+1;
                }

                DinstanceToDestinationNode = CalculateManhattanDistance(nodePosition, destination);

            }

            private int CalculateManhattanDistance(Point a, Point b)
            {
                return Math.Abs(a.Y - b.Y) + Math.Abs(a.X - b.X);
            }

        }



        private SearchNode FindClosest(){
            SearchNode closest = openList.First();
            int distanceToClosest = closest.DinstanceToDestinationNode + closest.DinstanceToStartingNode;
            foreach (SearchNode node in openList){
                int distanceToCurrent = node.DinstanceToDestinationNode + node.DinstanceToStartingNode;
                if(distanceToClosest > distanceToCurrent)
                {
                    closest = node;
                    distanceToClosest = distanceToCurrent;
                }
            }
            return closest;
        }

        private void AddNeighborsToOpenList(SearchNode node, IChunkProvider world, Point destination)
        {
            for (int x = node.NodePosition.Y - 1; x<=node.NodePosition.Y+1; x++)
            {
                for (int y = node.NodePosition.X - 1; y<=node.NodePosition.X+1; y++) {
                    Point currentposition = new Point(x,y);
                    bool isInClosed = closedList.Any(n => n.NodePosition.Equals(currentposition));
                    bool isInOpen = openList.Any(n => n.NodePosition.Equals(currentposition));
                    if (!isInClosed && !isInOpen) {
                        Tile t = world.getTile(x,y);
                        if(t.getPassable()) {
                            openList.Add(new SearchNode(node, currentposition, destination));
                        }
                        else
                        {
                            closedList.Add(new SearchNode(node, currentposition, destination));
                        }
                    }
                }
            }
        }

        List<SearchNode> openList;
        List<SearchNode> closedList;
        bool seachNearPosiiton;
        public List<Point> FindPath(Point start, Point destination, IChunkProvider world, bool positionNear){
            seachNearPosiiton = positionNear;
            openList = new List<SearchNode>();
            closedList = new List<SearchNode>();

            openList.Add(new SearchNode(null, start,destination));

            while (openList.Count>0)
            {
                SearchNode closestNode = FindClosest();
                openList.Remove(closestNode);
                closedList.Add(closestNode);
                if(closestNode.NodePosition.Equals(destination))
                {
                    return ConstructPath(closestNode);
                }

                if(seachNearPosiiton && closestNode.DinstanceToDestinationNode==1)
                {
                    return ConstructPath(closestNode);
                }
                AddNeighborsToOpenList(closestNode, world, destination);
                if (closedList.Count>500)
                {
                    return null;
                }
            }
            return null;

        }

        //excluding the start point
        private List<Point> ConstructPath(SearchNode closestNode) {
            List<Point> path = new List<Point>();
            SearchNode node = closestNode;
            while (node.Parent!=null){
                path.Add(node.NodePosition);
                node = node.Parent;
            }
            path.Reverse();
            return path;
        }
    }
}
