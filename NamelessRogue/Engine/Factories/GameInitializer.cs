using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems;

namespace NamelessRogue.Engine.Factories
{
    public class GameInitializer {

        public static IEntity CreateCursor()
        {
            Entity cursor = new Entity();
            cursor.AddComponent(new Cursor());
            cursor.AddComponent(new Position(0,0));
            Drawable dr = new Drawable("Cursor", new Engine.Utility.Color(0.9,0.9,0.9));
            dr.Visible = false;
            cursor.AddComponent(dr);
            cursor.AddComponent(new InputComponent());
            cursor.AddComponent(new LineToPlayer());
            return cursor;
        }
    }
}
