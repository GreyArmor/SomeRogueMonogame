﻿using Microsoft.Xna.Framework;
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
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Ingame
{
	internal class FlowFieldMovementSystem : BaseSystem
	{
		public override HashSet<Type> Signature => new HashSet<Type>() {
			typeof(FlowMoveComponent)
		};

		bool init = true;
		double moveDelayMilisecends = 0.01;
		double milisecondsCounter = 0;
		FlowFieldModel flowField;
		public override void Update(GameTime gameTime, NamelessGame namelessGame)
		{
			if (init)
			{
				flowField = new FlowFieldModel(namelessGame, namelessGame.WorldProvider);
				init = false;
			}

			while (namelessGame.Commander.DequeueCommand(out FlowFieldMoveCommand mc))
			{
				var pathId = flowField.ClaculateTo(mc.To, mc.From);
				foreach (Entity movableEntity in RegisteredEntities)
				{
					var flowMoveComponent = movableEntity.GetComponentOfType<FlowMoveComponent>();
					flowMoveComponent.To = mc.To;
					flowMoveComponent.PathId = pathId;
					flowMoveComponent.FinishedMoving = false;
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
							continue;
						}

						namelessGame.WorldProvider.MoveEntity(movableEntity,
						  new Point(nextPoint.X, nextPoint.Y));
					}
				}
			}

		}
	}
	public class FlowFieldMoveCommand : ICommand
	{
		public Point From;
		public Point To;
		public FlowFieldMoveCommand(Point from, Point to)
		{
			//Debug.WriteLine("FlowFieldMoveCommand");
			From = from;
			To = to;
		}
	}

}