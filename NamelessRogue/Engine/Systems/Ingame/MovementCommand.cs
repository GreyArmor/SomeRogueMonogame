using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Ingame
{


    public enum MovementDirection
    {
       None, Left, Right, Top, Bottom, Down, Up,
    }
    internal class PlayerMovementCommand : ICommand
    {
        public PlayerMovementCommand(IEnumerable<MovementDirection> directions)
        {
            Directions = directions;
        }
        public IEnumerable<MovementDirection> Directions { get; }
    }

    internal class CursorMovementCommand : ICommand
    {
        public CursorMovementCommand(IEnumerable<MovementDirection> directions)
        {
            Directions = directions;
        }
        public IEnumerable<MovementDirection> Directions { get; }
    }
}
