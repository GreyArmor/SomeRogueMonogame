using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS.Systems;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.Environment;
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
                if (sprited.CurrentAnimationTimeLeft<=0)
                { 
                    sprited.CurrentAnimationTimeLeft = 1000;
                    var idleIndex = Random.Shared.Next(1, 3);
                    sprited.CurrentAnimation = @$"idle_{idleIndex}";
                }
            }


            while (namelessGame.Commander.DequeueCommand(out PlayCharacterAnimationCommand command))
            {
                var entity = command.Entity;
                var sprited = entity.GetComponentOfType<SpritedObject>();
                sprited.CurrentAnimation = "attack_2";
                sprited.CurrentAnimationTimeLeft = 1000;
            }
        }
    }
}
