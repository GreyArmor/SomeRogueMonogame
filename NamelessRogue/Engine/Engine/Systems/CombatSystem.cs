using System;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.UI;
using NamelessRogue.Engine.Engine.Utility;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems
{
    public class CombatSystem : ISystem
    {

        public void Update(long gameTime, NamelessGame namelessGame)
        {
            foreach (IEntity entity in namelessGame.GetEntities()){

                AttackCommand ac = entity.GetComponentOfType<AttackCommand>();
                if (ac != null)
                {
                    Random r = new Random();
                    //TODO: attack damage based on stats, equipment etc.
                    int damage = r.Next(5) + 5;
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
}
