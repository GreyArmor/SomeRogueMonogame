using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Rendering
{
    internal class UiIconComponent : Component
    {
        public UiIconComponent(string id)
        {
            IconId = id;
        }

        public string IconId { get; }

        public override IComponent Clone()
        {
            return new UiIconComponent(IconId);
        }
    }
}
