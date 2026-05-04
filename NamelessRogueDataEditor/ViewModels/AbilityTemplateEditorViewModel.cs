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
        private ObservableCollection<AssociatedBuff> associatedBuffs = new();
               
        [ObservableProperty]
        private ObservableCollection<AbilityAction> abilityActions = new();

        [ObservableProperty]
        private int selectedAssociatedBuffIndex = 0;
        [ObservableProperty]
        private int selectedAbilityActionIndex = 0;

        [ObservableProperty]
        private ObservableCollection<AbilityAction> abilityActionsEnumValue = new ObservableCollection<AbilityAction>(Enum.GetValues(typeof(AbilityAction)).Cast<AbilityAction>());


        public AbilityTemplateEditorViewModel() : base("*.nraf", "Abilities")
        {
            ability = new AbilityTemplateData
            {
                AssociatedBuffs = [],
                AbilityActions = []
            };

            PropertyChanged += (s, e) => { CanSave = !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Description) && !string.IsNullOrEmpty(IconPath) && !string.IsNullOrEmpty(Id); };
        }

        [RelayCommand]
        private void AddAssociatedBuff()
        {
            var file = this._selectFile("*.nrbf", "Select a buff to associate");
            if (!string.IsNullOrEmpty(file) && File.Exists(file))
            {
                string buffId;
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(BuffTemplateData));
                using (FileStream fs = new(file, FileMode.Open))
                {
                    buffId = ((BuffTemplateData)serializer.Deserialize(fs)).Id ?? string.Empty;
                }

                    var relativePath = Path.GetRelativePath(editorPath, file);
                AssociatedBuffs.Add(new AssociatedBuff { BuffId = buffId, Path = relativePath });
                
            }
        }

        [RelayCommand]
        public virtual void RemoveAssociatedBuff()
        {
            if (!AssociatedBuffs.Any())
            {
                return;
            }
            var selectedFile = AssociatedBuffs[SelectedAssociatedBuffIndex];
            AssociatedBuffs.Remove(selectedFile);
            if (!AssociatedBuffs.Any() && SelectedAssociatedBuffIndex > 0)
            {
                SelectedAssociatedBuffIndex--;
            }
            
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
            IconPath = newIconLocation;
            return ability;
        }

        protected override void FillDataFromSave(AbilityTemplateData data)
        {
            if (data == null)
            {
                return;
            }

            ability = data;

            Id = data.Id ?? string.Empty;
            Name = data.Name ?? string.Empty;
            Description = data.Description ?? string.Empty;

            if (string.IsNullOrWhiteSpace(data.IconPath))
            {
                IconPath = string.Empty;
            }
            else if (Path.IsPathFullyQualified(data.IconPath))
            {
                IconPath = data.IconPath;
            }
            else
            {
                var baseDir = string.IsNullOrEmpty(editorPath) ? Directory.GetCurrentDirectory() : editorPath;
                IconPath = Path.GetFullPath(Path.Combine(baseDir, data.IconPath));
            }

            ActionPointsCost = data.ActionPointsCost;
            EnergyCost = data.EnergyCost;
            TargetMode = data.TargetMode;
            ActivationMode = data.ActivationMode;
            AreaOfEffect = data.AreaOfEffect;
            IsActive = data.IsActive;
            Range = data.Range;
            CooldownTurns = data.CooldownTurns;

            AssociatedBuffs = data.AssociatedBuffs != null
                ? new ObservableCollection<AssociatedBuff>(data.AssociatedBuffs)
                : new ObservableCollection<AssociatedBuff>();

            AbilityActions = data.AbilityActions != null
                ? new ObservableCollection<AbilityAction>(data.AbilityActions)
                : new ObservableCollection<AbilityAction>();
        }

        [RelayCommand]
        public void AddAbilityAction(AbilityAction action)
        {
            AbilityActions.Add(action);
        }
        [RelayCommand]
        public void RemoveAbilityAction(AbilityAction action)
        {
            AbilityActions.Remove(action);
        }

        [RelayCommand]
        public void AbilityActionUp(AbilityAction action)
        {
            var index = AbilityActions.IndexOf(action);
            if (index > 0)
            {
                AbilityActions.Move(index, index - 1);
            }
        }

        [RelayCommand]
        public void AbilityActionDown(AbilityAction action)
        {
            var index = AbilityActions.IndexOf(action);
            if (index >= 0 && index < AbilityActions.Count-1)
            {
                AbilityActions.Move(index, index + 1);
            }
        }
    }
}
