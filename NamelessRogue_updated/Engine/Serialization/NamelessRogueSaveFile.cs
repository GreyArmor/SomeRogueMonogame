
// AUTOGENERATED
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatSharp.Attributes;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using NamelessRogue.Engine.Serialization.CustomSerializationClasses;
namespace NamelessRogue.Engine.Serialization.AutogeneratedSerializationClasses
{
        [FlatBufferTable]
        public class NamelessRogueSaveFile
        {        
            public Dictionary<Type, dynamic> StoragesDictionary = new Dictionary<Type, dynamic>();
            public Dictionary<Type, Type> ComponentTypeToStorge = new Dictionary<Type, Type>();
            [FlatBufferItem(0)]  public IList<EntityStorage> EntityStorageTable { get; set; } = new List<EntityStorage>();

            [FlatBufferItem(1)]  public IList<ActionPointsStorage> ActionPointsStorageTable { get; set; } = new List<ActionPointsStorage>();

            [FlatBufferItem(2)]  public IList<AIControlledStorage> AIControlledStorageTable { get; set; } = new List<AIControlledStorage>();

            [FlatBufferItem(3)]  public IList<AmmoLibraryStorage> AmmoLibraryStorageTable { get; set; } = new List<AmmoLibraryStorage>();

            [FlatBufferItem(4)]  public IList<AmmoStorage> AmmoStorageTable { get; set; } = new List<AmmoStorage>();

            [FlatBufferItem(5)]  public IList<ArmorStatsStorage> ArmorStatsStorageTable { get; set; } = new List<ArmorStatsStorage>();

            [FlatBufferItem(6)]  public IList<BasicAiStorage> BasicAiStorageTable { get; set; } = new List<BasicAiStorage>();

            [FlatBufferItem(7)]  public IList<BlocksVisionStorage> BlocksVisionStorageTable { get; set; } = new List<BlocksVisionStorage>();

            [FlatBufferItem(8)]  public IList<BuffsStorage> BuffsStorageTable { get; set; } = new List<BuffsStorage>();

            [FlatBufferItem(9)]  public IList<BuildingStorage> BuildingStorageTable { get; set; } = new List<BuildingStorage>();

            [FlatBufferItem(10)]  public IList<CharacterStorage> CharacterStorageTable { get; set; } = new List<CharacterStorage>();

            [FlatBufferItem(11)]  public IList<CommanderStorage> CommanderStorageTable { get; set; } = new List<CommanderStorage>();

            [FlatBufferItem(12)]  public IList<ConsoleCameraStorage> ConsoleCameraStorageTable { get; set; } = new List<ConsoleCameraStorage>();

            [FlatBufferItem(13)]  public IList<ConsumableStorage> ConsumableStorageTable { get; set; } = new List<ConsumableStorage>();

            [FlatBufferItem(14)]  public IList<CursorStorage> CursorStorageTable { get; set; } = new List<CursorStorage>();

            [FlatBufferItem(15)]  public IList<DamageStorage> DamageStorageTable { get; set; } = new List<DamageStorage>();

            [FlatBufferItem(16)]  public IList<DeadStorage> DeadStorageTable { get; set; } = new List<DeadStorage>();

            [FlatBufferItem(17)]  public IList<DescriptionStorage> DescriptionStorageTable { get; set; } = new List<DescriptionStorage>();

            [FlatBufferItem(18)]  public IList<DoorStorage> DoorStorageTable { get; set; } = new List<DoorStorage>();

            [FlatBufferItem(19)]  public IList<DrawableStorage> DrawableStorageTable { get; set; } = new List<DrawableStorage>();

            [FlatBufferItem(20)]  public IList<EquipmentSlotsStorage> EquipmentSlotsStorageTable { get; set; } = new List<EquipmentSlotsStorage>();

            [FlatBufferItem(21)]  public IList<EquipmentStorage> EquipmentStorageTable { get; set; } = new List<EquipmentStorage>();

            [FlatBufferItem(22)]  public IList<FireStorage> FireStorageTable { get; set; } = new List<FireStorage>();

            [FlatBufferItem(23)]  public IList<FollowedByCameraStorage> FollowedByCameraStorageTable { get; set; } = new List<FollowedByCameraStorage>();

            [FlatBufferItem(24)]  public IList<FurnitureStorage> FurnitureStorageTable { get; set; } = new List<FurnitureStorage>();

