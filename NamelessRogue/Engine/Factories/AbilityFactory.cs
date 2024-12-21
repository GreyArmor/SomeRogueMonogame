using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Factories
{
    public class AbilityFactory
    {

        public static IEntity CreateDummyAbility()
        {
            Entity ability = new Entity();
            ability.AddComponent(new AbilityParameters() { ActivationMode = ActivationMode.Activatable, TargetMode = TargetMode.Targeted, AreaOfEffect = 1 }); 
            return ability;
        }
    }
}
