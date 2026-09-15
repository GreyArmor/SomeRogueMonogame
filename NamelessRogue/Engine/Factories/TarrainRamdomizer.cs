using Microsoft.Xna.Framework;
using MonoGame.Extended;
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
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace NamelessRogue.Engine.Factories
{
    public abstract class TerrainRandomizer
    {
        public abstract void Randomize(IWorldProvider world, Rectangle rectangle, List<Point> lockedTiles, int zLevel, InternalRandom random);
    }

    public class ApartmentShopsTerrainRandomizer : TerrainRandomizer
    {        
        private enum RoomWallType { Concrete, Brick, Glass, }
        Array roomWallTypes = Enum.GetValues(typeof(RoomWallType));
        const int windowProbability = 33;
        public override void Randomize(IWorldProvider world, Rectangle rectangle, List<Point> lockedTiles, int zLevel, InternalRandom random)
        {
            var wallFurniture = TerrainFurnitureFactory.GetFurniture("wall");
			var windowFurniture = TerrainFurnitureFactory.GetFurniture("window");

			const int roomOffset = 5;

            var centerX = random.Next(rectangle.Left + roomOffset, rectangle.Right - roomOffset);
            var centerY = random.Next(rectangle.Top + roomOffset, rectangle.Bottom - roomOffset);                      

            var rooms = RectangleUtility.SplitRectangle(rectangle, new Point(centerX, centerY));
            var cornersOfRooms = rooms.SelectMany(x => x.GetCorners());
		
			for (int i = 0; i < 4; i++)
            {
                var roomWallType = (RoomWallType)roomWallTypes.GetValue(random.Next(roomWallTypes.Length));
                var room = rooms[i];
                var roomWallCenters = RectangleUtility.RectangleCenters(room);

                var rectangleOutline = RectangleUtility.GetRectangleOutline(room);

                foreach(var point in rectangleOutline)
                {
                    var furniture = wallFurniture;

                    var isCorner = cornersOfRooms.Contains(point);
                    var isOnEdge = IsTileOnEdge(rectangle, point.X,point.Y);
                    var isReserved = lockedTiles.Contains(point);
					if (!isReserved && !isCorner && isOnEdge)
                    {
                        furniture = random.Next(100) <= windowProbability ? windowFurniture : wallFurniture;
                    }

					var tile = world.GetTile(point.X, point.Y, zLevel);
                    tile.ClearEntities();
                    
                    tile.AddEntity(furniture);
                }

                foreach (var wallCenter in roomWallCenters)
                {
					var isReserved = lockedTiles.Contains(wallCenter);
					if (cornersOfRooms.Contains(wallCenter) || isReserved)
					{
						continue;
					}
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

		public bool IsTileOnEdge(Rectangle rect, int tileX, int tileY)
		{
			if (tileX < rect.Left || tileX >= rect.Right || tileY < rect.Top || tileY >= rect.Bottom)
			{
				return false;
			}
			return tileX == rect.Left ||
				   tileX == rect.Right - 1 ||
				   tileY == rect.Top ||
				   tileY == rect.Bottom - 1;
		}

	}
}
