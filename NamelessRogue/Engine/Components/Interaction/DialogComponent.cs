using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
    internal class DialogComponent : Component
    {
        public string DialogLibraryId {  get; set; }

        public DialogComponent(string dialogLibraryId)
        {
            DialogLibraryId = dialogLibraryId;
        }

        public DialogComponent()
        {
        }
        public override IComponent Clone()
        {
           return new DialogComponent(DialogLibraryId);
        }
    }
}
