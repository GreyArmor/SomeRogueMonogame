using System.Collections.Generic;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Generation.Items;

namespace NamelessRogue.Engine.Engine.Generation.World.Tech
{
    public static class TechnologyLibrary
    {
        public static List<TechnologyAge> TechnologyAges { get; set; } = new List<TechnologyAge>();

        static TechnologyLibrary()
        {

            var ancientAgeOpenedItems = new List<ItemBlueprint>();

            //var ancientAge = new TechnologyAge()
            //{
            //    Name = "Ancient age",
            //    OpenedItems = 

            //};

            var medievalAge = new TechnologyAge();
        }

    }
}
