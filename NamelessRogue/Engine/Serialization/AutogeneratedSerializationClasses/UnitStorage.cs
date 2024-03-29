
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
    public class UnitStorage : IStorage<NamelessRogue.Engine.Components.WorldBoardComponents.Combat.Unit>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public Int32 MaxNumberOfTroops { get; set; }

        [FlatBufferItem(3)]  public Int32 NumberOfTroops { get; set; }

        [FlatBufferItem(4)]  public Int32 AttackPower { get; set; }

        [FlatBufferItem(5)]  public Int32 Defence { get; set; }

        [FlatBufferItem(6)]  public Int32 SingleTrooperHp { get; set; }

        [FlatBufferItem(7)]  public Int32 AttackRange { get; set; }

        [FlatBufferItem(8)]  public Int32 MovementSpeed { get; set; }

        [FlatBufferItem(9)]  public Int32 SupplyCostPerTurn { get; set; }

        [FlatBufferItem(10)]  public Int32 ManaCostPerTurn { get; set; }

        [FlatBufferItem(11)]  public Int32 CurrentHp { get; set; }

        [FlatBufferItem(12)]  public MovementTypeStorage MovementType { get; set; }

        [FlatBufferItem(13)]  public UnitAttackTypeStorage AttackType { get; set; }

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

            this.CurrentHp = component.CurrentHp;

            this.MovementType = (MovementTypeStorage)component.MovementType;

            this.AttackType = (UnitAttackTypeStorage)component.AttackType;

            this.Id = component.Id == null ? null : component.Id.ToString();

            this.ParentEntityId = component.ParentEntityId == null ? null : component.ParentEntityId.ToString();

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

            component.CurrentHp = this.CurrentHp;

            component.MovementType = (NamelessRogue.Engine.Components.WorldBoardComponents.Combat.MovementType)this.MovementType;

            component.AttackType = (NamelessRogue.Engine.Components.WorldBoardComponents.Combat.UnitAttackType)this.AttackType;

            component.Id = new Guid(this.Id);

            component.ParentEntityId = new Guid(this.ParentEntityId);


        }

        public static implicit operator NamelessRogue.Engine.Components.WorldBoardComponents.Combat.Unit (UnitStorage thisType)
        {
            if(thisType == null) { return null; }
            NamelessRogue.Engine.Components.WorldBoardComponents.Combat.Unit result = new NamelessRogue.Engine.Components.WorldBoardComponents.Combat.Unit();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator UnitStorage (NamelessRogue.Engine.Components.WorldBoardComponents.Combat.Unit  component)
        {
            if(component == null) { return null; }
            UnitStorage result = new UnitStorage();
            result.FillFrom(component);
            return result;
        }

    }

}