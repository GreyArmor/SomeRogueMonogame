using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Utility;

namespace NamelessRogue.Engine.Components.Rendering
{
    public class ScreenTile {
        public ScreenTile()
        {
            CharColor = new Color(0,0,0,0);
            BackGroundColor = new Color(0,0,0,0);
        }
        public string TerrainId = "Nothingness";
        public string ObjectId = "Nothingness";
        public string SpriteId = "Nothingness";
        public  Engine.Utility.Color CharColor;
        public Engine.Utility.Color BackGroundColor;
        public bool isVisible;
    }
}
