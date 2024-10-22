using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
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
        public override HashSet<Type> Signature { get; }

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
                            cursorPosition.Point = playerPosition.Point;
                            namelessGame.FollowedByCameraEntity = cursorEntity;
                            playerEntity.RemoveComponent(playerReceiver);

                        }
                    }
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
