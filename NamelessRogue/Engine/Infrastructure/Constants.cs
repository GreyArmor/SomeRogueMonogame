
using Veldrid;
using System.Numerics;

namespace NamelessRogue.Engine.Infrastructure
{
    internal class Constants {
        //this size is in constants for now, TODO: move this size into configuration file so users could make their own custom tiles
        public static int tileAtlasTileSize = 64;
        public static int ChunkSize = 32;
        public static int RealityBubbleRangeInChunks = 40;
        public static int ActionsPickUpCost { get; set; } = 100;
        public static int ActionsMovementCost { get; set; } = 100;
        public static int ActionsAttackCost { get; set; } = 100;
        public static int ActionsOpenDoorCost { get; } = 100;
        public static int CitySlotDimensions { get; } = 20;
        public static int CitySquare { get; } = 300;

		public static readonly float ScaleDownCoeficient = 0.001f;
		public static Matrix4x4 ScaleDownMatrix = Matrix4x4.CreateScale(ScaleDownCoeficient);
       

	}
}
