using System;
using System.Collections.Generic;
using System.Linq;
using Veldrid;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.shell;
using NamelessRogue.Engine.Infrastructure;

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

        public override void Update(GameTime gameTime, NamelessGame game)
        {
            var playerEntity = game.PlayerEntity;
                var playerAp = playerEntity.GetComponentOfType<ActionPoints>();
            if (playerAp.Points < 100)
            {
                game.CurrentGame.Turn++;
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
