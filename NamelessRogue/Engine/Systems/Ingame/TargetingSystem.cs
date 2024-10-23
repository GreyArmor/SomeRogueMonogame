using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
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
        public static TargetingState State { get; set; }  = TargetingState.NotTargeting;
        public override HashSet<Type> Signature { get; } = new HashSet<Type>() {  typeof(AIControlled)};

        int tabulationIndex = -1;
        public List<IEntity> Targets { get; set; } = new List<IEntity> { };

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out StartTargetingCommand command))
            {
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

                            foreach (var npc in RegisteredEntities)
                            {
                                var aiControlled = npc.GetComponentOfType<AIControlled>();
                                if(aiControlled.Affinity == Affinity.Hostile)
                                {
                                    Targets.Add(npc);
                                }
                            }
                            if (Targets.Count > 0)
                            {
                                tabulationIndex = 0;
                                var targetPosition = Targets[tabulationIndex].GetComponentOfType<Position>();
                                cursorPosition.Point = targetPosition.Point;
                            }
                            else
                            {
                                cursorPosition.Point = playerPosition.Point;
                            }
                        }
                    }
                }
            }

            while (namelessGame.Commander.DequeueCommand(out TabTargetingCommand command))
            {
                if (Targets.Count > 0)
                {
                    IEntity cursorEntity = namelessGame.CursorEntity;
                    Position cursorPosition = cursorEntity.GetComponentOfType<Position>();
                    tabulationIndex++;
                    if(tabulationIndex >= Targets.Count)
                    {
                        tabulationIndex = 0;
                    }
                    var targetPosition = Targets[tabulationIndex].GetComponentOfType<Position>();
                    cursorPosition.Point = targetPosition.Point;
                }
            }

                while (namelessGame.Commander.DequeueCommand(out EndTargetingCommand command))
            {
                {
                    if (State == TargetingState.Targeting)
                    {
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
}
