using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.AI.Pathfinder;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NamelessRogue.Engine.Systems.Ingame
{
	public class PedestrianAiProcessor
    {
        public void ProcessPedestrianAi(IEntity entity, NamelessGame game)
        {
            var flowMoveComponent = entity.GetComponentOfType<FlowMoveComponent>();
            if (flowMoveComponent != null)
            {
                if (pedestrianAiStateActions.TryGetValue(flowMoveComponent.CurrentState, out var action))
                {
                    action(entity, flowMoveComponent, game);
                }
                else
                {
                }
            }
        }
        Dictionary<PedestrianAiState, Action<IEntity, FlowMoveComponent, NamelessGame>> pedestrianAiStateActions = new Dictionary<PedestrianAiState, Action<IEntity, FlowMoveComponent, NamelessGame>>()
        {
            { PedestrianAiState.Start, HandleStartState },
            { PedestrianAiState.Wait, HandleWaitState },
            { PedestrianAiState.Move, HandleMoveState },
            { PedestrianAiState.WaitForCrossing, HandleWaitForCrossingState },
            { PedestrianAiState.CrossingMove, HandleCrossingMoveState },
            { PedestrianAiState.Finished, HandleFinishedState }
        };

		private static void HandleStartState(IEntity entity, FlowMoveComponent flowMoveComponent, NamelessGame game)
		{
			flowMoveComponent.Log += "start->";
			var entityPos = entity.GetComponentOfType<Position>().Point;
			var chunkPos = new Vector3Int(entityPos.X / Constants.ChunkSize, entityPos.Y / Constants.ChunkSize, 0);
			var closestLocation = game.MacroNavigator.Locations.Where(l => l.BoundingBox.IsPointInside(entityPos.X, entityPos.Y)).
									 FirstOrDefault(x => x.InternalNodes.Any());

			if (closestLocation == null || closestLocation.InternalNodes.Count == 0)
			{
				entity.RemoveComponentOfType<AIControlled>();
				return;
			}
			var waypoints = closestLocation.InternalNodes;

			flowMoveComponent.CurrentMacroNode = waypoints.ToList()[Random.Shared.Next(waypoints.Count)];			

			CalcultePath(flowMoveComponent);

			flowMoveComponent.CurrentState = PedestrianAiState.Move;
			

		}

		private static void CalcultePath(FlowMoveComponent flowMoveComponent)
		{
			var closestWaypoint = flowMoveComponent.CurrentMacroNode;

			if (flowMoveComponent.CurrentMacroNode.IsCrossingNode())
			{
				var closestWaypointRealityPosition = closestWaypoint.RealityPosition;
				var nextCrossingConnection = closestWaypoint.GetCrossingConnections().FirstOrDefault();
				var pathChain = new List<int>() { closestWaypoint.LocationPathId, nextCrossingConnection.PathId };
				var nodeChain = new List<MacroNode>() { closestWaypoint, nextCrossingConnection.Node };
				flowMoveComponent.To = closestWaypoint.RealityPosition.ToPoint();
				flowMoveComponent.PathChain = pathChain;
				flowMoveComponent.NodeChain = nodeChain;
				flowMoveComponent.CurrentPathIndex = 0;
			}
			else
			{
				var pathId = closestWaypoint.LocationPathId;
				if (pathId > -1)
				{
					flowMoveComponent.To = closestWaypoint.RealityPosition.ToPoint();
					flowMoveComponent.PathChain = new List<int>() { pathId };
					flowMoveComponent.NodeChain = new List<MacroNode>() { closestWaypoint };	
					flowMoveComponent.CurrentPathIndex = 0;
				}
			}
		}

		private static void HandleFinishedState(IEntity entity, FlowMoveComponent flowMoveComponent, NamelessGame game)
        {
			flowMoveComponent.Log += "finished->";
			var entityPos = entity.GetComponentOfType<Position>().Point;
			var chunkPos = new Vector3Int(entityPos.X / Constants.ChunkSize, entityPos.Y / Constants.ChunkSize, 0);
			var closestLocation = game.MacroNavigator.Locations.Where(l => l.BoundingBox.IsPointInside(entityPos.X, entityPos.Y)).
									 FirstOrDefault(x => x.InternalNodes.Any());

			if (closestLocation == null || closestLocation.InternalNodes.Count == 0)
			{
				entity.RemoveComponentOfType<AIControlled>();
				return;
			}
			var waypoints = closestLocation.InternalNodes;

			flowMoveComponent.CurrentMacroNode = waypoints.ToList()[Random.Shared.Next(waypoints.Count)];

			CalcultePath(flowMoveComponent);

			flowMoveComponent.CurrentState = PedestrianAiState.Move;
		}

        private static void HandleCrossingMoveState(IEntity entity, FlowMoveComponent flowMoveComponent, NamelessGame game)
        {
			flowMoveComponent.Log += "crossingmove->";
			var entityPos = entity.GetComponentOfType<Position>().Point;
			try
			{
				var hasNext = game.PathfindingController.GetNextPoint(flowMoveComponent.PathId, entityPos.ToPoint(), out Microsoft.Xna.Framework.Point? nextPoint);
				if (hasNext && nextPoint.HasValue)
				{
					if (flowMoveComponent.To == nextPoint.Value)
					{
						flowMoveComponent.CurrentPathIndex++;
						if (flowMoveComponent.PathId == -1)
						{
							flowMoveComponent.CurrentState = PedestrianAiState.Finished;
							flowMoveComponent.CurrentMacroNode = null;
						}
						else
						{
							flowMoveComponent.To = game.PathfindingController.GetPathEndPoint(flowMoveComponent.PathChain[flowMoveComponent.CurrentPathIndex]);

							if (flowMoveComponent.NodeChain[flowMoveComponent.CurrentPathIndex].IsCrossingNode())
							{
								flowMoveComponent.CurrentState = PedestrianAiState.CrossingMove;
							}
							else
							{
								flowMoveComponent.CurrentState = PedestrianAiState.Move;
							}
						}
					}
					game.WorldProvider.MoveEntityIgnoreCharacters(entity,
					nextPoint.Value.X, nextPoint.Value.Y, 0);


				}
				else
				{
					flowMoveComponent.CurrentState = PedestrianAiState.Finished;
					flowMoveComponent.CurrentMacroNode = null;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error moving entity {entity.Id}: {ex.Message}");
				Debug.WriteLine($"entityPos {entityPos}");
				Debug.WriteLine($"flowMoveComponent.To {flowMoveComponent.To}");
				Debug.WriteLine($"flowMoveComponent.CurrentMacroNode.FlowFieldId {flowMoveComponent.CurrentMacroNode.LocationPathId}");
				Debug.WriteLine($"flowMoveComponent.CurrentMacroNode.RealityPosition {flowMoveComponent.CurrentMacroNode.RealityPosition}");
				flowMoveComponent.CurrentState = PedestrianAiState.Finished;
				flowMoveComponent.CurrentMacroNode = null;
			}
			entity.GetComponentOfType<ActionPoints>().Points = -200;
		}

        private static void HandleWaitForCrossingState(IEntity entity, FlowMoveComponent flowMoveComponent, NamelessGame game)
        {
			flowMoveComponent.Log += "crossingwait->";
			var verticalCrossingEnabled = game.StreetLightsKeeper.GetComponentOfType<StreetlightsStatusKeeper>().VerticalMovementAllowed;
			var waitingForVertical = flowMoveComponent.NodeChain[flowMoveComponent.CurrentPathIndex].IsCrossingNodeVertical();
			if (verticalCrossingEnabled)
			{
				if (waitingForVertical)
				{
					flowMoveComponent.CurrentState = PedestrianAiState.CrossingMove;
					return;
				}
			}
			else
			{
				if (!waitingForVertical)
				{
					flowMoveComponent.CurrentState = PedestrianAiState.CrossingMove;
					return;
				}
			}
			entity.GetComponentOfType<ActionPoints>().Points = -200;
		}

        private static void HandleMoveState(IEntity entity, FlowMoveComponent flowMoveComponent, NamelessGame game)
        {
			flowMoveComponent.Log += "move->";
			var entityPos = entity.GetComponentOfType<Position>().Point;
			try
			{
				var hasNext = game.PathfindingController.GetNextPoint(flowMoveComponent.PathId, entityPos.ToPoint(), out Microsoft.Xna.Framework.Point? nextPoint);
				if (hasNext && nextPoint.HasValue)
				{
					if (flowMoveComponent.To == nextPoint.Value)
					{
						flowMoveComponent.CurrentPathIndex++;
						if (flowMoveComponent.PathId == -1)
						{
							flowMoveComponent.CurrentState = PedestrianAiState.Finished;
							flowMoveComponent.CurrentMacroNode = null;
						}
						else
						{
							flowMoveComponent.To = game.PathfindingController.GetPathEndPoint(flowMoveComponent.PathChain[flowMoveComponent.CurrentPathIndex]);
							if (flowMoveComponent.NodeChain[flowMoveComponent.CurrentPathIndex].IsCrossingNode())
							{
								flowMoveComponent.CurrentState = PedestrianAiState.WaitForCrossing;
							}
						}
					}
					//MoveEntitySwapCharacters(entity, worldProvider, namelessGame,
					//  nextPoint.Value.X, nextPoint.Value.Y, 0);
					game.WorldProvider.MoveEntityIgnoreCharacters(entity,
					nextPoint.Value.X, nextPoint.Value.Y, 0);

				

				}
				else
				{
					flowMoveComponent.CurrentState = PedestrianAiState.Finished;
					flowMoveComponent.CurrentMacroNode = null;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error moving entity {entity.Id}: {ex.Message}");
				Debug.WriteLine($"entityPos {entityPos}");
				Debug.WriteLine($"flowMoveComponent.To {flowMoveComponent.To}");
				Debug.WriteLine($"flowMoveComponent.CurrentMacroNode.FlowFieldId {flowMoveComponent.CurrentMacroNode.LocationPathId}");
				Debug.WriteLine($"flowMoveComponent.CurrentMacroNode.RealityPosition {flowMoveComponent.CurrentMacroNode.RealityPosition}");
				flowMoveComponent.CurrentState = PedestrianAiState.Finished;
				flowMoveComponent.CurrentMacroNode = null;
			}
			entity.GetComponentOfType<ActionPoints>().Points = -200;
		}

        private static void HandleWaitState(IEntity entity, FlowMoveComponent flowMoveComponent, NamelessGame game)
        {
            throw new NotImplementedException();
        }
    
    }
}




   

