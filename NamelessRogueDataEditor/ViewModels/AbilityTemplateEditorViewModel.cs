using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Generation.Editor;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace NamelessRogueDataEditor.ViewModels
{
    public partial class AbilityTemplateEditorViewModel : BaseEditorViewModel<AbilityTemplateData>
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
        private void AddAction()
        {
            ability?.AssociatedBuffs.Add(new AssociatedBuff());
            OnPropertyChanged(nameof(ability));
        }


        protected override AbilityTemplateData FillDataForSave()
        {
            ability.Id = id;
            ability.Name = name;
            ability.Description = description;
            ability.IconPath = iconPath;
            ability.ActionPointsCost = actionPointsCost;
            ability.EnergyCost = energyCost;
            ability.TargetMode = targetMode;
            ability.ActivationMode = activationMode;
            ability.AreaOfEffect = areaOfEffect;
            ability.IsActive = isActive;
            ability.Range = range;
            ability.CooldownTurns = cooldownTurns;
            ability.AssociatedBuffs = associatedBuffs.ToList();
            ability.AbilityActions = abilityActions.ToList();
            var directory = editorPath;
            var iconFileName = Path.GetFileName(iconPath);
            if (!Directory.Exists(directory + "\\Icons\\"))
            {
                Directory.CreateDirectory(directory + "\\Icons\\");
            }

            //move icon to local directory
            var newIconLocation = directory + "\\Icons\\" + iconFileName;

            if (Path.IsPathFullyQualified(iconPath))
            {
                if (iconPath != newIconLocation)
                {
                    File.Copy(iconPath, newIconLocation, true);
                }
            }
            ability.IconPath = Path.GetRelativePath(directory, directory + "\\Icons\\" + iconFileName);
            IconPath = ability.IconPath;
            return ability;
        }

    }
}
