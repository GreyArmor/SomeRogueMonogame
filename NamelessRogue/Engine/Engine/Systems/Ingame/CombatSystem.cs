using System;
using System.Collections.Generic;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.Stats;
using NamelessRogue.Engine.Engine.Components.UI;
using NamelessRogue.Engine.Engine.Utility;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems.Ingame
{
    public class CombatSystem : BaseSystem
    {
        public CombatSystem()
        {
            Signature = new HashSet<Type>();
            Signature.Add(typeof(AttackCommand));
        }
        public override HashSet<Type> Signature { get; }

        public override void Update(long gameTime, NamelessGame namelessGame)
        {
            foreach (IEntity entity in RegisteredEntities)
            {

                AttackCommand ac = entity.GetComponentOfType<AttackCommand>();
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
                    var logCommand = entity.GetComponentOfType<HudLogMessageCommand>();
                    if (logCommand == null)
                    {
                        logCommand = new HudLogMessageCommand();
                        entity.AddComponent(logCommand);
                    }

                    logCommand.LogMessage += (sourceDescription.Name + " deals " + (damage) +
                                              " damage to " + targetDescription.Name);
                    //namelessGame.WriteLineToConsole;
                }

                entity.RemoveComponentOfType<AttackCommand>();
            }
        }
    }
}
