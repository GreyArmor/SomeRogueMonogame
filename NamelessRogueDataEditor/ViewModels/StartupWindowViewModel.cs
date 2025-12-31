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
    public partial class StartupWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Object> rootDirectoryItems = new ObservableCollection<object>();
        [RelayCommand]
        void SaveFile() {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

     
            Nullable<bool> result = dlg.ShowDialog();

            if (result.Value)
            {
                string filename = dlg.FileName;
            }
        }


        [RelayCommand]
        void NewItem() { App.Current.MainWindow.DataContext = new ItemEditorViewModel(new NamelessRogue.Engine.Generation.Editor.ItemTemplateData()); }

        [RelayCommand]
        void NewFile() {}

        [RelayCommand]
        void LoadFile() { /* ... */ }
    }
}
