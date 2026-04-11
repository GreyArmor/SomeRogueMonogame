using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Generation.Editor;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace NamelessRogueDataEditor.ViewModels
{
    public partial class AbilityTemplateEditorViewModel : ObservableObject
    {
        [ObservableProperty]
        private AbilityTemplateData? ability;

        public AbilityTemplateEditorViewModel()
        {
            Ability = new AbilityTemplateData
            {
                AssociatedBuffs = [],
                AbilityActions = []
            };
        }

        [RelayCommand]
        private void Save()
        {
            var serializer = new XmlSerializer(typeof(AbilityTemplateData));
            var dir = Path.Combine("..", "NamelessRogue", "Content", "GameObjects", "Abilities");
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, $"{Ability?.Name ?? "Ability"}.xml");
            using var stream = File.Create(path);
            serializer.Serialize(stream, Ability);
            MessageBox.Show($"Saved to {path}");
        }

        [RelayCommand]
        private void AddAction()
        {
            Ability?.AssociatedBuffs.Add(new AssociatedBuff());
            OnPropertyChanged(nameof(Ability));
        }
    }
}
