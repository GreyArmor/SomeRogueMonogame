
using Microsoft.Xna.Framework;

namespace NamelessRogue.Engine.Infrastructure
{
    internal class Constants {
        //this size is in constants for now, TODO: move this size into configuration file so users could make their own custom tiles
        public static int tileAtlasTileSize = 32;
        public static int tileAtlasOutlineSize = 1;
        public static int tileAtlasSpacingSize = 1;
        public static int ChunkSize = 16;
        public static int TileSize = 64;
        public static int ChunkHeight = 32;
        //set to 100 for now, for alpha world testing;
        public static int RealityBubbleRangeInChunks = 100;
        public static int ActionsPickUpCost { get; set; } = 100;
        public static int ActionsMovementCost { get; set; } = 100;
        public static int ActionsAttackCost { get; set; } = 100;
        public static int ActionsOpenDoorCost { get; } = 100;
        public static int CitySlotDimensions { get; } = 20;
        public static int CitySquare { get; } = 300;

        public static int DebugVisionRange { get; } = 40;
        public static int DebugVisionRangePlusOne { get; } = DebugVisionRange + 1;
		public const string GameObjectRelativePath = "\\Content\\GameObjects\\";
        public static readonly int[] WorldMapZoomValues = [1, 2, 4, 8, 16, 32, 64];

    }
}
