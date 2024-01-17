using Veldrid;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.AI.Pathfinder;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.WorldBoardComponents.Combat;
using NamelessRogue.Engine.Systems;
using NamelessRogue.shell;
using Newtonsoft.Json.Linq;
using Veldrid;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class GroupMoveSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame game)
        {
            while (game.Commander.DequeueCommand(out GroupMoveCommand command))
            {
                var groups = game.PlayerEntity.GetComponentOfType<GroupsHolder>().Groups.Select(x => x.GetComponentOfType<Group>());
				var group = groups.FirstOrDefault(x=>x.TextId==command.GroupTag);
                if (group != null)
                {
                    var flagbearer = group.FlagbearerId;

					var diffPoint = command.NextPoint.Substract(command.PreviousPoint);

                    var units = group.EntitiesInGroup.ToList();
                    units.Remove(flagbearer);

					var nextFlagbearerPoint = command.NextPoint;

					game.WorldProvider.ClearTheWay(flagbearer, command.PreviousPoint);
				

					Queue<IEntity> immovableUnits = new Queue<IEntity>();

                    foreach (var unit in units)
                    {
						var groupTag = unit.GetComponentOfType<GroupTag>();
						var position = unit.GetComponentOfType<Position>();
                        var nextPoint = nextFlagbearerPoint.Sum(groupTag.FormationPositionDisplacement);
                        game.WorldProvider.ClearTheWay(unit, position.Point);
                    }

					game.WorldProvider.AddEntityToNewLocation(flagbearer, nextFlagbearerPoint);

					foreach (var unit in units)
                    {
						var groupTag = unit.GetComponentOfType<GroupTag>();
                        if (groupTag.IsInFormation)
                        {
                            var position = unit.GetComponentOfType<Position>();
							var nextPoint = nextFlagbearerPoint.Sum(groupTag.FormationPositionDisplacement);
							var canMove = game.WorldProvider.MoveEntity(unit,
							 nextPoint);

                            if (!canMove)
                            {
								//var flowMoveComponent = unit.GetComponentOfType<FlowMoveComponent>();
							//	nextPoint = FlowFieldMovementSystem.flowField.GetNextPoint(flowMoveComponent.PathId, position.Point);

								game.WorldProvider.MoveEntity(unit,
							 new Point(nextPoint.X, nextPoint.Y));

								IEnumerable<Point> nextPointNeighbors = null;

								//diagonal movement
								if (diffPoint.X != 0 && diffPoint.Y != 0)
								{
									nextPointNeighbors = DiagonalNeighborProviderFlowfield.GetNeighbors(nextPoint);
								}
								else
								{
									nextPointNeighbors = SharpCornerNeighborProviderFlowfield.GetNeighbors(nextPoint);
								}

								var orderedByDistance = nextPointNeighbors.OrderBy(p => DistanceBetweenPoints(p, position.Point)).ToList();

								canMove = game.WorldProvider.MoveEntity(unit, new Point(orderedByDistance[0].X, orderedByDistance[0].Y));
								if (!canMove)
								{
									canMove = game.WorldProvider.MoveEntity(unit, new Point(orderedByDistance[1].X, orderedByDistance[1].Y));

								}
							}
                        }
                    }
                }
            }
        }
		float DistanceBetweenPoints(Point value1, Point value2)
        {
			int num = value1.X - value2.X;
			int num2 = value1.Y - value2.Y;
			return MathF.Sqrt(num * num + num2 * num2);
		}

    }
}
