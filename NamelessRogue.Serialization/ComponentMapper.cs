using FlatSharp;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Environment.Spawners;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Serialization;
using NamelessRogue.Serialization.Schemas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NamelessRogue.Serialization;

public static class ComponentMapper
{
    private static readonly Dictionary<Type, Type> _componentToTable = new();
    private static readonly Dictionary<string, Type> _tableNameToType = new();
    private static bool _initialized;
    private static readonly object _lock = new();

    private static void EnsureInitialized()
    {
        if (!_initialized)
        {
            lock (_lock)
            {
                if (!_initialized)
                {
                    Register<CharacterStats, Schemas.CharacterStatsCollection>();
                    Register<AssociatedBuffs, Schemas.AssociatedBuffsCollection>();
                    Register<ItemsHolder, Schemas.ItemsHolderCollection>();
                    Register<EquipmentSlots, Schemas.EquipmentSlotsCollection>();
                    Register<Equipment, Schemas.EquipmentCollection>();
                    Register<QuestHolder, Schemas.QuestHolderCollection>();
                    Register<Group, Schemas.GroupCollection>();
                    Register<GroupTag, Schemas.GroupTagCollection>();
                    Register<ModifiersCollection, Schemas.ModifiersCollectionTable>();
                    Register<TimedModifier, Schemas.TimedModifierTable>();
                    Register<ModifierComponent, Schemas.ModifierComponentTable>();
                    Register<Building, Schemas.BuildingCollection>();
                    Register<DroppedItemsComponent, Schemas.DroppedItemsComponentTable>();
                    Register<Character, Schemas.CharacterCollection>();
                    _initialized = true;
                }
            }
        }
    }

    private static void Register<TComponent, TTable>() where TComponent : Component
    {
        _componentToTable[typeof(TComponent)] = typeof(TTable);
        _tableNameToType[typeof(TTable).Name] = typeof(TComponent);
    }

    private static void SetTableField(Type tableType, object instance, string fieldName, object value, SerializationContext context)
    {
        var field = tableType.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
        if (field != null) field.SetValue(instance, value);
    }

    private static int GetMaxSize(Type tableType, object instance)
    {
        var serializerProp = tableType.GetProperty("Serializer", BindingFlags.Public | BindingFlags.Static);
        if (serializerProp?.GetValue(null) is FlatSharp.ISerializer serializer)
            return serializer.GetMaxSize(instance);
        return 0;
    }

    private static int WriteToBuffer(Type tableType, byte[] buffer, object instance)
    {
        var serializerProp = tableType.GetProperty("Serializer", BindingFlags.Public | BindingFlags.Static);
        if (serializerProp?.GetValue(null) is FlatSharp.ISerializer serializer)
            return serializer.Write(buffer, instance);
        return 0;
    }

    private static object ParseTable(Type tableType, byte[] data)
    {
        var serializerProp = tableType.GetProperty("Serializer", BindingFlags.Public | BindingFlags.Static);
        if (serializerProp?.GetValue(null) is FlatSharp.ISerializer serializer)
            return serializer.Parse(data);
        return null;
    }

