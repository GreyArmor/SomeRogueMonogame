using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class AbilitySystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out ActivateAbilityCommand command))
            {
                var abilityParameters = command.Ability.GetComponentOfType<AbilityParameters>();

                if(abilityParameters.TargetMode == TargetMode.Targeted)
                {
                    var cursorEntity = namelessGame.CursorEntity;
                    var cursorPos = cursorEntity.GetComponentOfType<Position>();

               //     abilityParameters.
                }
            }
        }
    }
}
