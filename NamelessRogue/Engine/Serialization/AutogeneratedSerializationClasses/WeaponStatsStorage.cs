
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
    public class WeaponStatsStorage : IStorage<NamelessRogue.Engine.Components.ItemComponents.WeaponStats>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public Int32 MinimumDamage { get; set; }

        [FlatBufferItem(3)]  public Int32 MaximumDamage { get; set; }

        [FlatBufferItem(4)]  public Int32 Range { get; set; }

        [FlatBufferItem(5)]  public AttackTypeStorage AttackType { get; set; }

        [FlatBufferItem(6)]  public AmmoTypeStorage AmmoType { get; set; }

        [FlatBufferItem(7)]  public Int32 AmmoInClip { get; set; }

        [FlatBufferItem(8)]  public Int32 CurrentAmmo { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.ItemComponents.WeaponStats component)
        {

            this.MinimumDamage = component.MinimumDamage;

            this.MaximumDamage = component.MaximumDamage;

            this.Range = component.Range;

            this.AttackType = (AttackTypeStorage)component.AttackType;

            this.AmmoType = component.AmmoType;

            this.AmmoInClip = component.AmmoInClip;

            this.CurrentAmmo = component.CurrentAmmo;

            this.Id = component.Id == null ? null : component.Id.ToString();

            this.ParentEntityId = component.ParentEntityId == null ? null : component.ParentEntityId.ToString();

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

            component.Id = new Guid(this.Id);

            component.ParentEntityId = new Guid(this.ParentEntityId);


        }

        public static implicit operator NamelessRogue.Engine.Components.ItemComponents.WeaponStats (WeaponStatsStorage thisType)
        {
            if(thisType == null) { return null; }
            NamelessRogue.Engine.Components.ItemComponents.WeaponStats result = new NamelessRogue.Engine.Components.ItemComponents.WeaponStats();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator WeaponStatsStorage (NamelessRogue.Engine.Components.ItemComponents.WeaponStats  component)
        {
            if(component == null) { return null; }
            WeaponStatsStorage result = new WeaponStatsStorage();
            result.FillFrom(component);
            return result;
        }

    }

}