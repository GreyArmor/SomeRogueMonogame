using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class FireWeaponSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out FireWeaponCommand command))
            {
                var playerEntity = namelessGame.PlayerEntity;
                var playerPosition = playerEntity.GetComponentOfType<Position>();
                var cursorPosition = namelessGame.CursorEntity.GetComponentOfType<Position>();
                var tile = namelessGame.WorldProvider.GetTile(cursorPosition.X, cursorPosition.Y, cursorPosition.Z);
                if (tile.AnyEntities())
                { 
                    var character = tile.GetEntities().FirstOrDefault(x=>x.GetComponentOfType<Character>()!=null);
                    if (character != null)
                    {
                        var combatCommand = new AttackCommand(playerEntity, character);
                        namelessGame.Commander.EnqueueCommand(combatCommand);
                    }
                }
                var createProjectileCommand = new CreateProjectileCommand(playerPosition.Point, cursorPosition.Point, DamageType.Ballistic);
                namelessGame.Commander.EnqueueCommand(createProjectileCommand);
            }
        }
    }
}
