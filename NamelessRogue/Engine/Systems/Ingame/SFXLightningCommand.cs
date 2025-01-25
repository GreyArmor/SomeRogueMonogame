using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Utility;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class SFXLightningCommand : ICommand
    {
        public SFXLightningCommand(Vector3Int start, Vector3Int end)
        {
            Start = start;
            End = end;
        }

        public Vector3Int Start { get; }
        public Vector3Int End { get; }
    }
}