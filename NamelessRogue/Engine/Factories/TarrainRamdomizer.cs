using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Timers;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Utility;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledCSPlus;

namespace NamelessRogue.Engine.Factories
{
    public abstract class TerrainRandomizer
    {
        public abstract void Randomize(IWorldProvider world, Rectangle rectangle, int zLevel, InternalRandom random);
    }

    public class ApartmentShopsTerrainRandomizer : TerrainRandomizer
    {        
        public override void Randomize(IWorldProvider world, Rectangle rectangle, int zLevel, InternalRandom random)
        {
            var wallFurniture = TerrainFurnitureFactory.GetFurniture("wall");
 
            var roomsNumber = random.Next(1, 5);

            const int roomOffset = 5;

            var centerX = random.Next(rectangle.Left + roomOffset, rectangle.Right - roomOffset);
            var centerY = random.Next(rectangle.Top + roomOffset, rectangle.Bottom - roomOffset);                      

            var rooms = RectangleUtility.SplitRectangle(rectangle, new Point(centerX, centerY));

            for (int i = 0; i < 4; i++)
            {
                var room = rooms[i];
                var roomWallCenters = RectangleUtility.RectangleCenters(room);

                var rectangleOutline = RectangleUtility.GetRectangleOutline(room);

                foreach(var point in rectangleOutline)
                {
                    var tile = world.GetTile(point.X, point.Y, zLevel);
                    tile.ClearEntities();
                    tile.AddEntity(wallFurniture);
                }

                foreach (var wallCenter in roomWallCenters)
                {
                    var tile = world.GetTile(wallCenter.X, wallCenter.Y, zLevel);
                    tile.ClearEntities();

                    var doorFurniture = BuildingFactory.CreateDoor(wallCenter.X, wallCenter.Y, zLevel, "door");
                    tile.AddEntity(doorFurniture);
                }
            }

            BuildingDataCache buildingDataCache = new BuildingDataCache();
            buildingDataCache.CalculateCacheForRandomizer(rectangle.Width, rectangle.X, rectangle.Y, zLevel, world);
            for (int loopY = 0; loopY < rectangle.Width; loopY++)
            {
                for (int loopX = 0; loopX < rectangle.Width; loopX++)
                {
                    var tile = world.GetTile(loopX + rectangle.X, loopY + rectangle.Y, zLevel);
                    tile.TilesetPosition = buildingDataCache.tilesetPositions[loopY, loopX];
                }
            }

        }
    }
}
