using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class ConsumeCommand : ICommand
    {
        public ConsumeCommand(Entity entity)
        {
            ItemEntity = entity;
        }

        public Entity ItemEntity { get; }
    }
}
