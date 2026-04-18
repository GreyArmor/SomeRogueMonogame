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
    public partial class AbilityTemplateEditorViewModel : BaseEditorViewModel
    {
        private AbilityTemplateData? ability;
        
        [ObservableProperty]
        private string id = string.Empty;
        
        [ObservableProperty]
        private string name = string.Empty;
        
        [ObservableProperty]
        private string description = string.Empty;
        
        [ObservableProperty]
        private string iconPath = string.Empty;
        
        [ObservableProperty]
        private int actionPointsCost;
        
        [ObservableProperty]
        private int energyCost;
        
        [ObservableProperty]
        private TargetMode targetMode = TargetMode.None;
        
        [ObservableProperty]
        private ActivationMode activationMode = ActivationMode.Passive;
        
        [ObservableProperty]
        private int areaOfEffect = 0;
        
        [ObservableProperty]
        private bool isActive;
        
        [ObservableProperty]
        private int range = 0;
        
        [ObservableProperty]
        private int cooldownTurns = 0;
               
        [ObservableProperty]
        private List<AssociatedBuff> associatedBuffs = new();
               
        [ObservableProperty]
        private List<AbilityAction> abilityActions = new();



        public AbilityTemplateEditorViewModel() : base("*.nraf", "Abilities")
        {
            ability = new AbilityTemplateData
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
            var path = Path.Combine(dir, $"{ability?.Name ?? "Ability"}.xml");
            using var stream = File.Create(path);
            serializer.Serialize(stream, ability);
            MessageBox.Show($"Saved to {path}");
        }

        [RelayCommand]
        private void AddAction()
        {
            ability?.AssociatedBuffs.Add(new AssociatedBuff());
            OnPropertyChanged(nameof(ability));
        }
    }
}
