using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.shell;
using RoyT.AStar;

namespace NamelessRogue.Engine.Engine.Components.AI.NonPlayerCharacter
{
    public class AStarPathfinderSimple {

        public static Grid grid = new Grid(100, 100);
        public List<Point> FindPath(Point start, Point destination, IChunkProvider world, NamelessGame game)
        {

            var gridOffset = new Point(start.X-50,start.Y-50);

            var gridStart = WorldToGrid(gridOffset, start);
            var gridEnd = WorldToGrid(gridOffset, destination);

            if (gridEnd.X < 0 || gridEnd.Y < 0)
            {
                return new List<Point>();
            }

            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    var point = GridToWorld(gridOffset, new Point(x, y));
                    var tile = world.GetTile(point.X, point.Y);
                    grid.UnblockCell(new Position(x, y));
                    if (!tile.GetPassable(game))
                    {
                        grid.BlockCell(new Position(x, y));
                    }
                }
            }
           
            
            //unblock start and end
            grid.UnblockCell(new Position(gridStart.X, gridStart.Y));
            grid.UnblockCell(new Position(gridEnd.X, gridEnd.Y));

           var path = grid.GetPath(new Position(gridStart.X, gridStart.Y), new Position(gridEnd.X, gridEnd.Y));

            List<Point> resultPoints = new List<Point>();
            foreach (var position in path)
            {

                var point = new Point(position.X, position.Y);
                resultPoints.Add(GridToWorld(gridOffset,point));
            }

            return resultPoints;

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
