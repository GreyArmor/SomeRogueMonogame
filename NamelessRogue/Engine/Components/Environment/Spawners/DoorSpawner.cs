using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Environment.Spawners
{
    internal class DoorSpawner : Spawner
    {
        public override Entity Spawn()
        {
            Entity door = new Entity();
            door.AddComponent(new Drawable("Door", new Engine.Utility.Color(0.7, 0.7, 0.7), new Engine.Utility.Color()));
            door.AddComponent(new Description("Door", ""));
            door.AddComponent(new Door());
            door.AddComponent(new SimpleSwitch(true));
            door.AddComponent(new OccupiesTile());
            door.AddComponent(new BlocksVision());
            return door;
        }
    }
}
