 
namespace NamelessRogue.Engine.Engine.Infrastructure
{
    class Constants {
        //this size is in constants for now, TODO: move this size into configuration file so users could make their own custom tiles
        public static int tileAtlasTileSize = 64;
        public static int ChunkSize = 64;
        public static int RealityBubbleRangeInChunks = 1;
        public static int ActionsPickUpCost { get; set; } = 100;
        public static int ActionsMovementCost { get; set; } = 100;
        public static int ActionsAttackCost { get; set; } = 100;
        public static int ActionsOpenDoorCost { get; set; } = 100;
    }
}
