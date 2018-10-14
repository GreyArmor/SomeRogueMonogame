using NamelessRogue.Engine.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.Engine.Engine.Utility;

namespace NamelessRogue.Engine.Engine.Factories
{
    public  class ItemFactory {

        public static Entity CreateItem()
        {
            Entity item = new Entity();
            item.AddComponent(new Item(item.GetId()));
            item.AddComponent(new Drawable('I',new Color(1f,0,0)));
            //for debug;
            item.AddComponent(new Position(109* Constants.ChunkSize + 3,307*Constants.ChunkSize + 3));
            return item;

        }

    }
}
