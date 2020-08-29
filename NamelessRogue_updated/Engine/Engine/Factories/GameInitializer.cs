using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Components.UI;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.Engine.Engine.Systems;

namespace NamelessRogue.Engine.Engine.Factories
{
    public class GameInitializer {

        public static IEntity CreateCursor()
        {
            Entity cursor = new Entity();
            cursor.AddComponent(new Cursor());
            cursor.AddComponent(new Position(0,0));
            Drawable dr = new Drawable('X', new Engine.Utility.Color(0.9,0.9,0.9));
            dr.setVisible(false);
            cursor.AddComponent(dr);
            cursor.AddComponent(new InputComponent());
            cursor.AddComponent(new LineToPlayer());
            return cursor;
        }
    }
}
