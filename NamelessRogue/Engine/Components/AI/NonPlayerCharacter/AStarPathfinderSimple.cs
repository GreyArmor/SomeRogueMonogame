using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AStarNavigator;
using AStarNavigator.Algorithms;
using AStarNavigator.Providers;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.shell;
using Tile = NamelessRogue.Engine.Components.ChunksAndTiles.Tile;

namespace NamelessRogue.Engine.Components.AI.NonPlayerCharacter
{
    public class AStarPathfinderSimple
    {
        // public const int gridSize = 100;
        // public static Grid grid = new Grid(gridSize, gridSize);
        //public List<Point> FindPath(Point start, Point destination, IWorldProvider world, NamelessGame game)
        //{

        //    var gridOffset = new Point(start.X- (gridSize/2), start.Y- (gridSize/2));

        //    var gridStart = WorldToGrid(gridOffset, start);
        //    var gridEnd = WorldToGrid(gridOffset, destination);

        //    if (gridEnd.X < 0 || gridEnd.Y < 0 || gridEnd.X >= gridSize || gridEnd.Y >= gridSize)
        //    {
        //        return new List<Point>();
        //    }

        //    for (int x = 0; x < 100; x++)
        //    {
        //        for (int y = 0; y < 100; y++)
        //        {
        //            var point = GridToWorld(gridOffset, new Point(x, y));
        //            var tile = world.GetTile(point.X, point.Y);
        //            grid.UnblockCell(new Position(x, y));
        //            if (!tile.IsPassable())
        //            {
        //                grid.BlockCell(new Position(x, y));
        //            }
        //        }
        //    }


        //    //unblock start and end
        //    grid.UnblockCell(new Position(gridStart.X, gridStart.Y));
        //    grid.UnblockCell(new Position(gridEnd.X, gridEnd.Y));

        //    var path = grid.GetPath(new Position(gridStart.X, gridStart.Y), new Position(gridEnd.X, gridEnd.Y));

        //    List<Point> resultPoints = new List<Point>();
        //    foreach (var position in path)
        //    {

        //        var point = new Point(position.X, position.Y);
        //        resultPoints.Add(GridToWorld(gridOffset,point));
        //    }

        //    return resultPoints;

        //}


        //public List<Point> FindPath(Point start, Point destination, IWorldProvider world, NamelessGame game)
        //{

        //    Func<Tile, IEnumerable<Tile>> getNeighbors = delegate(Tile p)
        //    {
        //        return new[]
        //        {
        //            world.GetTile(p.GetCoordinate().X - 1, p.GetCoordinate().Y + 0), // L
        //            world.GetTile(p.GetCoordinate().X + 1, p.GetCoordinate().Y + 0), // R
        //            world.GetTile(p.GetCoordinate().X, p.GetCoordinate().Y + 1), // B
        //            world.GetTile(p.GetCoordinate().X, p.GetCoordinate().Y - 0), // T
        //        };
        //    };

        //    Func<Tile, Tile, double> getScoreBetween = (p1, p2) =>
        //    {
        //        // Manhatten Distance
        //        return Math.Abs(p1.GetCoordinate().X - p2.GetCoordinate().X) + Math.Abs(p1.GetCoordinate().Y - p2.GetCoordinate().Y);
        //    };

        //    var rand = new Random(42);

        //    Func<Tile, double> getHeuristicScore = p =>
        //    {
        //        if (p.IsPassable())
        //        {
        //            return Math.Abs(p.GetCoordinate().X - destination.X) + Math.Abs(p.GetCoordinate().Y - destination.Y);
        //        }
        //        else if( p.GetCoordinate().X<0||p.GetCoordinate().Y<0)
        //        {
        //            return double.MaxValue;
        //        }
        //        return double.MaxValue;
        //    };

        //    Console.WriteLine("Going from {0} to {1}", start, destination);

        //    var sw = Stopwatch.StartNew();
        //    double distance = 0;
        //    var results = AStarUtilities.FindMinimalPath(world.GetTile(start.X,start.Y), world.GetTile(destination.X,destination.Y), getNeighbors,
        //        getScoreBetween, getHeuristicScore).Result;
        //    sw.Stop();
        //    return results.Select(x=>x.GetCoordinate()).ToList();
        //}



