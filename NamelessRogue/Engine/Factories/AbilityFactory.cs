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

namespace NamelessRogue.Engine.Factories
{
    public class AbilityFactory
    {

        public static IEntity CreateJumpAbility()
        {
            Entity ability = new Entity();
            ability.AddComponent(new Description() { Name = "Jump", Text = "Jump to target location" });
            ability.AddComponent(new Drawable() { ObjectID = "jump" });
            ability.AddComponent(new AbilityComponent());
            ability.AddComponent(new AbilityParameters() { ActivationMode = ActivationMode.Activatable, TargetMode = TargetMode.Targeted, AreaOfEffect = 1,
                AbilityActions = new List<AbilityAction>() { AbilityAction.JumpToTarget }}); 
            return ability;
        }

        public static IEntity CreateLeapSlashAbility()
        {
            Entity ability = new Entity();
            ability.AddComponent(new Description() { Name = "Leap slash", Text = "Jump to target location and slash the enemy with your weapon" });
            ability.AddComponent(new Drawable() { ObjectID = "jump_slash" });
            ability.AddComponent(new AbilityComponent());
            ability.AddComponent(new AbilityParameters()
            {
                ActivationMode = ActivationMode.Activatable,
                TargetMode = TargetMode.TargetEnemies,
                AreaOfEffect = 1,
                AbilityActions = new List<AbilityAction>() { AbilityAction.JumpBesidesTarget, AbilityAction.AttackTargetMelee }
            });
            return ability;
        }
    }
}
