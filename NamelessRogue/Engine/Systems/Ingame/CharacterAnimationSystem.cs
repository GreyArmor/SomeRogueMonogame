using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS.Systems;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Status;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class CharacterAnimationSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>() { typeof(Character), typeof(SpritedObject) };

        bool once = false;
        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            foreach (var entity in RegisteredEntities)
            {
                var sprited = entity.GetComponentOfType<SpritedObject>();
                if (!sprited.IsStatic)
                {
                    if (sprited.CurrentAnimationTimeLeft <= 0)
                    {
                        var sprite = SpriteLibrary.SpritesAnimated[sprited.SpriteId];
                        var index = Random.Shared.Next(0, sprite._animationsByType[sprited.IdleAnimationType].Count);
                        var animationName = sprite._animationsByType[sprited.IdleAnimationType][index];
                        var animationDurationMS = sprite._animationsDurations[animationName];
                        sprited.CurrentAnimationTimeLeft = animationDurationMS;
                        sprited.CurrentAnimation = animationName;
                    }
                }
            }

            while (namelessGame.Commander.DequeueCommand(out PlayCharacterAnimationForATimeCommand command))
            {
                var entity = command.Entity;
                var sprited = entity.GetComponentOfType<SpritedObject>();

                if (sprited!=null && !sprited.IsStatic)
                {
                    var sprite = SpriteLibrary.SpritesAnimated[sprited.SpriteId];

                    if(!sprite._animationsByType[command.Type].Any())
                    {
                        continue;
                    }

                    var index = Random.Shared.Next(0, sprite._animationsByType[command.Type].Count);
                    var animationName = sprite._animationsByType[command.Type][index];

                    sprited.CurrentAnimation = animationName;
                    sprited.IdleAnimation = animationName;
                    sprited.CurrentAnimationTimeLeft = 1000;
                }
            }

            while (namelessGame.Commander.DequeueCommand(out PlayCharacterAnimationForNumberOfLoopsCommand command))
            {
                var entity = command.Entity;
                var sprited = entity.GetComponentOfType<SpritedObject>();

                if (sprited != null && !sprited.IsStatic)
                {
                    var sprite = SpriteLibrary.SpritesAnimated[sprited.SpriteId];

                    if (!sprite._animationsByType[command.Type].Any())
                    {
                        continue;
                    }

                    var index = Random.Shared.Next(0, sprite._animationsByType[command.Type].Count);
                    var animationName = sprite._animationsByType[command.Type][index];
                    var animationDurationMS = sprite._animationsDurations[animationName];
                    sprited.CurrentAnimation = animationName;
                    sprited.IdleAnimation = animationName;
                    sprited.CurrentAnimationTimeLeft = animationDurationMS * (command.LoopsCount);
                }
            }

            while (namelessGame.Commander.DequeueCommand(out LockIdleAnimationCommand command))
            {
                var entity = command.Entity;
                var sprited = entity.GetComponentOfType<SpritedObject>();

                if (sprited != null && !sprited.IsStatic)
                {
                    var sprite = SpriteLibrary.SpritesAnimated[sprited.SpriteId];
                    if (!sprite._animationsByType[command.Type].Any())
                    {
                        continue;
                    }
                    var index = Random.Shared.Next(0, sprite._animationsByType[command.Type].Count);
                    var animationName = sprite._animationsByType[command.Type][index];
                    sprited.IdleAnimation = animationName;
                    sprited.IdleAnimationType = command.Type;
                }
            }
                

              
        }
    }
}
