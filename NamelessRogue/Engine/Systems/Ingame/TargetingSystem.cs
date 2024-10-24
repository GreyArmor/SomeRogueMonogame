using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public enum TargetingState
    {
        NotTargeting,
        Targeting
    }
    public class TargetingSystem : BaseSystem
    {
        public static TargetingState State { get; set; } = TargetingState.NotTargeting;
        public override HashSet<Type> Signature { get; } = new HashSet<Type>() { typeof(AIControlled) };


        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            var targeter = namelessGame.TargeterEntity.GetComponentOfType<TergeterComponent>();
            while (namelessGame.Commander.DequeueCommand(out StartTargetingCommand command))
            {
                if (State == TargetingState.NotTargeting)
                {
                    var playerEntity = namelessGame.PlayerEntity;
                    State = TargetingState.Targeting;
                    InputReceiver receiver = new InputReceiver();

                    IEntity cursorEntity = namelessGame.CursorEntity;

                    var playerReceiver = playerEntity.GetComponentOfType<InputReceiver>();
                    playerEntity.RemoveComponentOfType<InputReceiver>();

                    if (playerReceiver != null)
                    {
                        cursorEntity.AddComponent(receiver);
                        Drawable cursorDrawable = cursorEntity.GetComponentOfType<Drawable>();
                        cursorDrawable.Visible = true;
                        Position cursorPosition = cursorEntity.GetComponentOfType<Position>();
                        Position playerPosition = playerEntity.GetComponentOfType<Position>();

                        namelessGame.FollowedByCameraEntity = cursorEntity;
                        playerEntity.RemoveComponent(playerReceiver);
                        List<IEntity> hostileEntities = new List<IEntity>();
                        foreach (var npc in RegisteredEntities)
                        {
                            var aiControlled = npc.GetComponentOfType<AIControlled>();                         
                            if (aiControlled.Affinity == Affinity.Hostile)
                            {
                                hostileEntities.Add(npc);
                            }
                        }

                        hostileEntities = hostileEntities.OrderBy(entity => (entity.GetComponentOfType<Position>().Point - playerPosition.Point).Length()).ToList();

                        targeter.Targets.AddRange(hostileEntities);

                        if (targeter.Targets.Count > 0)
                        {
                            targeter.TabulationIndex = 0;
                            var targetPosition = targeter.Targets[targeter.TabulationIndex].GetComponentOfType<Position>();
                            cursorPosition.Point = targetPosition.Point;
                        }
                        else
                        {
                            cursorPosition.Point = playerPosition.Point;
                        }
                    }
                }
            }

            while (namelessGame.Commander.DequeueCommand(out TabTargetingCommand command))
            {
                if (targeter.Targets.Count > 0)
                {
                    IEntity cursorEntity = namelessGame.CursorEntity;
                    Position cursorPosition = cursorEntity.GetComponentOfType<Position>();
                    targeter.TabulationIndex++;
                    if (targeter.TabulationIndex >= targeter.Targets.Count)
                    {
                        targeter.TabulationIndex = 0;
                    }
                    var targetPosition = targeter.Targets[targeter.TabulationIndex].GetComponentOfType<Position>();
                    cursorPosition.Point = targetPosition.Point;
                }
            }

            while (namelessGame.Commander.DequeueCommand(out EndTargetingCommand command))
            {
                if (State == TargetingState.Targeting)
                {
                    targeter.Targets.Clear();
                    var playerEntity = namelessGame.PlayerEntity;
                    State = TargetingState.NotTargeting;
                    InputReceiver receiver = new InputReceiver();
                    IEntity cursorEntity = namelessGame.CursorEntity;
                    var playerReceiver = playerEntity.GetComponentOfType<InputReceiver>();
                    var cursorReceiver = namelessGame.CursorEntity.GetComponentOfType<InputReceiver>();
                    playerEntity.RemoveComponentOfType<InputReceiver>();
                    if (cursorReceiver != null)
                    {
                        playerEntity.AddComponent(receiver);
                        Drawable cursorDrawable = cursorEntity.GetComponentOfType<Drawable>();
                        cursorDrawable.Visible = false;
                        cursorEntity.RemoveComponent(cursorReceiver);
                        namelessGame.FollowedByCameraEntity = playerEntity;
                    }
                }
            }
        }
    }
}
