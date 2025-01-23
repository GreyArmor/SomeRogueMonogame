using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class TurnManagementSystem : BaseSystem
    {
        public TurnManagementSystem()
        {
            Signature = new HashSet<Type>();
            Signature.Add(typeof(ActionPoints));
        }

        public override HashSet<Type> Signature { get; }

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            namelessGame.TurnUpdated = false;
            var playerEntity = namelessGame.PlayerEntity;
            var playerAp = playerEntity.GetComponentOfType<ActionPoints>();
            if (playerAp.Points < 100)
            {
                namelessGame.CurrentGame.Turn++;
                foreach (var entity in RegisteredEntities)
                {
                    var ap = entity.GetComponentOfType<ActionPoints>();
                    if (ap != null)
                    {
                        ap.Points += 100;
                    }
                }

                //reduce cooldowns of all abilities by one when the turn passes
                foreach(var ability in EntityInfrastructureManager.Components[typeof(AbilityParameters)])
                {
                    var abilitiParams = (AbilityParameters)ability.Value;
                    if (abilitiParams.CooldownTurnsRemaining > 0)
                    {
                        abilitiParams.CooldownTurnsRemaining--;
                    }
                }

                namelessGame.TurnUpdated = true;
            }
        }
    }
}
