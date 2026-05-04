using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NamelessRogueDataEditor.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<object> TabItems { get; set; }

        private object _selectedTab;
        public object SelectedTab
        {
            get => _selectedTab;
            set { _selectedTab = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            TabItems = new ObservableCollection<object> {
            new BuffEditorViewModel { HeaderName = "Buff" },
            new AbilityTemplateEditorViewModel { HeaderName = "Ability" },
            new ItemEditorViewModel() { HeaderName = "Item" },
            new CharacterViewModel { HeaderName = "Character" },
    
        };
            SelectedTab = TabItems[0]; // Set default tab
        }
    }
}
