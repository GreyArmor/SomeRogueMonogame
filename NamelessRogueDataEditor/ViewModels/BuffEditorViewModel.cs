using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NamelessRogue.Engine.Generation.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace NamelessRogueDataEditor.ViewModels
{
    public partial class BuffEditorViewModel : BaseEditorViewModel<BuffTemplateData>
    {
 
        private BuffTemplateData buff;

        [ObservableProperty]
        public string name;
        [ObservableProperty]
        public string description;       

        [ObservableProperty]
        public int duration;

        [ObservableProperty]
        public bool isAppliedImmediately;

        [ObservableProperty]
        public bool permanentModifier;

        [ObservableProperty]
        public bool isDamageOverTime;

        [ObservableProperty]
        public int healthModificator = 0;

        [ObservableProperty]
        public int energyModificator = 0;

        [ObservableProperty]
        public int armorModificator = 0;

        [ObservableProperty]
        public int resistanceModificator = 0;

        [ObservableProperty]
        public int damageModificator = 0;
       
        public BuffEditorViewModel() : base("*.nrbf", "Buffs")
        {
            buff = new BuffTemplateData();       
            PropertyChanged += (s, e) => { CanSave = !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Description) && !string.IsNullOrEmpty(IconPath) && !string.IsNullOrEmpty(Id); };
        }    
        [RelayCommand]
        public void SelectIcon()
        {
            IconPath = _selectFile("*.png;*.jpg", "Select an icon");
        }

        protected override BuffTemplateData FillDataForSave()
        {
            buff.Id = id;
            buff.Name = name;
            buff.Description = description;
            buff.IconPath = iconPath;
            buff.Duration = duration;
            buff.IsAppliedImmediately = isAppliedImmediately;
            buff.PermanentModifier = permanentModifier;
            buff.IsDamageOverTime = isDamageOverTime;
            buff.HealthModificator = healthModificator;
            buff.EnergyModificator = energyModificator;
            buff.ArmorModificator = armorModificator;
            buff.ResistanceModificator = resistanceModificator;
            buff.DamageModificator = damageModificator;


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
            buff.IconPath = Path.GetRelativePath(directory, directory + "\\Icons\\" + iconFileName);
            IconPath = buff.IconPath;
            return buff;
        }

        protected override void FillDataFromSave(BuffTemplateData data)
        {
            buff = data;

            Id = data.Id;
            Name = data.Name;
            Description = data.Description;
            Duration = data.Duration;
            IsAppliedImmediately = data.IsAppliedImmediately;
            PermanentModifier = data.PermanentModifier;
            IsDamageOverTime = data.IsDamageOverTime;
            HealthModificator = data.HealthModificator;
            EnergyModificator = data.EnergyModificator;
            ArmorModificator = data.ArmorModificator;
            ResistanceModificator = data.ResistanceModificator;
            DamageModificator = data.DamageModificator;

            if (!string.IsNullOrEmpty(data.IconPath))
            {
                var resolved = data.IconPath;
                try
                {
                    if (!Path.IsPathFullyQualified(resolved))
                    {
                        var baseDir = string.IsNullOrEmpty(editorPath) ? Directory.GetCurrentDirectory() : editorPath;
                        resolved = Path.GetFullPath(Path.Combine(baseDir, data.IconPath));
                    }
                }
                catch
                {
                    resolved = data.IconPath;
                }

                IconPath = resolved;
            }
            else
            {
                IconPath = string.Empty;
            }
        }
    }
}
