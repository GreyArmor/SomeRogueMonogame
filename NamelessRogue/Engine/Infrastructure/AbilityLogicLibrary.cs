using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Infrastructure
{
    internal static class AbilityLogicLibrary
    {
        public static NamelessGame Game { get; private set; }
        public static void Init(NamelessGame game)
        {
            Game = game;
        }
        public static void Jump(IEntity jumpingEntity, Vector3Int destination)
        {
            Game.WorldProvider.MoveEntity(jumpingEntity, destination);            
        }
    }
}
