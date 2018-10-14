using NamelessRogue.Engine.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Engine.Utility;


/**
 * Created by Admin on 16.06.2017.
 */
namespace NamelessRogue.Engine.Engine.Components.Rendering
{
    public class ScreenTile : Component {
        public ScreenTile()
        {
            Char=' ';
            CharColor = new Color(0,0,0,0);
            BackGroundColor = new Color(0,0,0,0);
        }
        public char Char;
        public  Engine.Utility.Color CharColor;
        public Engine.Utility.Color BackGroundColor;
        public bool isVisible;
    }
}
