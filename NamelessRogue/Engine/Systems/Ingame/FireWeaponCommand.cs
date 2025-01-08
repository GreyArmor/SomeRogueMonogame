using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class FireWeaponCommand : Abstraction.ICommand
    {
        public FireWeaponCommand(IEntity source, Vector3Int target)
        {
            Source = source;
            Target = target;
        }

        public IEntity Source { get; }
        public Vector3Int Target { get; }
    }

    internal class AttachToTargetCommand : Abstraction.ICommand
    {
        public AttachToTargetCommand(Entity tileEntity)
        {
            TileEntity = tileEntity;
        }

        public Entity TileEntity { get; }
    }

    internal class DetachFromToTargetCommand : Abstraction.ICommand
    {
    }
}
