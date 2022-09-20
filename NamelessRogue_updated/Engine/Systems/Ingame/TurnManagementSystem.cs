using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
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
            }
        }
    }
}
