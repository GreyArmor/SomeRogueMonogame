using SharpDX;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.AI.Pathfinder;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Group = NamelessRogue.Engine.Components.Interaction.Group;

namespace NamelessRogue.Engine.Systems.Ingame
{
	internal class FlowFieldMovementSystem : BaseSystem
	{
		public override HashSet<Type> Signature => new HashSet<Type>() {
			typeof(FlowMoveComponent), typeof(GroupTag)
		};

		bool init = true;
		double moveDelayMilisecends = 0.01;
		double milisecondsCounter = 0;
		public static FlowFieldModel flowField;
		public override void Update(GameTime gameTime, NamelessGame game)
		{
			if (init)
			{
				flowField = new FlowFieldModel(game, game.WorldProvider);
				init = false;
			}

			while (game.Commander.DequeueCommand(out FlowFieldMoveCommand mc))
			{
				var pathId = flowField.ClaculateTo(mc.To, mc.From);

				if (pathId > -1)
				{
					foreach (Entity movableEntity in mc.EntitiesToMove)
					{
						var flowMoveComponent = movableEntity.GetComponentOfType<FlowMoveComponent>();
						flowMoveComponent.To = mc.To;
						flowMoveComponent.PathId = pathId;
						flowMoveComponent.FinishedMoving = false;
					}
				}
			}
			var deltaTime = gameTime.ElapsedGameTime.TotalSeconds;

			milisecondsCounter += deltaTime;

			if (milisecondsCounter >= moveDelayMilisecends)
			{
				milisecondsCounter = -milisecondsCounter;
				foreach (Entity movableEntity in RegisteredEntities)
				{
					Position position = movableEntity.GetComponentOfType<Position>();
					var flowMoveComponent = movableEntity.GetComponentOfType<FlowMoveComponent>();
					if (!flowMoveComponent.FinishedMoving && position.Point.X > 0 && position.Point.Y > 0)
					{
						var nextPoint = flowField.GetNextPoint(flowMoveComponent.PathId, position.Point);

						if (flowMoveComponent.To == nextPoint)
						{
							flowMoveComponent.FinishedMoving = true;
							//continue;
						}

						var flagbearerTag = movableEntity.GetComponentOfType<FlagBearerTag>();
						if (flagbearerTag != null)
						{
							var groupToMove = movableEntity.GetComponentOfType<GroupTag>();
							game.Commander.EnqueueCommand(new GroupMoveCommand(groupToMove.GroupId, position.Point, nextPoint, flowMoveComponent.To));
						}
						else
						{
							game.WorldProvider.MoveEntity(movableEntity,
							  new Point(nextPoint.X, nextPoint.Y));
						}
					}
				}
			}

		}
	}
	public class FlowFieldMoveCommand : ICommand
	{
		public Point From;
		public Point To;
		public FlowFieldMoveCommand(Point from, Point to, params IEntity[] entitiesToMove)
		{
			//Debug.WriteLine("FlowFieldMoveCommand");
			From = from;
			To = to;
			EntitiesToMove = entitiesToMove.ToList();
		}

		public List<IEntity> EntitiesToMove { get; }
	}

}
