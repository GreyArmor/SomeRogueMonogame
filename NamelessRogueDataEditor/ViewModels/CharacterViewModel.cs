using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogueDataEditor.ViewModels
{
    public partial class CharacterViewModel : BaseEditorViewModel
    {
        public CharacterViewModel() : base("*.nrcf", "Characters")
        {
        }
    }
}
