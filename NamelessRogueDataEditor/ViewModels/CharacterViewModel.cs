using CommunityToolkit.Mvvm.ComponentModel;
using NamelessRogue.Engine.Generation.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogueDataEditor.ViewModels
{
    public partial class CharacterViewModel : BaseEditorViewModel<CharacterTemplateData>
    {
        public CharacterViewModel() : base("*.nrcf", "Characters")
        {
        }

        protected override CharacterTemplateData FillDataForSave()
        {
            return new CharacterTemplateData();            
        }

        protected override void FillDataFromSave(CharacterTemplateData data)
        {
            throw new NotImplementedException();
        }
    }
}
