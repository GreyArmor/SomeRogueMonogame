using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ICommand = NamelessRogue.Engine.Abstraction.ICommand;

namespace NamelessRogue.Engine.Systems.Editors
{
    public class CharacterScreeChangeSpriteCommand : ICommand
    {
        public CharacterScreeChangeSpriteCommand(string spriteId)
        {
            SpriteId = spriteId;
        }

        public string SpriteId { get; }
    }

    public class CharacterScreeChangeSpriteAnimationCommand : ICommand
    {
        public CharacterScreeChangeSpriteAnimationCommand(string animationId)
        {
            AnimationId = animationId;
        }

        public string AnimationId { get; }
    }
}
