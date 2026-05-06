using CommunityToolkit.Mvvm.ComponentModel;
using NamelessRogue.Engine.Generation.Editor;
using System.Collections.ObjectModel;

namespace NamelessRogueDataEditor.ViewModels
{

    public partial class CharacterViewModel : BaseEditorViewModel<CharacterTemplateData>
    {
        public CharacterViewModel() : base("*.nrcf", "Characters")
        {
            // Initialize collections to avoid nulls in bindings
            DroppedItems = new ObservableCollection<DroppedItemTemplate>();
            VendorItems = new ObservableCollection<DroppedItemTemplate>();
        }

        [ObservableProperty]
        private string id = string.Empty;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private string spritePath = string.Empty;

        [ObservableProperty]
        private bool castsShadow;

        [ObservableProperty]
        private bool immobile;

        [ObservableProperty]
        private int health;

        [ObservableProperty]
        private int energy;

        [ObservableProperty]
        private int movementSpeed;

        [ObservableProperty]
        private int visionRange;

        [ObservableProperty]
        private string factionId = string.Empty;

        [ObservableProperty]
        private ObservableCollection<DroppedItemTemplate> droppedItems;

        [ObservableProperty]
        private ObservableCollection<DroppedItemTemplate> vendorItems;

        [ObservableProperty]
        private WeaponTemplateData weaponTemplateData;

        [ObservableProperty]
        private ArmorTemplateData armorTemplateData;

        [ObservableProperty]
        private bool isFlying;

        [ObservableProperty]
        private string dialogDataId = string.Empty;

        [ObservableProperty]
        private string dialogFilePath = string.Empty;

        [ObservableProperty]
        private bool randomName;

        [ObservableProperty]
        private bool randomVendorAvailableItems;

        [ObservableProperty]
        private bool randomDroppedItems;

        protected override CharacterTemplateData FillDataForSave()
        {
            return new CharacterTemplateData
            {
                Id = Id ?? string.Empty,
                Name = Name ?? string.Empty,
                Description = Description ?? string.Empty,
                SpritePath = SpritePath ?? string.Empty,
                CastsShadow = CastsShadow,
                Immobile = Immobile,
                Health = Health,
                Energy = Energy,
                MovementSpeed = MovementSpeed,
                VisionRange = VisionRange,
                FactionId = FactionId ?? string.Empty,
                DroppedItems = DroppedItems != null ? new List<DroppedItemTemplate>(DroppedItems) : new List<DroppedItemTemplate>(),
                VendorItems = VendorItems != null ? new List<DroppedItemTemplate>(VendorItems) : new List<DroppedItemTemplate>(),
                WeaponTemplateData = WeaponTemplateData,
                ArmorTemplateData = ArmorTemplateData,
                IsFlying = IsFlying,
                DialogDataId = DialogDataId ?? string.Empty,
                DialogFilePath = DialogFilePath ?? string.Empty,
                RandomName = RandomName,
                RandomVendorAvailableItems = RandomVendorAvailableItems,
                RandomDroppedItems = RandomDroppedItems
            };
        }

        protected override void FillDataFromSave(CharacterTemplateData data)
        {
            Id = data.Id ?? string.Empty;
            Name = data.Name ?? string.Empty;
            Description = data.Description ?? string.Empty;
            SpritePath = data.SpritePath ?? string.Empty;
            CastsShadow = data.CastsShadow;
            Immobile = data.Immobile;
            Health = data.Health;
            Energy = data.Energy;
            MovementSpeed = data.MovementSpeed;
            VisionRange = data.VisionRange;
            FactionId = data.FactionId ?? string.Empty;
            DroppedItems = data.DroppedItems != null
                ? new ObservableCollection<DroppedItemTemplate>(data.DroppedItems)
                : new ObservableCollection<DroppedItemTemplate>();
            VendorItems = data.VendorItems != null
                ? new ObservableCollection<DroppedItemTemplate>(data.VendorItems)
                : new ObservableCollection<DroppedItemTemplate>();
            WeaponTemplateData = data.WeaponTemplateData;
            ArmorTemplateData = data.ArmorTemplateData;
            IsFlying = data.IsFlying;
            DialogDataId = data.DialogDataId ?? string.Empty;
            DialogFilePath = data.DialogFilePath ?? string.Empty;
            RandomName = data.RandomName;
            RandomVendorAvailableItems = data.RandomVendorAvailableItems;
            RandomDroppedItems = data.RandomDroppedItems;
        }
    }
}
