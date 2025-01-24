using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Status;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class FireSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>() { typeof(Fire) }; 

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            if (namelessGame.TurnUpdated)
            {
                var removalList = new List<IEntity>();
                foreach (var fireEntity in RegisteredEntities)
                {
                    var fireComponent = fireEntity.GetComponentOfType<Fire>();
                    fireComponent.Duration--;

                    var firePosition = fireEntity.GetComponentOfType<Position>();
                    var tile = namelessGame.WorldProvider.GetTile(firePosition.X, firePosition.Y, firePosition.Z);

                    var tileEntities = tile.GetEntities();
                    foreach (var tileEntity in tileEntities)
                    {
                        var characterComponent = tileEntity.GetComponentOfType<Character>();
                        if(characterComponent != null)
                        {
                            var damageCommand = new DealDamageCommand(new Damage(fireEntity, tileEntity, 5, Components.Stats.DamageType.Flame));
                            namelessGame.Commander.EnqueueCommand(damageCommand);
                        }
                    }

                    if(fireComponent.Duration <= 0)
                    {
                        removalList.Add(fireEntity);
                    }
                }

                foreach (var fireEntity in removalList)
                {
                    namelessGame.RemoveEntity(fireEntity);
                }
            }
        }
    }
}
