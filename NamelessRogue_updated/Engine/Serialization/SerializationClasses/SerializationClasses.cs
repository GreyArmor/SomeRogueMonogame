

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatSharp.Attributes;

namespace NamelessRogue.Engine.Serialization.SerializationClasses
{
    [FlatBufferTable]
    public class Commander
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue_updated.Engine.Infrastructure.Commander component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue_updated.Engine.Infrastructure.Commander component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue_updated.Engine.Infrastructure.Commander (Commander thisType)
        {
            NamelessRogue_updated.Engine.Infrastructure.Commander result = new NamelessRogue_updated.Engine.Infrastructure.Commander();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Commander ( NamelessRogue_updated.Engine.Infrastructure.Commander  component)
        {
            Commander result = new Commander();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class WorldBoardPlayer
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.WorldBoardComponents.WorldBoardPlayer component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.WorldBoardComponents.WorldBoardPlayer component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.WorldBoardComponents.WorldBoardPlayer (WorldBoardPlayer thisType)
        {
            NamelessRogue.Engine.Components.WorldBoardComponents.WorldBoardPlayer result = new NamelessRogue.Engine.Components.WorldBoardComponents.WorldBoardPlayer();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator WorldBoardPlayer ( NamelessRogue.Engine.Components.WorldBoardComponents.WorldBoardPlayer  component)
        {
            WorldBoardPlayer result = new WorldBoardPlayer();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Unit
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public Int32 MaxNumberOfTroops { get; set; }
        [FlatBufferItem(3)]  public Int32 NumberOfTroops { get; set; }
        [FlatBufferItem(4)]  public Int32 AttackPower { get; set; }
        [FlatBufferItem(5)]  public Int32 Defence { get; set; }
        [FlatBufferItem(6)]  public Int32 SingleTrooperHp { get; set; }
        [FlatBufferItem(7)]  public Int32 AttackRange { get; set; }
        [FlatBufferItem(8)]  public Int32 MovementSpeed { get; set; }
        [FlatBufferItem(9)]  public Int32 SupplyCostPerTurn { get; set; }
        [FlatBufferItem(10)]  public Int32 ManaCostPerTurn { get; set; }
        [FlatBufferItem(11)]  public Int32 MaxHp { get; set; }
        [FlatBufferItem(12)]  public Int32 CurrentHp { get; set; }
        [FlatBufferItem(13)]  public MovementType MovementType { get; set; }
        [FlatBufferItem(14)]  public AttackType AttackType { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.WorldBoardComponents.Combat.Unit component)
        {
            this.MaxNumberOfTroops = component.MaxNumberOfTroops;
            this.NumberOfTroops = component.NumberOfTroops;
            this.AttackPower = component.AttackPower;
            this.Defence = component.Defence;
            this.SingleTrooperHp = component.SingleTrooperHp;
            this.AttackRange = component.AttackRange;
            this.MovementSpeed = component.MovementSpeed;
            this.SupplyCostPerTurn = component.SupplyCostPerTurn;
            this.ManaCostPerTurn = component.ManaCostPerTurn;
            this.MaxHp = component.MaxHp;
            this.CurrentHp = component.CurrentHp;
            this.MovementType = (MovementType)component.MovementType;
            this.AttackType = (AttackType)component.AttackType;
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.WorldBoardComponents.Combat.Unit component)
        {
            component.MaxNumberOfTroops = this.MaxNumberOfTroops;
            component.NumberOfTroops = this.NumberOfTroops;
            component.AttackPower = this.AttackPower;
            component.Defence = this.Defence;
            component.SingleTrooperHp = this.SingleTrooperHp;
            component.AttackRange = this.AttackRange;
            component.MovementSpeed = this.MovementSpeed;
            component.SupplyCostPerTurn = this.SupplyCostPerTurn;
            component.ManaCostPerTurn = this.ManaCostPerTurn;
            component.MaxHp = this.MaxHp;
            component.CurrentHp = this.CurrentHp;
            component.MovementType = (NamelessRogue.Engine.Components.WorldBoardComponents.Combat.MovementType)this.MovementType;
            component.AttackType = (NamelessRogue.Engine.Components.WorldBoardComponents.Combat.AttackType)this.AttackType;
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.WorldBoardComponents.Combat.Unit (Unit thisType)
        {
            NamelessRogue.Engine.Components.WorldBoardComponents.Combat.Unit result = new NamelessRogue.Engine.Components.WorldBoardComponents.Combat.Unit();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Unit ( NamelessRogue.Engine.Components.WorldBoardComponents.Combat.Unit  component)
        {
            Unit result = new Unit();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Description
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public String Name { get; set; }
        [FlatBufferItem(3)]  public String Text { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.UI.Description component)
        {
            this.Name = component.Name;
            this.Text = component.Text;
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.UI.Description component)
        {
            component.Name = this.Name;
            component.Text = this.Text;
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.UI.Description (Description thisType)
        {
            NamelessRogue.Engine.Components.UI.Description result = new NamelessRogue.Engine.Components.UI.Description();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Description ( NamelessRogue.Engine.Components.UI.Description  component)
        {
            Description result = new Description();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Damage
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public IEntity Source { get; set; }
        [FlatBufferItem(3)]  public Int32 DamageValue { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Status.Damage component)
        {
            this.Source = component.Source;
            this.DamageValue = component.DamageValue;
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Status.Damage component)
        {
            component.Source = this.Source;
            component.DamageValue = this.DamageValue;
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Status.Damage (Damage thisType)
        {
            NamelessRogue.Engine.Components.Status.Damage result = new NamelessRogue.Engine.Components.Status.Damage();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Damage ( NamelessRogue.Engine.Components.Status.Damage  component)
        {
            Damage result = new Damage();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Dead
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Status.Dead component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Status.Dead component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Status.Dead (Dead thisType)
        {
            NamelessRogue.Engine.Components.Status.Dead result = new NamelessRogue.Engine.Components.Status.Dead();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Dead ( NamelessRogue.Engine.Components.Status.Dead  component)
        {
            Dead result = new Dead();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Stats
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public SimpleStat Strength { get; set; }
        [FlatBufferItem(3)]  public SimpleStat Reflexes { get; set; }
        [FlatBufferItem(4)]  public SimpleStat Perception { get; set; }
        [FlatBufferItem(5)]  public SimpleStat Willpower { get; set; }
        [FlatBufferItem(6)]  public SimpleStat Wit { get; set; }
        [FlatBufferItem(7)]  public SimpleStat Imagination { get; set; }
        [FlatBufferItem(8)]  public SimpleStat Health { get; set; }
        [FlatBufferItem(9)]  public SimpleStat Stamina { get; set; }
        [FlatBufferItem(10)]  public SimpleStat Attack { get; set; }
        [FlatBufferItem(11)]  public SimpleStat Defence { get; set; }
        [FlatBufferItem(12)]  public SimpleStat AttackSpeed { get; set; }
        [FlatBufferItem(13)]  public SimpleStat MoveSpeed { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Stats.Stats component)
        {
            this.Strength = component.Strength;
            this.Reflexes = component.Reflexes;
            this.Perception = component.Perception;
            this.Willpower = component.Willpower;
            this.Wit = component.Wit;
            this.Imagination = component.Imagination;
            this.Health = component.Health;
            this.Stamina = component.Stamina;
            this.Attack = component.Attack;
            this.Defence = component.Defence;
            this.AttackSpeed = component.AttackSpeed;
            this.MoveSpeed = component.MoveSpeed;
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Stats.Stats component)
        {
            component.Strength = this.Strength;
            component.Reflexes = this.Reflexes;
            component.Perception = this.Perception;
            component.Willpower = this.Willpower;
            component.Wit = this.Wit;
            component.Imagination = this.Imagination;
            component.Health = this.Health;
            component.Stamina = this.Stamina;
            component.Attack = this.Attack;
            component.Defence = this.Defence;
            component.AttackSpeed = this.AttackSpeed;
            component.MoveSpeed = this.MoveSpeed;
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Stats.Stats (Stats thisType)
        {
            NamelessRogue.Engine.Components.Stats.Stats result = new NamelessRogue.Engine.Components.Stats.Stats();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Stats ( NamelessRogue.Engine.Components.Stats.Stats  component)
        {
            Stats result = new Stats();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class ConsoleCamera
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Rendering.ConsoleCamera component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Rendering.ConsoleCamera component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Rendering.ConsoleCamera (ConsoleCamera thisType)
        {
            NamelessRogue.Engine.Components.Rendering.ConsoleCamera result = new NamelessRogue.Engine.Components.Rendering.ConsoleCamera();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator ConsoleCamera ( NamelessRogue.Engine.Components.Rendering.ConsoleCamera  component)
        {
            ConsoleCamera result = new ConsoleCamera();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Drawable
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public Color BackgroundColor { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Rendering.Drawable component)
        {
            this.BackgroundColor = component.BackgroundColor;
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Rendering.Drawable component)
        {
            component.BackgroundColor = this.BackgroundColor;
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Rendering.Drawable (Drawable thisType)
        {
            NamelessRogue.Engine.Components.Rendering.Drawable result = new NamelessRogue.Engine.Components.Rendering.Drawable();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Drawable ( NamelessRogue.Engine.Components.Rendering.Drawable  component)
        {
            Drawable result = new Drawable();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class FollowedByCamera
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Rendering.FollowedByCamera component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Rendering.FollowedByCamera component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Rendering.FollowedByCamera (FollowedByCamera thisType)
        {
            NamelessRogue.Engine.Components.Rendering.FollowedByCamera result = new NamelessRogue.Engine.Components.Rendering.FollowedByCamera();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator FollowedByCamera ( NamelessRogue.Engine.Components.Rendering.FollowedByCamera  component)
        {
            FollowedByCamera result = new FollowedByCamera();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class LineToPlayer
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Rendering.LineToPlayer component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Rendering.LineToPlayer component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Rendering.LineToPlayer (LineToPlayer thisType)
        {
            NamelessRogue.Engine.Components.Rendering.LineToPlayer result = new NamelessRogue.Engine.Components.Rendering.LineToPlayer();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator LineToPlayer ( NamelessRogue.Engine.Components.Rendering.LineToPlayer  component)
        {
            LineToPlayer result = new LineToPlayer();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Screen
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public Int32 Width { get; set; }
        [FlatBufferItem(3)]  public Int32 Height { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Rendering.Screen component)
        {
            this.Width = component.Width;
            this.Height = component.Height;
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Rendering.Screen component)
        {
            component.Width = this.Width;
            component.Height = this.Height;
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Rendering.Screen (Screen thisType)
        {
            NamelessRogue.Engine.Components.Rendering.Screen result = new NamelessRogue.Engine.Components.Rendering.Screen();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Screen ( NamelessRogue.Engine.Components.Rendering.Screen  component)
        {
            Screen result = new Screen();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class BlocksVision
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Physical.BlocksVision component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Physical.BlocksVision component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Physical.BlocksVision (BlocksVision thisType)
        {
            NamelessRogue.Engine.Components.Physical.BlocksVision result = new NamelessRogue.Engine.Components.Physical.BlocksVision();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator BlocksVision ( NamelessRogue.Engine.Components.Physical.BlocksVision  component)
        {
            BlocksVision result = new BlocksVision();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class OccupiesTile
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Physical.OccupiesTile component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Physical.OccupiesTile component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Physical.OccupiesTile (OccupiesTile thisType)
        {
            NamelessRogue.Engine.Components.Physical.OccupiesTile result = new NamelessRogue.Engine.Components.Physical.OccupiesTile();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator OccupiesTile ( NamelessRogue.Engine.Components.Physical.OccupiesTile  component)
        {
            OccupiesTile result = new OccupiesTile();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Position
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Physical.Position component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Physical.Position component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Physical.Position (Position thisType)
        {
            NamelessRogue.Engine.Components.Physical.Position result = new NamelessRogue.Engine.Components.Physical.Position();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Position ( NamelessRogue.Engine.Components.Physical.Position  component)
        {
            Position result = new Position();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Ammo
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public AmmoType Type { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.ItemComponents.Ammo component)
        {
            this.Type = component.Type;
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.ItemComponents.Ammo component)
        {
            component.Type = this.Type;
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.ItemComponents.Ammo (Ammo thisType)
        {
            NamelessRogue.Engine.Components.ItemComponents.Ammo result = new NamelessRogue.Engine.Components.ItemComponents.Ammo();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Ammo ( NamelessRogue.Engine.Components.ItemComponents.Ammo  component)
        {
            Ammo result = new Ammo();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class AmmoLibrary
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public List<AmmoType> AmmoTypes { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.ItemComponents.AmmoLibrary component)
        {
            this.AmmoTypes = component.AmmoTypes;
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.ItemComponents.AmmoLibrary component)
        {
            component.AmmoTypes = this.AmmoTypes;
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.ItemComponents.AmmoLibrary (AmmoLibrary thisType)
        {
            NamelessRogue.Engine.Components.ItemComponents.AmmoLibrary result = new NamelessRogue.Engine.Components.ItemComponents.AmmoLibrary();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator AmmoLibrary ( NamelessRogue.Engine.Components.ItemComponents.AmmoLibrary  component)
        {
            AmmoLibrary result = new AmmoLibrary();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class ArmorStats
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public Int32 DefenceValue { get; set; }
        [FlatBufferItem(3)]  public Int32 EvasionValue { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.ItemComponents.ArmorStats component)
        {
            this.DefenceValue = component.DefenceValue;
            this.EvasionValue = component.EvasionValue;
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.ItemComponents.ArmorStats component)
        {
            component.DefenceValue = this.DefenceValue;
            component.EvasionValue = this.EvasionValue;
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.ItemComponents.ArmorStats (ArmorStats thisType)
        {
            NamelessRogue.Engine.Components.ItemComponents.ArmorStats result = new NamelessRogue.Engine.Components.ItemComponents.ArmorStats();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator ArmorStats ( NamelessRogue.Engine.Components.ItemComponents.ArmorStats  component)
        {
            ArmorStats result = new ArmorStats();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Consumable
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.ItemComponents.Consumable component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.ItemComponents.Consumable component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.ItemComponents.Consumable (Consumable thisType)
        {
            NamelessRogue.Engine.Components.ItemComponents.Consumable result = new NamelessRogue.Engine.Components.ItemComponents.Consumable();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Consumable ( NamelessRogue.Engine.Components.ItemComponents.Consumable  component)
        {
            Consumable result = new Consumable();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Equipment
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public List<List<Slot>> PossibleSlots { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.ItemComponents.Equipment component)
        {
            this.PossibleSlots = component.PossibleSlots;
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.ItemComponents.Equipment component)
        {
            component.PossibleSlots = this.PossibleSlots;
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.ItemComponents.Equipment (Equipment thisType)
        {
            NamelessRogue.Engine.Components.ItemComponents.Equipment result = new NamelessRogue.Engine.Components.ItemComponents.Equipment();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Equipment ( NamelessRogue.Engine.Components.ItemComponents.Equipment  component)
        {
            Equipment result = new Equipment();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class EquipmentSlots
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public List<Tuple<Slot,EquipmentSlot>> Slots { get; set; }
        [FlatBufferItem(3)]  public ItemsHolder Holder { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.ItemComponents.EquipmentSlots component)
        {
            this.Slots = component.Slots;
            this.Holder = component.Holder;
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.ItemComponents.EquipmentSlots component)
        {
            component.Slots = this.Slots;
            component.Holder = this.Holder;
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.ItemComponents.EquipmentSlots (EquipmentSlots thisType)
        {
            NamelessRogue.Engine.Components.ItemComponents.EquipmentSlots result = new NamelessRogue.Engine.Components.ItemComponents.EquipmentSlots();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator EquipmentSlots ( NamelessRogue.Engine.Components.ItemComponents.EquipmentSlots  component)
        {
            EquipmentSlots result = new EquipmentSlots();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Item
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public Single Weight { get; set; }
        [FlatBufferItem(3)]  public ItemType Type { get; set; }
        [FlatBufferItem(4)]  public ItemQuality Quality { get; set; }
        [FlatBufferItem(5)]  public Int32 Amount { get; set; }
        [FlatBufferItem(6)]  public Int32 Level { get; set; }
        [FlatBufferItem(7)]  public String Author { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.ItemComponents.Item component)
        {
            this.Weight = component.Weight;
            this.Type = (ItemType)component.Type;
            this.Quality = (ItemQuality)component.Quality;
            this.Amount = component.Amount;
            this.Level = component.Level;
            this.Author = component.Author;
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.ItemComponents.Item component)
        {
            component.Weight = this.Weight;
            component.Type = (NamelessRogue.Engine.Components.ItemComponents.ItemType)this.Type;
            component.Quality = (NamelessRogue.Engine.Components.ItemComponents.ItemQuality)this.Quality;
            component.Amount = this.Amount;
            component.Level = this.Level;
            component.Author = this.Author;
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.ItemComponents.Item (Item thisType)
        {
            NamelessRogue.Engine.Components.ItemComponents.Item result = new NamelessRogue.Engine.Components.ItemComponents.Item();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Item ( NamelessRogue.Engine.Components.ItemComponents.Item  component)
        {
            Item result = new Item();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class ItemsHolder
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public List<IEntity> Items { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.ItemComponents.ItemsHolder component)
        {
            this.Items = component.Items;
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.ItemComponents.ItemsHolder component)
        {
            component.Items = this.Items;
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.ItemComponents.ItemsHolder (ItemsHolder thisType)
        {
            NamelessRogue.Engine.Components.ItemComponents.ItemsHolder result = new NamelessRogue.Engine.Components.ItemComponents.ItemsHolder();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator ItemsHolder ( NamelessRogue.Engine.Components.ItemComponents.ItemsHolder  component)
        {
            ItemsHolder result = new ItemsHolder();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class WeaponStats
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public Int32 MinimumDamage { get; set; }
        [FlatBufferItem(3)]  public Int32 MaximumDamage { get; set; }
        [FlatBufferItem(4)]  public Int32 Range { get; set; }
        [FlatBufferItem(5)]  public AttackType AttackType { get; set; }
        [FlatBufferItem(6)]  public AmmoType AmmoType { get; set; }
        [FlatBufferItem(7)]  public Int32 AmmoInClip { get; set; }
        [FlatBufferItem(8)]  public Int32 CurrentAmmo { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.ItemComponents.WeaponStats component)
        {
            this.MinimumDamage = component.MinimumDamage;
            this.MaximumDamage = component.MaximumDamage;
            this.Range = component.Range;
            this.AttackType = (AttackType)component.AttackType;
            this.AmmoType = component.AmmoType;
            this.AmmoInClip = component.AmmoInClip;
            this.CurrentAmmo = component.CurrentAmmo;
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.ItemComponents.WeaponStats component)
        {
            component.MinimumDamage = this.MinimumDamage;
            component.MaximumDamage = this.MaximumDamage;
            component.Range = this.Range;
            component.AttackType = (NamelessRogue.Engine.Components.ItemComponents.AttackType)this.AttackType;
            component.AmmoType = this.AmmoType;
            component.AmmoInClip = this.AmmoInClip;
            component.CurrentAmmo = this.CurrentAmmo;
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.ItemComponents.WeaponStats (WeaponStats thisType)
        {
            NamelessRogue.Engine.Components.ItemComponents.WeaponStats result = new NamelessRogue.Engine.Components.ItemComponents.WeaponStats();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator WeaponStats ( NamelessRogue.Engine.Components.ItemComponents.WeaponStats  component)
        {
            WeaponStats result = new WeaponStats();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class ActionPoints
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public Int32 Points { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Interaction.ActionPoints component)
        {
            this.Points = component.Points;
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Interaction.ActionPoints component)
        {
            component.Points = this.Points;
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Interaction.ActionPoints (ActionPoints thisType)
        {
            NamelessRogue.Engine.Components.Interaction.ActionPoints result = new NamelessRogue.Engine.Components.Interaction.ActionPoints();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator ActionPoints ( NamelessRogue.Engine.Components.Interaction.ActionPoints  component)
        {
            ActionPoints result = new ActionPoints();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Buffs
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public List<Buff> Children { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Interaction.Buffs component)
        {
            this.Children = component.Children;
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Interaction.Buffs component)
        {
            component.Children = this.Children;
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Interaction.Buffs (Buffs thisType)
        {
            NamelessRogue.Engine.Components.Interaction.Buffs result = new NamelessRogue.Engine.Components.Interaction.Buffs();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Buffs ( NamelessRogue.Engine.Components.Interaction.Buffs  component)
        {
            Buffs result = new Buffs();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Cursor
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Interaction.Cursor component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Interaction.Cursor component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Interaction.Cursor (Cursor thisType)
        {
            NamelessRogue.Engine.Components.Interaction.Cursor result = new NamelessRogue.Engine.Components.Interaction.Cursor();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Cursor ( NamelessRogue.Engine.Components.Interaction.Cursor  component)
        {
            Cursor result = new Cursor();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class InputComponent
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Interaction.InputComponent component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Interaction.InputComponent component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Interaction.InputComponent (InputComponent thisType)
        {
            NamelessRogue.Engine.Components.Interaction.InputComponent result = new NamelessRogue.Engine.Components.Interaction.InputComponent();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator InputComponent ( NamelessRogue.Engine.Components.Interaction.InputComponent  component)
        {
            InputComponent result = new InputComponent();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class InputReceiver
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Interaction.InputReceiver component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Interaction.InputReceiver component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Interaction.InputReceiver (InputReceiver thisType)
        {
            NamelessRogue.Engine.Components.Interaction.InputReceiver result = new NamelessRogue.Engine.Components.Interaction.InputReceiver();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator InputReceiver ( NamelessRogue.Engine.Components.Interaction.InputReceiver  component)
        {
            InputReceiver result = new InputReceiver();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Movable
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Interaction.Movable component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Interaction.Movable component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Interaction.Movable (Movable thisType)
        {
            NamelessRogue.Engine.Components.Interaction.Movable result = new NamelessRogue.Engine.Components.Interaction.Movable();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Movable ( NamelessRogue.Engine.Components.Interaction.Movable  component)
        {
            Movable result = new Movable();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Player
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Interaction.Player component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Interaction.Player component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Interaction.Player (Player thisType)
        {
            NamelessRogue.Engine.Components.Interaction.Player result = new NamelessRogue.Engine.Components.Interaction.Player();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Player ( NamelessRogue.Engine.Components.Interaction.Player  component)
        {
            Player result = new Player();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class SimpleSwitch
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Interaction.SimpleSwitch component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Interaction.SimpleSwitch component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Interaction.SimpleSwitch (SimpleSwitch thisType)
        {
            NamelessRogue.Engine.Components.Interaction.SimpleSwitch result = new NamelessRogue.Engine.Components.Interaction.SimpleSwitch();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator SimpleSwitch ( NamelessRogue.Engine.Components.Interaction.SimpleSwitch  component)
        {
            SimpleSwitch result = new SimpleSwitch();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Building
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Environment.Building component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Environment.Building component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Environment.Building (Building thisType)
        {
            NamelessRogue.Engine.Components.Environment.Building result = new NamelessRogue.Engine.Components.Environment.Building();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Building ( NamelessRogue.Engine.Components.Environment.Building  component)
        {
            Building result = new Building();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Door
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Environment.Door component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Environment.Door component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Environment.Door (Door thisType)
        {
            NamelessRogue.Engine.Components.Environment.Door result = new NamelessRogue.Engine.Components.Environment.Door();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Door ( NamelessRogue.Engine.Components.Environment.Door  component)
        {
            Door result = new Door();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Fire
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Environment.Fire component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Environment.Fire component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Environment.Fire (Fire thisType)
        {
            NamelessRogue.Engine.Components.Environment.Fire result = new NamelessRogue.Engine.Components.Environment.Fire();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Fire ( NamelessRogue.Engine.Components.Environment.Fire  component)
        {
            Fire result = new Fire();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Furniture
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Environment.Furniture component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.Environment.Furniture component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.Environment.Furniture (Furniture thisType)
        {
            NamelessRogue.Engine.Components.Environment.Furniture result = new NamelessRogue.Engine.Components.Environment.Furniture();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Furniture ( NamelessRogue.Engine.Components.Environment.Furniture  component)
        {
            Furniture result = new Furniture();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class AIControlled
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.AI.NonPlayerCharacter.AIControlled component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.AI.NonPlayerCharacter.AIControlled component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.AI.NonPlayerCharacter.AIControlled (AIControlled thisType)
        {
            NamelessRogue.Engine.Components.AI.NonPlayerCharacter.AIControlled result = new NamelessRogue.Engine.Components.AI.NonPlayerCharacter.AIControlled();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator AIControlled ( NamelessRogue.Engine.Components.AI.NonPlayerCharacter.AIControlled  component)
        {
            AIControlled result = new AIControlled();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class BasicAi
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public Queue<Point> Route { get; set; }
        [FlatBufferItem(3)]  public BasicAiStates State { get; set; }
        [FlatBufferItem(4)]  public Point DestinationPoint { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAi component)
        {
            this.Route = component.Route;
            this.State = (BasicAiStates)component.State;
            this.DestinationPoint = component.DestinationPoint;
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAi component)
        {
            component.Route = this.Route;
            component.State = (NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAiStates)this.State;
            component.DestinationPoint = this.DestinationPoint;
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAi (BasicAi thisType)
        {
            NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAi result = new NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAi();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator BasicAi ( NamelessRogue.Engine.Components.AI.NonPlayerCharacter.BasicAi  component)
        {
            BasicAi result = new BasicAi();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Character
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.AI.NonPlayerCharacter.Character component)
        {
            this.Id = component.Id;
            this.ParentEntityId = component.ParentEntityId;
        }

        public void FillTo(NamelessRogue.Engine.Components.AI.NonPlayerCharacter.Character component)
        {
            component.Id = this.Id;
            component.ParentEntityId = this.ParentEntityId;

        }

        public static implicit operator NamelessRogue.Engine.Components.AI.NonPlayerCharacter.Character (Character thisType)
        {
            NamelessRogue.Engine.Components.AI.NonPlayerCharacter.Character result = new NamelessRogue.Engine.Components.AI.NonPlayerCharacter.Character();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Character ( NamelessRogue.Engine.Components.AI.NonPlayerCharacter.Character  component)
        {
            Character result = new Character();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class ChunkData
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public List<Chunk> RealityChunks { get; set; }
        [FlatBufferItem(3)]  public Int32 ChunkResolution { get; set; }
        [FlatBufferItem(4)]  public TimelineLayer WorldBoard { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.ChunksAndTiles.ChunkData component)
        {
            this.RealityChunks = component.RealityChunks;
            this.ChunkResolution = component.ChunkResolution;
            this.WorldBoard = component.WorldBoard;
        }

        public void FillTo(NamelessRogue.Engine.Components.ChunksAndTiles.ChunkData component)
        {
            component.RealityChunks = this.RealityChunks;
            component.ChunkResolution = this.ChunkResolution;
            component.WorldBoard = this.WorldBoard;

        }

        public static implicit operator NamelessRogue.Engine.Components.ChunksAndTiles.ChunkData (ChunkData thisType)
        {
            NamelessRogue.Engine.Components.ChunksAndTiles.ChunkData result = new NamelessRogue.Engine.Components.ChunksAndTiles.ChunkData();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator ChunkData ( NamelessRogue.Engine.Components.ChunksAndTiles.ChunkData  component)
        {
            ChunkData result = new ChunkData();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class IEntity
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Abstraction.IEntity component)
        {
        }

        public void FillTo(NamelessRogue.Engine.Abstraction.IEntity component)
        {

        }

        public static implicit operator NamelessRogue.Engine.Abstraction.IEntity (IEntity thisType)
        {
            NamelessRogue.Engine.Abstraction.IEntity result = new NamelessRogue.Engine.Abstraction.IEntity();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator IEntity ( NamelessRogue.Engine.Abstraction.IEntity  component)
        {
            IEntity result = new IEntity();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class SimpleStat
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public Int32 Value { get; set; }
        [FlatBufferItem(3)]  public Int32 MinValue { get; set; }
        [FlatBufferItem(4)]  public Int32 MaxValue { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Stats.SimpleStat component)
        {
            this.Value = component.Value;
            this.MinValue = component.MinValue;
            this.MaxValue = component.MaxValue;
        }

        public void FillTo(NamelessRogue.Engine.Components.Stats.SimpleStat component)
        {
            component.Value = this.Value;
            component.MinValue = this.MinValue;
            component.MaxValue = this.MaxValue;

        }

        public static implicit operator NamelessRogue.Engine.Components.Stats.SimpleStat (SimpleStat thisType)
        {
            NamelessRogue.Engine.Components.Stats.SimpleStat result = new NamelessRogue.Engine.Components.Stats.SimpleStat();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator SimpleStat ( NamelessRogue.Engine.Components.Stats.SimpleStat  component)
        {
            SimpleStat result = new SimpleStat();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class Color
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }

        public void FillFrom(NamelessRogue.Engine.Utility.Color component)
        {
        }

        public void FillTo(NamelessRogue.Engine.Utility.Color component)
        {

        }

        public static implicit operator NamelessRogue.Engine.Utility.Color (Color thisType)
        {
            NamelessRogue.Engine.Utility.Color result = new NamelessRogue.Engine.Utility.Color();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Color ( NamelessRogue.Engine.Utility.Color  component)
        {
            Color result = new Color();
            result.FillFrom(result);
            return result;
        }

    }
    [FlatBufferTable]
    public class AmmoType
    {
        [FlatBufferItem(0)]  public Guid Id { get; set; }
        [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public String Name { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.ItemComponents.AmmoType component)
        {
            this.Name = component.Name;
        }

        public void FillTo(NamelessRogue.Engine.Components.ItemComponents.AmmoType component)
        {
            component.Name = this.Name;

        }

        public static implicit operator NamelessRogue.Engine.Components.ItemComponents.AmmoType (AmmoType thisType)
        {
            NamelessRogue.Engine.Components.ItemComponents.AmmoType result = new NamelessRogue.Engine.Components.ItemComponents.AmmoType();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator AmmoType ( NamelessRogue.Engine.Components.ItemComponents.AmmoType  component)
        {
            AmmoType result = new AmmoType();
            result.FillFrom(result);
            return result;
        }

    }



    [FlatBufferEnum(typeof(Int32))]
    public enum MovementType : Int32 
    {
                Ground,
                Flying,
                Naval,
        }



    [FlatBufferEnum(typeof(Int32))]
    public enum AttackType : Int32 
    {
                Melee,
                Ranged,
        }



    [FlatBufferEnum(typeof(Int32))]
    public enum ItemType : Int32 
    {
                Weapon,
                Armor,
                Consumable,
                Food,
                Ammo,
                Misc,
        }



    [FlatBufferEnum(typeof(Int32))]
    public enum ItemQuality : Int32 
    {
                Terrible,
                Poor,
                Shoddy,
                Normal,
                Good,
                Excellent,
                Superb,
        }



    [FlatBufferEnum(typeof(Int32))]
    public enum AttackType : Int32 
    {
                Close,
                Ranged,
        }



    [FlatBufferEnum(typeof(Int32))]
    public enum BasicAiStates : Int32 
    {
                Idle,
                Attacking,
                Following,
                Moving,
        }



    [FlatBufferEnum(typeof(Int32))]
    public enum ItemType : Int32 
    {
                EntityRef,
                CharEntity,
                SurrogateCharEntity,
                Whitespace,
                String,
                StringChars,
                Raw,
                RawChars,
                ValueString,
        }



}