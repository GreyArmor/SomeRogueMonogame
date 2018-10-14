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
                        //namelessGame.WriteLineToConsole(sourceDescription.Name + " deals " + String.valueOf(damage) +
                        //                        " damage to " + targetDescription.Name);
                    }

                    entity.RemoveComponentOfType<AttackCommand>();
                }
            }
        }
    }
}