            [FlatBufferItem(25)]  public IList<InputComponentStorage> InputComponentStorageTable { get; set; } = new List<InputComponentStorage>();

            [FlatBufferItem(26)]  public IList<InputReceiverStorage> InputReceiverStorageTable { get; set; } = new List<InputReceiverStorage>();

            [FlatBufferItem(27)]  public IList<ItemsHolderStorage> ItemsHolderStorageTable { get; set; } = new List<ItemsHolderStorage>();

            [FlatBufferItem(28)]  public IList<ItemStorage> ItemStorageTable { get; set; } = new List<ItemStorage>();

            [FlatBufferItem(29)]  public IList<LineToPlayerStorage> LineToPlayerStorageTable { get; set; } = new List<LineToPlayerStorage>();

            [FlatBufferItem(30)]  public IList<MovableStorage> MovableStorageTable { get; set; } = new List<MovableStorage>();

            [FlatBufferItem(31)]  public IList<OccupiesTileStorage> OccupiesTileStorageTable { get; set; } = new List<OccupiesTileStorage>();

            [FlatBufferItem(32)]  public IList<PlayerStorage> PlayerStorageTable { get; set; } = new List<PlayerStorage>();

            [FlatBufferItem(33)]  public IList<PositionStorage> PositionStorageTable { get; set; } = new List<PositionStorage>();

            [FlatBufferItem(34)]  public IList<ScreenStorage> ScreenStorageTable { get; set; } = new List<ScreenStorage>();

            [FlatBufferItem(35)]  public IList<SimpleSwitchStorage> SimpleSwitchStorageTable { get; set; } = new List<SimpleSwitchStorage>();

            [FlatBufferItem(36)]  public IList<StatsStorage> StatsStorageTable { get; set; } = new List<StatsStorage>();

            [FlatBufferItem(37)]  public IList<UnitStorage> UnitStorageTable { get; set; } = new List<UnitStorage>();

            [FlatBufferItem(38)]  public IList<WeaponStatsStorage> WeaponStatsStorageTable { get; set; } = new List<WeaponStatsStorage>();

            [FlatBufferItem(39)]  public IList<WorldBoardPlayerStorage> WorldBoardPlayerStorageTable { get; set; } = new List<WorldBoardPlayerStorage>();
		public NamelessRogueSaveFile()
		{
			FillInfrastructureCollections();
		}

