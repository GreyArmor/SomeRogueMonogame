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
    using System.Windows.Input;
    using System.Xml.Linq;
    using static NamelessRogue.Engine.Generation.Editor.QuestTemplateData;

    public partial class ItemEditorViewModel : ObservableObject
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
        public ItemEditorViewModel(ItemTemplateData item)
        {
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

            WeaponTemplateData = item.WeaponTemplateData != null ? CloneWeaponTemplateData(item.WeaponTemplateData) : null;
            ArmorTemplateData = item.ArmorTemplateData != null ? CloneArmorTemplateData(item.ArmorTemplateData) : null;
            ConsumableItemTemplateData = item.ConsumableItemTemplateData != null ? CloneConsumableItemTemplateData(item.ConsumableItemTemplateData) : null;

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
            // TODO: Map back to model or raise event to notify saving
            // Example: Update ItemTemplateData instance or notify observer
        }

        private void OnCancel()
        {
            // TODO: Handle cancel, e.g. close window or discard changes
        }
    }
}