        public List<Point> FindPath(Point start, Point destination, IWorldProvider world, NamelessGame game)
        {
       


            var from = new AStarNavigator.Tile(start.X, start.Y);

            var trimmedDestination = new Point(destination.X, destination.Y);


            //var differenceInX = Math.Abs(destination.X - start.X);

            //var areaSize = 5;

            //if (differenceInX > areaSize)
            //{
            //    trimmedDestination.X = destination.X > start.X
            //        ? destination.X - (differenceInX - areaSize)
            //        : destination.X + (differenceInX - areaSize);
            //}

            //var differenceInY = Math.Abs(destination.Y - start.Y);

            //if (differenceInY > areaSize)
            //{
            //    trimmedDestination.Y = destination.Y > start.Y
            //        ? destination.Y - (differenceInY - areaSize)
            //        : destination.Y + (differenceInY - areaSize);
            //}


            var navigator = new TileNavigator(
                new BlockedProvider(world, trimmedDestination, start),         // Instance of: IBockedProvider
                new DiagonalNeighborProvider(),     // Instance of: INeighborProvider
                new PythagorasAlgorithm(),          // Instance of: IDistanceAlgorithm
                new ManhattanHeuristicAlgorithm()   // Instance of: IDistanceAlgorithm
            );

            //if (destination.X - start.X <= -5)
            //{
            //    trimmedDestination.X = destination.X + Math.Abs(destination.X - start.X + 5);
            //}
            //else if (destination.X - start.X >= 5)
            //{
            //    trimmedDestination.X = destination.X - Math.Abs(destination.X - start.X + 5); ;
            //}


            //if (destination.Y - start.Y <= -5)
            //{
            //    trimmedDestination.Y = destination.Y + Math.Abs(destination.Y - start.Y + 5);
            //}
            //else if (destination.Y - start.Y >= 5)
            //{
            //    trimmedDestination.Y = destination.Y - Math.Abs(destination.Y - start.Y + 5);
            //}

            var to = new AStarNavigator.Tile(trimmedDestination.X, trimmedDestination.Y);

            var result = navigator.Navigate(from, to);

            if (result == null)
            {
                return new List<Point>();
            }

            return result.Select(x => new Point((int)x.X, (int)x.Y)).ToList();

        }

        public class BlockedProvider : IBlockedProvider
        {
            private readonly IWorldProvider _worlldProvider;
            private readonly Point _destination;
            private readonly Point _start;
            private int counter = 0;
            private int maxSearches = 200;
            public BlockedProvider(IWorldProvider worlldProvider, Point destination, Point start)
            {
                _worlldProvider = worlldProvider;
                _destination = destination;
                _start = start;
            }

            public bool IsBlocked(AStarNavigator.Tile coord)
            {
                counter++;
                //  return false;
                var tile = _worlldProvider.GetTile((int)coord.X, (int)coord.Y, 0);
                if (counter>= maxSearches)
                {
                    return true;
                }

                if (coord.X == _destination.X && coord.Y == _destination.Y)
                {
                    return false;
                }

                if (coord.X == _start.X && coord.Y == _start.Y)
                {
                    return false;
                }


                var isBlocked = !tile.IsPassable();
                return isBlocked;


            }
        }

        public Point GridToWorld(Point position, Point world)
        {
            Point result = new Point();

            int cameraX = position.X;
            int cameraY = position.Y;
            int worldX = world.X;
            int worldY = world.Y;
            int screenX = worldX + cameraX;
            int screenY = worldY + cameraY;

            result.X = (screenX);
            result.Y = (screenY);
            return result;
        }

        public Point WorldToGrid(Point position, Point world)
        {
            Point result = new Point();

            int cameraX = position.X;
            int cameraY = position.Y;
            int worldX = world.X;
            int worldY = world.Y;
            int screenX = worldX - cameraX;
            int screenY = worldY - cameraY;

            result.X = (screenX);
            result.Y = (screenY);
            return result;
        }

    }
}
