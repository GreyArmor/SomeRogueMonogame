using AStarNavigator.Providers;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Timers;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.AI.Pathfinder;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
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
        public abstract void Randomize(NamelessGame game, IWorldProvider world, Rectangle rectangle, List<Point> lockedTiles, int zLevel, InternalRandom random);
    }

    public class ApartmentShopsTerrainRandomizer : TerrainRandomizer
    {
		private enum RoomWallType { Concrete, Brick, Glass, }
        Array roomWallTypes = Enum.GetValues(typeof(RoomWallType));
        const int windowProbability = 33;
        public override void Randomize(NamelessGame game, IWorldProvider world, Rectangle rectangle, List<Point> lockedTiles, int zLevel, InternalRandom random)
		{
			var wallFurniture = TerrainFurnitureFactory.GetFurniture("wall");
			var windowFurniture = TerrainFurnitureFactory.GetFurniture("window");
			

			var weaponDisplayCases = new IEntity[] {
				TerrainFurnitureFactory.GetFurniture("displayCaseWeapons_1"),
				TerrainFurnitureFactory.GetFurniture("displayCaseWeapons_2"),
				TerrainFurnitureFactory.GetFurniture("displayCaseWeapons_2") };

			var medicalDisplayCases = new IEntity[] {
				TerrainFurnitureFactory.GetFurniture("displayCaseMedical_1"),
				TerrainFurnitureFactory.GetFurniture("displayCaseMedical_2"),
				TerrainFurnitureFactory.GetFurniture("displayCaseMedical_2") };


			const int roomOffset = 5;

			var centerX = random.Next(rectangle.Left + roomOffset, rectangle.Right - roomOffset);
			var centerY = random.Next(rectangle.Top + roomOffset, rectangle.Bottom - roomOffset);

			var rooms = RectangleUtility.SplitRectangle(rectangle, new Point(centerX, centerY));
			var cornersOfRooms = rooms.SelectMany(x => x.GetCorners());
			var doorSurroundingTiles = new List<Point>();
			GenerateRooms(world, rectangle, lockedTiles, zLevel, random, wallFurniture, windowFurniture, rooms, cornersOfRooms, doorSurroundingTiles);
			GenerateDecorationsAndVendor(game, world, zLevel, random, weaponDisplayCases, medicalDisplayCases, rooms, doorSurroundingTiles);
		}

		private static void GenerateDecorationsAndVendor(NamelessGame game, IWorldProvider world, int zLevel, InternalRandom random, IEntity[] weaponDisplayCases, IEntity[] medicalDisplayCases, Rectangle[] rooms, List<Point> doorSurroundingTiles)
		{
			var tableFurniture = TerrainFurnitureFactory.GetFurniture("table");
			var largestRoom = rooms.OrderByDescending(x => (x.Width * x.Height)).First();
			int storefrontIndex = rooms.ToList().FindIndex(x => x == largestRoom);

			for (int i = 0; i < 4; i++)
			{
				//if (storefrontIndex != i)
				//{
				//	continue;
				//}
				//50% 
				bool isMedical = random.Next(2) == 1;
				var room = rooms[i];
				var interiorRoom = RectangleUtility.GetExpanded(room, -1);
				var deeperRoom = RectangleUtility.GetExpanded(room, -2);
				var interiorOutline = RectangleUtility.GetRectangleOutline(interiorRoom);
				foreach (var point in interiorOutline)
				{
					var displayCase = isMedical ? medicalDisplayCases[random.Next(0, medicalDisplayCases.Length)] : weaponDisplayCases[random.Next(0, weaponDisplayCases.Length)];

					//var isCorner = cornersOfRooms.Contains(point);
					//var isOnEdge = IsTileOnEdge(rectangle, point.X, point.Y);
					var isReserved = doorSurroundingTiles.Contains(point);
					if (isReserved)
					{
						continue;
					}

					var tile = world.GetTile(point.X, point.Y, zLevel);
					tile.AddEntity(tableFurniture);
					tile.AddEntity((Infrastructure.Entity)displayCase);
				}

				var vendorLocation = RectangleUtility.GetRandomPointInRectangle(deeperRoom, random);
				var vendorLocationTile = world.GetTile(vendorLocation.X, vendorLocation.Y, zLevel);
				var vendorId = isMedical ? "meds_merchant" : "weapons_merchant";
				_ = CharacterFactory.CharacterDataById.TryGetValue(vendorId, out var characterData);
				var character = CharacterFactory.CreateCharacterFromData(game, new Vector3Int(vendorLocation.X, vendorLocation.Y, zLevel), characterData);
				vendorLocationTile.AddEntity(character);

			}
		}

		private void GenerateRooms(IWorldProvider world, Rectangle rectangle, List<Point> lockedTiles, int zLevel, InternalRandom random, Infrastructure.Entity wallFurniture, Infrastructure.Entity windowFurniture, Rectangle[] rooms, IEnumerable<Point> cornersOfRooms, List<Point> doorSurroundingTiles)
		{
			for (int i = 0; i < 4; i++)
			{
				var roomWallType = (RoomWallType)roomWallTypes.GetValue(random.Next(roomWallTypes.Length));
				var room = rooms[i];
				var roomWallCenters = RectangleUtility.RectangleCenters(room);

				var rectangleOutline = RectangleUtility.GetRectangleOutline(room);
				var interiorRoom = RectangleUtility.GetExpanded(room, -1);
				var interiorOutline = RectangleUtility.GetRectangleOutline(interiorRoom);

				foreach (var point in rectangleOutline)
				{
					var furniture = wallFurniture;

					var isCorner = cornersOfRooms.Contains(point);
					var isOnEdge = IsTileOnEdge(rectangle, point.X, point.Y);
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

					doorSurroundingTiles.AddRange(AllNeighborProviderFlowfield.GetNeighbors(wallCenter));
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
