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
    public partial class BuffEditorViewModel : ObservableObject
    {
        string buffDirectory;
        private BuffTemplateData buff;

        List<string> buffFiles;
        [ObservableProperty]
        List<string> buffFileNames;

        [ObservableProperty]
        public string id;
        [ObservableProperty]
        public string name;
        [ObservableProperty]
        public string description;

        [ObservableProperty]
        public string iconPath;

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

        [ObservableProperty]
        bool canSave = false;
        public BuffEditorViewModel()
        {
            buff = new BuffTemplateData();
            buffDirectory = System.IO.Path.Combine(ContentDirectoryHelper.contentDirectoryPath, "GameObjects", "Buffs");
            buffFiles = Directory.EnumerateFiles(buffDirectory, "*.nrbf").ToList();
            buffFileNames = buffFiles.Select(x=>Path.GetFileName(x)).ToList();

            PropertyChanged += (s, e) => { CanSave = !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Description) && !string.IsNullOrEmpty(IconPath) && !string.IsNullOrEmpty(Id); };
        }

        [RelayCommand]
        public void Back()
        {
            App.Current.MainWindow.DataContext = new StartupWindowViewModel();
        }

        [RelayCommand]
        public void Save()
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


            var directory = System.IO.Path.Combine(ContentDirectoryHelper.contentDirectoryPath, "GameObjects", "Buffs");
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
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(BuffTemplateData));
            var dir = System.IO.Path.Combine(ContentDirectoryHelper.contentDirectoryPath, "GameObjects", "Buffs");
            System.IO.Directory.CreateDirectory(dir);
            var path = System.IO.Path.Combine(dir, $"{buff?.Name ?? "Buff"}.nrbf");
            using var stream = System.IO.File.Create(path);
            serializer.Serialize(stream, buff);
            System.Windows.MessageBox.Show($"Saved to {path}");
        }

        [RelayCommand]
        public void SelectIcon()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            var dir = ContentDirectoryHelper.contentDirectoryPath;
            System.IO.Directory.CreateDirectory(dir);
            fileDialog.InitialDirectory = dir;
            fileDialog.Filter = "Files|*.png;*.jpg";
            fileDialog.Title = "Select an icon";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(fileDialog.FileName))
                {
                    IconPath = fileDialog.FileName;
                }
            }
          
            fileDialog = null;
        }

      
    }
}
