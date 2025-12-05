using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NamelessRogueDataEditor.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [RelayCommand]
        void SaveFile() { /* ... */ }

        [RelayCommand]
        void NewFile() { /* ... */ }

        [RelayCommand]
        void LoadFile() { /* ... */ }
    }
}
