using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogueDataEditor.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using NamelessRogue.Engine.Components.ItemComponents;
    using NamelessRogue.Engine.Components.Stats;
    using NamelessRogue.Engine.Generation.Editor;
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using System.Windows.Input;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using static NamelessRogue.Engine.Generation.Editor.QuestTemplateData;

    public partial class ItemEditorViewModel : BaseEditorViewModel
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
        private string id;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        private int price;

        [ObservableProperty]
        private string iconPath;

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

        public ObservableCollection<FileReference> AssociatedBuffs { get; } = new ObservableCollection<FileReference>();

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        string contentDirectoryPath;
        private string iconFileName;

        public ItemEditorViewModel(ItemTemplateData item) : base("*.nrif", "")
        {

            string workingDirectory = Environment.CurrentDirectory;
#if DEBUG
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            contentDirectoryPath = projectDirectory + "\\Content";
#else
            contentDirectoryPath = workingDirectory+"\\Content";
#endif

            if (item == null) throw new ArgumentNullException(nameof(item));

            Id = item.Id;
            Name = item.Name;
            Description = item.Description;
            Price = item.Price;
            IconPath = item.IconPath;
            ItemType = item.ItemType;
            ItemQuality = item.ItemQuality;

            PossibleSlots.Clear();
            if (item.PossibleSlots != null)
                foreach (var slot in item.PossibleSlots)
                    PossibleSlots.Add(slot);

            WeaponTemplateData = item.WeaponTemplateData != null ? CloneWeaponTemplateData(item.WeaponTemplateData) : new WeaponTemplateData();
            ArmorTemplateData = item.ArmorTemplateData != null ? CloneArmorTemplateData(item.ArmorTemplateData) : new ArmorTemplateData();
            ConsumableItemTemplateData = item.ConsumableItemTemplateData != null ? CloneConsumableItemTemplateData(item.ConsumableItemTemplateData) : new ConsumableItemTemplateData();

            AssociatedBuffs.Clear();
            if (item.AssociatedBuffs != null)
                foreach (var buff in item.AssociatedBuffs)
                    AssociatedBuffs.Add(buff);

            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        // Implement cloning methods for deep copies to allow editing without mutating original instance
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

        // Example Save logic, replace with your integration
        private void OnSave()
        {
            var directory = "";
            var currentItemType = itemType;
            switch (itemType)
            {
                case ItemType.Weapon:
                    directory = contentDirectoryPath + "\\GameObjects\\Weapons\\";
                    break;
                case ItemType.Armor:
                    directory = contentDirectoryPath + "\\GameObjects\\Armor\\";
                    break;
                case ItemType.Consumable:
                    directory = contentDirectoryPath + "\\GameObjects\\Consumable\\";
                    break;
                case ItemType.Supplies:
                    directory = contentDirectoryPath + "\\GameObjects\\Supplies\\";
                    break;
                case ItemType.Ammo:
                    directory = contentDirectoryPath + "\\GameObjects\\Ammo\\";
                    break;
                case ItemType.Misc:
                    directory = contentDirectoryPath + "\\GameObjects\\Misc\\";
                    break;
            }


            var newItemPath = directory + name + ".nrif";
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

            if (IconPath != string.Empty)
            {
                if (!Directory.Exists(directory + "\\Icons\\"))
                {
                    Directory.CreateDirectory(directory + "\\Icons\\");
                }
                // File.Delete(iconPath);

                var newIconLocation = directory + "Icons\\" + iconFileName;
                //if (IconPath != newIconLocation)
                //{
                //    File.Copy(IconPath, newIconLocation, true);
                //}
                data.IconPath = Path.GetRelativePath(directory, directory + "\\Icons\\" + iconFileName);
            }

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

            data.AssociatedBuffs = AssociatedBuffs.ToList();

            using (TextWriter writer = new StreamWriter(newItemPath))
            {
                XmlSerializer ser = new XmlSerializer(typeof(ItemTemplateData));
                ser.Serialize(writer, data);
            }
        }

        private void OnCancel()
        {
            // TODO: Handle cancel, e.g. close window or discard changes
        }
    }
}
