using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogueDataEditor.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using NamelessRogue.Engine.Components.Interaction;
    using NamelessRogue.Engine.Components.ItemComponents;
    using NamelessRogue.Engine.Components.Stats;
    using NamelessRogue.Engine.Generation.Editor;
    using NamelessRogue.Engine.Serialization;
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using System.Windows.Input;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using static NamelessRogue.Engine.Generation.Editor.QuestTemplateData;

    public partial class ItemEditorViewModel : BaseEditorViewModel<ItemTemplateData>
    {
        [ObservableProperty]
        ObservableCollection<ItemQuality> itemQualityValues = new ObservableCollection<ItemQuality>(Enum.GetValues<ItemQuality>());
        [ObservableProperty]
        ObservableCollection<ItemType> itemTypeValues = new ObservableCollection<ItemType>(Enum.GetValues<ItemType>());
        [ObservableProperty]
        ObservableCollection<AttackType> attackTypeValue = new ObservableCollection<AttackType>(Enum.GetValues<AttackType>());
        [ObservableProperty]
        ObservableCollection<DamageType> damageTypeValues = new ObservableCollection<DamageType>(Enum.GetValues<DamageType>());
        [ObservableProperty]
        ObservableCollection<AmmoType> ammoTypeValues = new ObservableCollection<AmmoType>(Enum.GetValues<AmmoType>());
        //[ObservableProperty]
        //ObservableCollection<ItemType> itemTypeValues = new ObservableCollection<ItemType>(Enum.GetValues<ItemType>());

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        private int price;

        [ObservableProperty]
        private ItemType itemType;

        [ObservableProperty]
        private ItemQuality itemQuality;

        public ObservableCollection<Slot> PossibleSlots { get; } = new ObservableCollection<Slot>();

        [ObservableProperty]
        private WeaponTemplateData weaponTemplateData;

        [ObservableProperty]
        private ArmorTemplateData armorTemplateData;

        [ObservableProperty]
        private ConsumableItemTemplateData consumableItemTemplateData;

        [ObservableProperty]
        private ObservableCollection<FileReference> associatedBuffs = new ObservableCollection<FileReference>();

        [ObservableProperty]
        private int selectedAssociatedBuffIndex = 0;
        [ObservableProperty]
        private int selectedSlotIndex = 0;

        public ItemEditorViewModel() : base("*.nrif","")
        {
            PropertyChanged += (s, e) => { CanSave = !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Description) && !string.IsNullOrEmpty(IconPath) && !string.IsNullOrEmpty(Id); };
        }

        private WeaponTemplateData CloneWeaponTemplateData(WeaponTemplateData source) =>
            new WeaponTemplateData
            {
                MinimumDamage = source.MinimumDamage,
                MaximumDamage = source.MaximumDamage,
                Range = source.Range,
                AttackType = source.AttackType,
                DamageType = source.DamageType,
                AmmoType = source.AmmoType,
                AmmoInClip = source.AmmoInClip
            };

        private ArmorTemplateData CloneArmorTemplateData(ArmorTemplateData source) =>
            new ArmorTemplateData
            {
                DamageType = source.DamageType,
                ArmorValue = source.ArmorValue,
                ResistType = source.ResistType,
                ResistValue = source.ResistValue
            };

        private ConsumableItemTemplateData CloneConsumableItemTemplateData(ConsumableItemTemplateData source) =>
            new ConsumableItemTemplateData
            {
                Charges = source.Charges,
                Duration = source.Duration,
                IsThrowable = source.IsThrowable,
                IsAppliedImmediately = source.IsAppliedImmediately,
                IsDamageOverTime = source.IsDamageOverTime,
                HealthModificator = source.HealthModificator,
                EnergyModificator = source.EnergyModificator,
                ArmorModificator = source.ArmorModificator,
                ResistanceModificator = source.ResistanceModificator,
                DamageModificator = source.DamageModificator
            };

        public override void Save()
        {
            string directory = GetItemDirectory();
            ItemTemplateData objecToSave = FillDataForSave();
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ItemTemplateData));
            var dir = System.IO.Path.Combine(ContentDirectoryHelper.contentDirectoryPath, "GameObjects", directory);
            System.IO.Directory.CreateDirectory(dir);
            var path = System.IO.Path.Combine(dir, $"{Id + Path.GetExtension(this.FileType)}");
            using var stream = System.IO.File.Create(path);
            serializer.Serialize(stream, objecToSave);
            System.Windows.MessageBox.Show($"Saved to {path}");
            ReloadFiles(FileType, FilesDirectory);
        }

        private string GetItemDirectory()
        {
            var directory = "";
            var currentItemType = itemType;
            switch (itemType)
            {
                case ItemType.Weapon:
                    directory = ContentDirectoryHelper.contentDirectoryPath + "GameObjects\\Weapons\\";
                    break;
                case ItemType.Armor:
                    directory = ContentDirectoryHelper.contentDirectoryPath + "GameObjects\\Armor\\";
                    break;
                case ItemType.Consumable:
                    directory = ContentDirectoryHelper.contentDirectoryPath + "GameObjects\\Consumable\\";
                    break;
                case ItemType.Supplies:
                    directory = ContentDirectoryHelper.contentDirectoryPath + "GameObjects\\Supplies\\";
                    break;
                case ItemType.Ammo:
                    directory = ContentDirectoryHelper.contentDirectoryPath + "GameObjects\\Ammo\\";
                    break;
                case ItemType.Misc:
                    directory = ContentDirectoryHelper.contentDirectoryPath + "GameObjects\\Misc\\";
                    break;
            }
            return directory;
        }

        protected override ItemTemplateData FillDataForSave()
        {
            var directory = GetItemDirectory();
            var currentItemType = itemType;
            var newItemPath = directory + id + ".nrif";
            if (File.Exists(newItemPath))
            {
                TextReader reader = null;
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ItemTemplateData));
                    reader = new StreamReader(newItemPath);
                    var oldData = (ItemTemplateData)serializer.Deserialize(reader);
                    id = oldData.Id;
                    reader.Close();
                }
                catch (Exception ex)
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                    id = "";
                }
            }

            ItemTemplateData data = new ItemTemplateData();
            if (id == "" || id == null)
            {
                id = Guid.NewGuid().ToString();
            }

            data.Id = Id;
            data.Name = Name;
            data.Description = Description;
            data.Price = Price;
            data.ItemType = currentItemType;      

            if (currentItemType == ItemType.Weapon)
            {
                data.WeaponTemplateData = WeaponTemplateData;
                data.PossibleSlots = new List<Slot>() { Slot.LefHand, Slot.RightHand };
            }
            else if (currentItemType == ItemType.Armor)
            {
                data.ArmorTemplateData = ArmorTemplateData;
                data.PossibleSlots = new List<Slot>() { this.PossibleSlots.FirstOrDefault() };
            }
            else if (currentItemType == ItemType.Consumable)
            {
                var citd = consumableItemTemplateData;
                data.ConsumableItemTemplateData = citd;
            }

            var iconFileName = Path.GetFileName(iconPath);
            if (!Directory.Exists(directory + "Icons\\"))
            {
                Directory.CreateDirectory(directory + "Icons\\");
            }
            //move icon to local directory
            var newIconLocation = directory + "Icons\\" + iconFileName;

            if (Path.IsPathFullyQualified(iconPath))
            {
                if (iconPath != newIconLocation)
                {
                    File.Copy(iconPath, newIconLocation, true);
                }
            }
            data.IconPath = Path.GetRelativePath(directory, directory + "Icons\\" + iconFileName);
            IconPath = newIconLocation;

            data.AssociatedBuffs = AssociatedBuffs.ToList();
            return data;
        }

        protected override void FillDataFromSave(ItemTemplateData data)
        {
            var directory = GetItemDirectory();

            Id = data.Id;
            Name = data.Name;
            Description = data.Description;
            Price = data.Price;
            ItemType = data.ItemType;
            ItemQuality = data.ItemQuality;

            PossibleSlots.Clear();
            if (data.PossibleSlots != null)
            {
                foreach (var slot in data.PossibleSlots)
                {
                    PossibleSlots.Add(slot);
                }
            }
          
            WeaponTemplateData = data.WeaponTemplateData != null
                ? CloneWeaponTemplateData(data.WeaponTemplateData)
                : new WeaponTemplateData();

            ArmorTemplateData = data.ArmorTemplateData != null
                ? CloneArmorTemplateData(data.ArmorTemplateData)
                : new ArmorTemplateData();

            ConsumableItemTemplateData = data.ConsumableItemTemplateData != null
                ? CloneConsumableItemTemplateData(data.ConsumableItemTemplateData)
                : new ConsumableItemTemplateData();
   
            AssociatedBuffs.Clear();
            if (data.AssociatedBuffs != null)
            {
                foreach (var buff in data.AssociatedBuffs)
                {
                    AssociatedBuffs.Add(buff);
                }
            }

            if (!string.IsNullOrEmpty(data.IconPath))
            {
                var resolved = data.IconPath;
                try
                {
                    if (!Path.IsPathFullyQualified(resolved))
                    {
                        var baseDir = directory;
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

        [RelayCommand]
        public void AddSlot(Slot slot)
        {
            if (!PossibleSlots.Contains(slot))
            {
                PossibleSlots.Add(slot);
            }
        }
        [RelayCommand]
        public void RemoveSlot(Slot slot)
        {
            PossibleSlots.Remove(slot);
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
                AssociatedBuffs.Add(new FileReference { Id = buffId, Path = relativePath });

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
    }
}
