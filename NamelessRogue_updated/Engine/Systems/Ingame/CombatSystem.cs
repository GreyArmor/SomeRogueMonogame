using System;
using System.Collections.Generic;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class CombatSystem : BaseSystem
    {
        public CombatSystem()
        {
            Signature = new HashSet<Type>();
        }
        public override HashSet<Type> Signature { get; }

        public override void Update(long gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out AttackCommand ac))
            {
                Random r = new Random();

                var source = ac.getSource();
                var stats = source.GetComponentOfType<Stats>();

                //TODO: attack damage based on stats, equipment etc.
                int damage = stats.Attack.Value;
                DamageHelper.ApplyDamage(ac.getTarget(), ac.getSource(), damage);

                Description targetDescription = ac.getTarget().GetComponentOfType<Description>();
                Description sourceDescription = ac.getSource().GetComponentOfType<Description>();
                if (targetDescription != null && sourceDescription != null)
                {
                    var logCommand = new HudLogMessageCommand();
                    namelessGame.Commander.EnqueueCommand(logCommand);

                    logCommand.LogMessage += (sourceDescription.Name + " deals " + (damage) +
                                              " damage to " + targetDescription.Name);
                    //namelessGame.WriteLineToConsole;
                }
            }
        }
    }
}