    private static void MapTableToComponent(object table, Component component)
    {
        var props = component.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in props)
        {
            var field = table.GetType().GetField(prop.Name, BindingFlags.Public | BindingFlags.Instance);
            if (field != null) prop.SetValue(component, field.GetValue(table));
        }
    }

    private static Entity ResolveEntityReference(Schemas.EntityRef ref2, SerializationContext context)
    {
        if (ref2.EntityIndex == 0) return null;
        var index = (int)ref2.EntityIndex;
        if (index < 0 || index >= EntityInfrastructureManager.Entities.Count) return null;
        return EntityInfrastructureManager.Entities[index];
    }

    private static Entity GetEntityByItemId(Guid parentId, SerializationContext context)
    {
        foreach (var entity in EntityInfrastructureManager.Entities)
        {
            if (entity?.Id == parentId) return entity;
        }
        return null;
    }

    public static byte[] SerializeComponent(Component component, SerializationContext context)
    {
        EnsureInitialized();
        var componentType = component.GetType();
        if (!_componentToTable.TryGetValue(componentType, out var tableType))
            return null;
        var instance = Activator.CreateInstance(tableType);

        if (component is NamelessRogue.Engine.Components.Stats.CharacterStats cs) return SerializeCharacterStats(cs, instance, context);
        if (component is AssociatedBuffs ab) return SerializeAssociatedBuffs(ab, instance);
        if (component is ItemsHolder ih) return SerializeItemsHolder(ih, instance, context);
        if (component is EquipmentSlots es) return SerializeEquipmentSlots(es, instance, context);
        if (component is Equipment eq) return SerializeEquipment(eq, instance);
        if (component is QuestHolder qh) return SerializeQuestHolder(qh, instance, context);
        if (component is Group g) return SerializeGroup(g, instance, context);
        if (component is GroupTag gt) return SerializeGroupTag(gt, instance);
        if (component is ModifiersCollection mc) return SerializeModifiersCollection(mc, instance, context);
        if (component is TimedModifier tm) return SerializeTimedModifier(tm, instance);
        if (component is ModifierComponent mdc) return SerializeModifierComponent(mdc, instance);
        if (component is Building b) return SerializeBuilding(b, instance, context);
        if (component is DroppedItemsComponent dic) return SerializeDroppedItemsComponent(dic, instance);
        if (component is Character c) return SerializeCharacter(c, instance);

        var props = componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in props)
        {
            var value = prop.GetValue(component);
            SetTableField(tableType, instance, prop.Name, value, context);
        }

        var maxSize = GetMaxSize(tableType, instance);
        if (maxSize <= 0) return Array.Empty<byte>();
        var buffer = new byte[maxSize];
        var bytesWritten = WriteToBuffer(tableType, buffer, instance);
        return bytesWritten > 0 ? buffer.Take(bytesWritten).ToArray() : Array.Empty<byte>();
    }

    public static Component DeserializeComponent(byte[] data, Type componentType, SerializationContext context)
    {
        EnsureInitialized();
        if (data == null || data.Length == 0) return null;
        if (!_componentToTable.TryGetValue(componentType, out var tableType)) return null;
        var table = ParseTable(tableType, data);
        if (table == null) return null;
        var instance = (Component)Activator.CreateInstance(componentType);

        if (componentType == typeof(NamelessRogue.Engine.Components.Stats.CharacterStats)) MapCharacterStats(table, instance, context);
        else if (componentType == typeof(AssociatedBuffs)) MapAssociatedBuffs(table, instance);
        else if (componentType == typeof(ItemsHolder)) MapItemsHolder(table, instance, context);
        else if (componentType == typeof(EquipmentSlots)) MapEquipmentSlots(table, instance, context);
        else if (componentType == typeof(Equipment)) MapEquipment(table, instance);
        else if (componentType == typeof(QuestHolder)) MapQuestHolder(table, instance, context);
        else if (componentType == typeof(Group)) MapGroup(table, instance, context);
        else if (componentType == typeof(GroupTag)) MapGroupTag(table, instance);
        else if (componentType == typeof(ModifiersCollection)) MapModifiersCollection(table, instance, context);
        else if (componentType == typeof(TimedModifier)) MapTimedModifier(table, instance);
        else if (componentType == typeof(ModifierComponent)) MapModifierComponent(table, instance);
        else if (componentType == typeof(Building)) MapBuilding(table, instance, context);
        else if (componentType == typeof(DroppedItemsComponent)) MapDroppedItemsComponent(table, instance);
        else if (componentType == typeof(Character)) MapCharacter(table, instance);
        else MapTableToComponent(table, instance);

        return instance;
    }


    private static byte[] SerializeCharacterStats(NamelessRogue.Engine.Components.Stats.CharacterStats component, object instance, SerializationContext context)
    {
        var table = (Schemas.CharacterStatsCollection)instance;
        table.Health = new Schemas.SimpleStat { Value = component.Health.Value, MinValue = component.Health.MinValue, MaxValue = component.Health.MaxValue };
        table.Energy = new Schemas.SimpleStat { Value = component.Energy.Value, MinValue = component.Energy.MinValue, MaxValue = component.Energy.MaxValue };
        table.Speed = new Schemas.SimpleStat { Value = component.Speed.Value, MinValue = component.Speed.MinValue, MaxValue = component.Speed.MaxValue };
        table.MovementSpeed = new Schemas.SimpleStat { Value = component.MovementSpeed.Value, MinValue = component.MovementSpeed.MinValue, MaxValue = component.MovementSpeed.MaxValue };
        table.VisionRange = new Schemas.SimpleStat { Value = component.VisionRange.Value, MinValue = component.VisionRange.MinValue, MaxValue = component.VisionRange.MaxValue };
        table.Weight = new Schemas.SimpleStat { Value = component.Weight.Value, MinValue = component.Weight.MinValue, MaxValue = component.Weight.MaxValue };
        table.NeuralLoad = new Schemas.SimpleStat { Value = component.NeuralLoad.Value, MinValue = component.NeuralLoad.MinValue, MaxValue = component.NeuralLoad.MaxValue };
        table.Money = component.Money;
        table.Immobile = component.Immobile;
        table.FactionId = component.FactionId ?? string.Empty;

        foreach (var ws in component.WeaponStats)
            table.WeaponStatsList.Add(new Schemas.WeaponStats { MinimumDamage = ws.MinimumDamage, MaximumDamage = ws.MaximumDamage, Range = ws.Range, AttackType = (Schemas.AttackType)ws.AttackType, DamageType = (Schemas.DamageType)ws.DamageType, AmmoType = (Schemas.AmmoType)ws.AmmoType, AmmoInClip = ws.AmmoInClip, CurrentAmmo = ws.CurrentAmmo });
        foreach (var rs in component.Resistances)
            table.ResistancesList.Add(new Schemas.ResistanceStat { DamageType = (Schemas.DamageType)rs.DamageType, Value = new Schemas.SimpleStat { Value = rs.Value.Value, MinValue = rs.Value.MinValue, MaxValue = rs.Value.MaxValue } });
        foreach (var as2 in component.Armor)
            table.ArmorList.Add(new Schemas.ArmorStats { DamageType = (Schemas.DamageType)as2.DamageType, Value = new Schemas.SimpleStat { Value = as2.Value.Value, MinValue = as2.Value.MinValue, MaxValue = as2.Value.MaxValue } });

        var maxSize = Schemas.CharacterStatsCollection.Serializer.GetMaxSize(table);
        var buffer = new byte[maxSize];
        var bytesWritten = Schemas.CharacterStatsCollection.Serializer.Write(buffer, table);
        return buffer.Take(bytesWritten).ToArray();
    }

    private static byte[] SerializeAssociatedBuffs(AssociatedBuffs component, object instance)
    {
        var table = (Schemas.AssociatedBuffsCollection)instance;
        foreach (var id in component.BuffIds) table.BuffIds.Add(id);
        var maxSize = Schemas.AssociatedBuffsCollection.Serializer.GetMaxSize(table);
        var buffer = new byte[maxSize];
        var bytesWritten = Schemas.AssociatedBuffsCollection.Serializer.Write(buffer, table);
        return buffer.Take(bytesWritten).ToArray();
    }

    private static byte[] SerializeItemsHolder(ItemsHolder component, object instance, SerializationContext context)
    {
        var table = (Schemas.ItemsHolderCollection)instance;
        foreach (var entity in component.Items)
            table.ItemRefs.Add(new Schemas.EntityRef { EntityIndex = (ulong)entity.Index, ComponentTypeHash = 0 });
        var maxSize = Schemas.ItemsHolderCollection.Serializer.GetMaxSize(table);
        var buffer = new byte[maxSize];
        var bytesWritten = Schemas.ItemsHolderCollection.Serializer.Write(buffer, table);
        return buffer.Take(bytesWritten).ToArray();
    }

    private static byte[] SerializeEquipmentSlots(EquipmentSlots component, object instance, SerializationContext context)
    {
        var table = (Schemas.EquipmentSlotsCollection)instance;
        foreach (var tuple in component.Slots)
        {
            var slot = new Schemas.EquipmentSlot { SlotType = (Schemas.Slot)tuple.Item1 };
            if (tuple.Item2?.Equipment != null)
            {
                var eqEntity = GetEntityByItemId(tuple.Item2.Equipment.ParentEntityId, context);
                if (eqEntity != null)
                    slot.EquipmentEntityRef = new Schemas.EntityRef { EntityIndex = (ulong)eqEntity.Index, ComponentTypeHash = 0 };
            }
            table.Slots.Add(slot);
        }
        table.HolderRef = new Schemas.EntityRef { EntityIndex = 0, ComponentTypeHash = 0 };
        var maxSize = Schemas.EquipmentSlotsCollection.Serializer.GetMaxSize(table);
        var buffer = new byte[maxSize];
        var bytesWritten = Schemas.EquipmentSlotsCollection.Serializer.Write(buffer, table);
        return buffer.Take(bytesWritten).ToArray();
    }

    private static byte[] SerializeEquipment(Equipment component, object instance)
    {
        var table = (Schemas.EquipmentCollection)instance;
        foreach (var slot in component.PossibleSlots) table.PossibleSlots.Add((Schemas.Slot)slot);
        var maxSize = Schemas.EquipmentCollection.Serializer.GetMaxSize(table);
        var buffer = new byte[maxSize];
        var bytesWritten = Schemas.EquipmentCollection.Serializer.Write(buffer, table);
        return buffer.Take(bytesWritten).ToArray();
    }

    private static byte[] SerializeQuestHolder(QuestHolder component, object instance, SerializationContext context)
    {
        var table = (Schemas.QuestHolderCollection)instance;
        if (component.Quests == null)
        {
            var questHolderMaxSize = Schemas.QuestHolderCollection.Serializer.GetMaxSize(table);
            var questHolderBuffer = new byte[questHolderMaxSize];
            var questHolderBytesWritten = Schemas.QuestHolderCollection.Serializer.Write(questHolderBuffer, table);
            return questHolderBuffer.Take(questHolderBytesWritten).ToArray();
        }
        foreach (var quest in component.Quests)
        {
            var q = new Schemas.Quest { Status = (Schemas.QuestStatus)quest.Status };
            if (quest.Milestones != null) foreach (var m in quest.Milestones) q.Milestones.Add(m);
            if (quest.RooNode != null) q.RootNode = SerializeQuestNode(quest.RooNode, context);
            if (quest.CurrentNode != null) q.CurrentNode = SerializeQuestNode(quest.CurrentNode, context);
            table.Quests.Add(q);
        }
        var maxSize = Schemas.QuestHolderCollection.Serializer.GetMaxSize(table);
        var buffer = new byte[maxSize];
        var bytesWritten = Schemas.QuestHolderCollection.Serializer.Write(buffer, table);
        return buffer.Take(bytesWritten).ToArray();
    }

    private static Schemas.QuestNode SerializeQuestNode(Engine.Components.Interaction.QuestNode node, SerializationContext context)
    {
        var qn = new Schemas.QuestNode { Id = node.Id ?? string.Empty, Name = node.Name ?? string.Empty, DescriptionText = node.Description ?? string.Empty, ProgressCondition = (Schemas.QuestType)node.ProgressCondition };
        if (node.ChildNodes != null) foreach (var child in node.ChildNodes) qn.ChildNodes.Add(SerializeQuestNode(child, context));
        if (node.Items != null) foreach (var entity in node.Items) qn.Items.Add(new Schemas.EntityRef { EntityIndex = (ulong)entity.Index, ComponentTypeHash = 0 });
        if (node.NPCs != null) foreach (var entity in node.NPCs) qn.NPCs.Add(new Schemas.EntityRef { EntityIndex = (ulong)entity.Index, ComponentTypeHash = 0 });
        if (node.Locations != null) foreach (var entity in node.Locations) qn.Locations.Add(new Schemas.EntityRef { EntityIndex = (ulong)entity.Index, ComponentTypeHash = 0 });
        return qn;
    }

    private static byte[] SerializeGroup(Group component, object instance, SerializationContext context)
    {
        var table = (Schemas.GroupCollection)instance;
        table.TextId = component.TextId ?? string.Empty;
        table.FormationMaintained = component.FormationMaintained;
        foreach (var entity in component.EntitiesInGroup)
            table.EntitiesInGroup.Add(new Schemas.EntityRef { EntityIndex = (ulong)entity.Index, ComponentTypeHash = 0 });
        if (component.FlagbearerId != null)
            table.FlagbearerId = new Schemas.EntityRef { EntityIndex = (ulong)component.FlagbearerId.Index, ComponentTypeHash = 0 };
        var maxSize = Schemas.GroupCollection.Serializer.GetMaxSize(table);
        var buffer = new byte[maxSize];
        var bytesWritten = Schemas.GroupCollection.Serializer.Write(buffer, table);
        return buffer.Take(bytesWritten).ToArray();
    }

    private static byte[] SerializeGroupTag(GroupTag component, object instance)
    {
        var table = (Schemas.GroupTagCollection)instance;
        table.GroupId = component.GroupId ?? string.Empty;
        table.IsInFormation = component.IsInFormation;
        table.FormationPositionDisplacementX = component.FormationPositionDisplacement.X;
        table.FormationPositionDisplacementY = component.FormationPositionDisplacement.Y;
        var maxSize = Schemas.GroupTagCollection.Serializer.GetMaxSize(table);
        var buffer = new byte[maxSize];
        var bytesWritten = Schemas.GroupTagCollection.Serializer.Write(buffer, table);
        return buffer.Take(bytesWritten).ToArray();
    }

    private static byte[] SerializeModifiersCollection(ModifiersCollection component, object instance, SerializationContext context)
    {
        var table = (Schemas.ModifiersCollectionTable)instance;
        if (component.Accumulator != null)
            table.AccumulatorRef = new Schemas.EntityRef { EntityIndex = (ulong)component.Accumulator.Index, ComponentTypeHash = 0 };
        foreach (var entity in component.ModifierEntities)
            table.ModifierEntities.Add(new Schemas.EntityRef { EntityIndex = (ulong)entity.Index, ComponentTypeHash = 0 });
        var maxSize = Schemas.ModifiersCollectionTable.Serializer.GetMaxSize(table);
        var buffer = new byte[maxSize];
        var bytesWritten = Schemas.ModifiersCollectionTable.Serializer.Write(buffer, table);
        return buffer.Take(bytesWritten).ToArray();
    }

    private static byte[] SerializeTimedModifier(TimedModifier component, object instance)
    {
        var table = (Schemas.TimedModifierTable)instance;
        table.TurnsToLast = component.TurnsToLast;
        var maxSize = Schemas.TimedModifierTable.Serializer.GetMaxSize(table);
        var buffer = new byte[maxSize];
        var bytesWritten = Schemas.TimedModifierTable.Serializer.Write(buffer, table);
        return buffer.Take(bytesWritten).ToArray();
    }

    private static byte[] SerializeModifierComponent(ModifierComponent component, object instance)
    {
        var table = (Schemas.ModifierComponentTable)instance;
        var maxSize = Schemas.ModifierComponentTable.Serializer.GetMaxSize(table);
        var buffer = new byte[maxSize];
        var bytesWritten = Schemas.ModifierComponentTable.Serializer.Write(buffer, table);
        return buffer.Take(bytesWritten).ToArray();
    }

    private static byte[] SerializeBuilding(Building component, object instance, SerializationContext context)
    {
        var table = (Schemas.BuildingCollection)instance;
        var parts = component.getBuildingParts();
        foreach (var entity in parts)
            table.BuildingParts.Add(new Schemas.EntityRef { EntityIndex = (ulong)entity.Index, ComponentTypeHash = 0 });
        var maxSize = Schemas.BuildingCollection.Serializer.GetMaxSize(table);
        var buffer = new byte[maxSize];
        var bytesWritten = Schemas.BuildingCollection.Serializer.Write(buffer, table);
        return buffer.Take(bytesWritten).ToArray();
    }

    private static byte[] SerializeDroppedItemsComponent(DroppedItemsComponent component, object instance)
    {
        var table = (Schemas.DroppedItemsComponentTable)instance;
        foreach (var di in component.DroppedItems)
            table.DroppedItems.Add(new Schemas.DroppedItem { ItemId = di.ItemId ?? string.Empty, Probability = di.Probability });
        var maxSize = Schemas.DroppedItemsComponentTable.Serializer.GetMaxSize(table);
        var buffer = new byte[maxSize];
        var bytesWritten = Schemas.DroppedItemsComponentTable.Serializer.Write(buffer, table);
        return buffer.Take(bytesWritten).ToArray();
    }

    private static byte[] SerializeCharacter(Character component, object instance)
    {
        var table = (Schemas.CharacterCollection)instance;
        table.FactionId = component.FactionId ?? string.Empty;
        if (component.FactionRelations != null)
            foreach (var kvp in component.FactionRelations)
            {
                table.FactionRelationsKeys.Add(kvp.Key);
                table.FactionRelationsValues.Add(kvp.Value);
            }
        var maxSize = Schemas.CharacterCollection.Serializer.GetMaxSize(table);
        var buffer = new byte[maxSize];
        var bytesWritten = Schemas.CharacterCollection.Serializer.Write(buffer, table);
        return buffer.Take(bytesWritten).ToArray();
    }
    private static void MapCharacterStats(object table, Component component, SerializationContext context)
    {
        var t = (Schemas.CharacterStatsCollection)table;
        var cs = (NamelessRogue.Engine.Components.Stats.CharacterStats)component;
        cs.Health = new NamelessRogue.Engine.Components.Stats.SimpleStat(t.Health.Value, t.Health.MinValue, t.Health.MaxValue);
        cs.Energy = new NamelessRogue.Engine.Components.Stats.SimpleStat(t.Energy.Value, t.Energy.MinValue, t.Energy.MaxValue);
        cs.Speed = new NamelessRogue.Engine.Components.Stats.SimpleStat(t.Speed.Value, t.Speed.MinValue, t.Speed.MaxValue);
        cs.MovementSpeed = new NamelessRogue.Engine.Components.Stats.SimpleStat(t.MovementSpeed.Value, t.MovementSpeed.MinValue, t.MovementSpeed.MaxValue);
        cs.VisionRange = new NamelessRogue.Engine.Components.Stats.SimpleStat(t.VisionRange.Value, t.VisionRange.MinValue, t.VisionRange.MaxValue);
        cs.Weight = new NamelessRogue.Engine.Components.Stats.SimpleStat(t.Weight.Value, t.Weight.MinValue, t.Weight.MaxValue);
        cs.NeuralLoad = new NamelessRogue.Engine.Components.Stats.SimpleStat(t.NeuralLoad.Value, t.NeuralLoad.MinValue, t.NeuralLoad.MaxValue);
        cs.Money = t.Money; cs.Immobile = t.Immobile; cs.FactionId = t.FactionId;
        cs.WeaponStats.Clear();
        foreach (var ws in t.WeaponStatsList)
            cs.WeaponStats.Add(new NamelessRogue.Engine.Components.ItemComponents.WeaponStats { MinimumDamage = ws.MinimumDamage, MaximumDamage = ws.MaximumDamage, Range = ws.Range, AttackType = (NamelessRogue.Engine.Components.ItemComponents.AttackType)ws.AttackType, DamageType = (NamelessRogue.Engine.Components.Stats.DamageType)ws.DamageType, AmmoType = (NamelessRogue.Engine.Components.ItemComponents.AmmoType)ws.AmmoType, AmmoInClip = ws.AmmoInClip, CurrentAmmo = ws.CurrentAmmo });
        cs.Resistances.Clear();
        foreach (var rs in t.ResistancesList)
            cs.Resistances.Add(new NamelessRogue.Engine.Components.Stats.ResistanceStat { DamageType = (NamelessRogue.Engine.Components.Stats.DamageType)rs.DamageType, Value = new NamelessRogue.Engine.Components.Stats.SimpleStat(rs.Value.Value, rs.Value.MinValue, rs.Value.MaxValue) });
        cs.Armor.Clear();
        foreach (var as2 in t.ArmorList)
            cs.Armor.Add(new NamelessRogue.Engine.Components.Stats.ArmorStats { DamageType = (NamelessRogue.Engine.Components.Stats.DamageType)as2.DamageType, Value = new NamelessRogue.Engine.Components.Stats.SimpleStat(as2.Value.Value, as2.Value.MinValue, as2.Value.MaxValue) });
    }

    private static void MapAssociatedBuffs(object table, Component component)
    {
        var t = (Schemas.AssociatedBuffsCollection)table;
        var ab = (AssociatedBuffs)component;
        ab.BuffIds.Clear();
        foreach (var id in t.BuffIds) ab.BuffIds.Add(id);
    }

    private static void MapItemsHolder(object table, Component component, SerializationContext context)
    {
        var ih = (ItemsHolder)component;
        ih.Items.Clear();
    }

    private static void MapEquipmentSlots(object table, Component component, SerializationContext context)
    {
        var t = (Schemas.EquipmentSlotsCollection)table;
        var es = (EquipmentSlots)component;
        es.Slots.Clear();
        foreach (var slot in t.Slots)
            es.Slots.Add(new Tuple<NamelessRogue.Engine.Components.ItemComponents.Slot, NamelessRogue.Engine.Components.ItemComponents.EquipmentSlot>((NamelessRogue.Engine.Components.ItemComponents.Slot)slot.SlotType, new NamelessRogue.Engine.Components.ItemComponents.EquipmentSlot((NamelessRogue.Engine.Components.ItemComponents.Slot)slot.SlotType)));
    }

    private static void MapEquipment(object table, Component component)
    {
        var t = (Schemas.EquipmentCollection)table;
        var eq = (Equipment)component;
        eq.PossibleSlots.Clear();
        foreach (var slot in t.PossibleSlots) eq.PossibleSlots.Add((NamelessRogue.Engine.Components.ItemComponents.Slot)slot);
    }

    private static void MapQuestHolder(object table, Component component, SerializationContext context)
    {
        var t = (Schemas.QuestHolderCollection)table;
        var qh = (QuestHolder)component;
        qh.Quests = new List<Engine.Components.Interaction.Quest>();
        foreach (var q in t.Quests)
        {
            var quest = new Engine.Components.Interaction.Quest { Status = (Engine.Components.Interaction.QuestStatus)q.Status, Milestones = new List<string>(q.Milestones), RooNode = DeserializeQuestNode(q.RootNode, context), CurrentNode = DeserializeQuestNode(q.CurrentNode, context) };
            qh.Quests.Add(quest);
        }
    }

    private static Engine.Components.Interaction.QuestNode DeserializeQuestNode(Schemas.QuestNode tableNode, SerializationContext context)
    {
        if (tableNode == null) return null;
        var node = new Engine.Components.Interaction.QuestNode { Id = tableNode.Id, Name = tableNode.Name, Description = tableNode.DescriptionText, ProgressCondition = (Engine.Components.Interaction.QuestType)tableNode.ProgressCondition, ChildNodes = new List<Engine.Components.Interaction.QuestNode>(), Items = new List<Engine.Infrastructure.Entity>(), NPCs = new List<Engine.Infrastructure.Entity>(), Locations = new List<Engine.Infrastructure.Entity>() };
        foreach (var child in tableNode.ChildNodes) node.ChildNodes.Add(DeserializeQuestNode(child, context));
        foreach (var ref2 in tableNode.Items) { var e = ResolveEntityReference(ref2, context); if (e != null) node.Items.Add(e); }
        foreach (var ref2 in tableNode.NPCs) { var e = ResolveEntityReference(ref2, context); if (e != null) node.NPCs.Add(e); }
        foreach (var ref2 in tableNode.Locations) { var e = ResolveEntityReference(ref2, context); if (e != null) node.Locations.Add(e); }
        return node;
    }

    private static void MapGroup(object table, Component component, SerializationContext context)
    {
        var t = (Schemas.GroupCollection)table;
        var g = (Group)component;
        g.TextId = t.TextId; g.FormationMaintained = t.FormationMaintained;
        g.EntitiesInGroup.Clear();
        foreach (var ref2 in t.EntitiesInGroup) { var e = ResolveEntityReference(ref2, context); if (e != null) g.EntitiesInGroup.Add(e); }
        if (t.FlagbearerId.EntityIndex > 0) { var fb = ResolveEntityReference(t.FlagbearerId, context); if (fb != null) { var flagProp = typeof(Group).GetProperty("FlagbearerId", BindingFlags.NonPublic | BindingFlags.Instance); if (flagProp != null) flagProp.SetValue(g, fb); } }
    }

    private static void MapGroupTag(object table, Component component)
    {
        var t = (Schemas.GroupTagCollection)table;
        var gt = (GroupTag)component;
        gt.GroupId = t.GroupId; gt.IsInFormation = t.IsInFormation;
        gt.FormationPositionDisplacement = new Microsoft.Xna.Framework.Point(t.FormationPositionDisplacementX, t.FormationPositionDisplacementY);
    }

    private static void MapModifiersCollection(object table, Component component, SerializationContext context)
    {
        var t = (Schemas.ModifiersCollectionTable)table;
        var mc = (ModifiersCollection)component;
        if (t.AccumulatorRef.EntityIndex > 0) { var a = ResolveEntityReference(t.AccumulatorRef, context); if (a != null) mc.Accumulator = a; }
        mc.ModifierEntities.Clear();
        foreach (var ref2 in t.ModifierEntities) { var e = ResolveEntityReference(ref2, context); if (e != null) mc.ModifierEntities.Add(e); }
    }

    private static void MapTimedModifier(object table, Component component)
    {
        var t = (Schemas.TimedModifierTable)table;
        var tm = (TimedModifier)component;
        tm.TurnsToLast = t.TurnsToLast;
    }

    private static void MapModifierComponent(object table, Component component) { }

    private static void MapBuilding(object table, Component component, SerializationContext context)
    {
        var t = (Schemas.BuildingCollection)table;
        var b = (Building)component;
        var parts = new List<Entity>();
        foreach (var ref2 in t.BuildingParts) { var e = ResolveEntityReference(ref2, context); if (e != null) parts.Add(e); }
        b.setBuildingParts(parts);
    }

    private static void MapDroppedItemsComponent(object table, Component component)
    {
        var t = (Schemas.DroppedItemsComponentTable)table;
        var dic = (DroppedItemsComponent)component;
        var items = t.DroppedItems.Select(di => new NamelessRogue.Engine.Components.Interaction.DroppedItem { ItemId = di.ItemId, Probability = di.Probability }).ToList();
        var field = typeof(DroppedItemsComponent).GetField("_droppedItems", BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null) field.SetValue(dic, items);
    }

    private static void MapCharacter(object table, Component component)
    {
        var t = (Schemas.CharacterCollection)table;
        var c = (Character)component;
        c.FactionId = t.FactionId;
        c.FactionRelations = new Dictionary<string, int>();
        for (int i = 0; i < t.FactionRelationsKeys.Count; i++)
            c.FactionRelations[t.FactionRelationsKeys[i]] = t.FactionRelationsValues[i];
    }
}
