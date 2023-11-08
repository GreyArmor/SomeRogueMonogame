using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Systems;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class GroupMoveSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out GroupMoveCommand command))
            {
                var groups = namelessGame.GetEntitiesByComponentClass<Group>().Select(gr=>gr.GetComponentOfType<Group>());
                var group = groups.FirstOrDefault(x=>x.TextId==command.GroupTag);
                if (group != null)
                {

                    var diffPoint = command.NextPoint - command.PreviousPoint;

                    foreach (var unitId in group.EntitiesInGroup)
                    {
                        var unit = namelessGame.GetEntity(unitId);

                        var position = unit.GetComponentOfType<Position>();

                        var nextPoint = position.Point + diffPoint;

						namelessGame.WorldProvider.MoveEntity(unit,
						  new Point(nextPoint.X, nextPoint.Y));

					}
				}
            }
        }
    }
}