		public void FillInfrastructureCollections()
		{
			StoragesDictionary.Add(typeof(EntityStorage), EntityStorageTable);
			StoragesDictionary.Add(typeof(ActionPointsStorage), ActionPointsStorageTable);
			StoragesDictionary.Add(typeof(AIControlledStorage), AIControlledStorageTable);
			StoragesDictionary.Add(typeof(AmmoLibraryStorage), AmmoLibraryStorageTable);
			StoragesDictionary.Add(typeof(AmmoStorage), AmmoStorageTable);
			StoragesDictionary.Add(typeof(ArmorStatsStorage), ArmorStatsStorageTable);
			StoragesDictionary.Add(typeof(BasicAiStorage), BasicAiStorageTable);
			StoragesDictionary.Add(typeof(BlocksVisionStorage), BlocksVisionStorageTable);
			StoragesDictionary.Add(typeof(BuffsStorage), BuffsStorageTable);
			StoragesDictionary.Add(typeof(BuildingStorage), BuildingStorageTable);
			StoragesDictionary.Add(typeof(CharacterStorage), CharacterStorageTable);
			StoragesDictionary.Add(typeof(CommanderStorage), CommanderStorageTable);
			StoragesDictionary.Add(typeof(ConsoleCameraStorage), ConsoleCameraStorageTable);
			StoragesDictionary.Add(typeof(ConsumableStorage), ConsumableStorageTable);
			StoragesDictionary.Add(typeof(CursorStorage), CursorStorageTable);
			StoragesDictionary.Add(typeof(DamageStorage), DamageStorageTable);
			StoragesDictionary.Add(typeof(DeadStorage), DeadStorageTable);
			StoragesDictionary.Add(typeof(DescriptionStorage), DescriptionStorageTable);
			StoragesDictionary.Add(typeof(DoorStorage), DoorStorageTable);
			StoragesDictionary.Add(typeof(DrawableStorage), DrawableStorageTable);
			StoragesDictionary.Add(typeof(EquipmentSlotsStorage), EquipmentSlotsStorageTable);
			StoragesDictionary.Add(typeof(EquipmentStorage), EquipmentStorageTable);
			StoragesDictionary.Add(typeof(FireStorage), FireStorageTable);
			StoragesDictionary.Add(typeof(FollowedByCameraStorage), FollowedByCameraStorageTable);
			StoragesDictionary.Add(typeof(FurnitureStorage), FurnitureStorageTable);
			StoragesDictionary.Add(typeof(InputComponentStorage), InputComponentStorageTable);
			StoragesDictionary.Add(typeof(InputReceiverStorage), InputReceiverStorageTable);
			StoragesDictionary.Add(typeof(ItemsHolderStorage), ItemsHolderStorageTable);
			StoragesDictionary.Add(typeof(ItemStorage), ItemStorageTable);
			StoragesDictionary.Add(typeof(LineToPlayerStorage), LineToPlayerStorageTable);
			StoragesDictionary.Add(typeof(MovableStorage), MovableStorageTable);
			StoragesDictionary.Add(typeof(OccupiesTileStorage), OccupiesTileStorageTable);
			StoragesDictionary.Add(typeof(PlayerStorage), PlayerStorageTable);
			StoragesDictionary.Add(typeof(PositionStorage), PositionStorageTable);
			StoragesDictionary.Add(typeof(ScreenStorage), ScreenStorageTable);
			StoragesDictionary.Add(typeof(SimpleSwitchStorage), SimpleSwitchStorageTable);
			StoragesDictionary.Add(typeof(StatsStorage), StatsStorageTable);
			StoragesDictionary.Add(typeof(UnitStorage), UnitStorageTable);
			StoragesDictionary.Add(typeof(WeaponStatsStorage), WeaponStatsStorageTable);
			StoragesDictionary.Add(typeof(WorldBoardPlayerStorage), WorldBoardPlayerStorageTable);

			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Infrastructure.Entity), typeof(EntityStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Abstraction.IEntity), typeof(EntityStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Interaction.ActionPoints), typeof(ActionPointsStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.AI.NonPlayerCharacter.AIControlled), typeof(AIControlledStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.ItemComponents.AmmoLibrary), typeof(AmmoLibraryStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.ItemComponents.Ammo), typeof(AmmoStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.ItemComponents.ArmorStats), typeof(ArmorStatsStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAi), typeof(BasicAiStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Physical.BlocksVision), typeof(BlocksVisionStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Interaction.Buffs), typeof(BuffsStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Environment.Building), typeof(BuildingStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.AI.NonPlayerCharacter.Character), typeof(CharacterStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Infrastructure.Commander), typeof(CommanderStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Rendering.ConsoleCamera), typeof(ConsoleCameraStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.ItemComponents.Consumable), typeof(ConsumableStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Interaction.Cursor), typeof(CursorStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Status.Damage), typeof(DamageStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Status.Dead), typeof(DeadStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.UI.Description), typeof(DescriptionStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Environment.Door), typeof(DoorStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Rendering.Drawable), typeof(DrawableStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.ItemComponents.EquipmentSlots), typeof(EquipmentSlotsStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.ItemComponents.Equipment), typeof(EquipmentStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Environment.Fire), typeof(FireStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Rendering.FollowedByCamera), typeof(FollowedByCameraStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Environment.Furniture), typeof(FurnitureStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Interaction.InputComponent), typeof(InputComponentStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Interaction.InputReceiver), typeof(InputReceiverStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.ItemComponents.ItemsHolder), typeof(ItemsHolderStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.ItemComponents.Item), typeof(ItemStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Rendering.LineToPlayer), typeof(LineToPlayerStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Interaction.Movable), typeof(MovableStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Physical.OccupiesTile), typeof(OccupiesTileStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Interaction.Player), typeof(PlayerStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Physical.Position), typeof(PositionStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Rendering.Screen), typeof(ScreenStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Interaction.SimpleSwitch), typeof(SimpleSwitchStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.Stats.Stats), typeof(StatsStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.WorldBoardComponents.Combat.Unit), typeof(UnitStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.ItemComponents.WeaponStats), typeof(WeaponStatsStorage));
			ComponentTypeToStorge.Add(typeof(NamelessRogue.Engine.Components.WorldBoardComponents.WorldBoardPlayer), typeof(WorldBoardPlayerStorage));
		}
	}
}