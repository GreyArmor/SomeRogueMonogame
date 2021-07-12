
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
    public class AmmoTypeStorage : IStorage<NamelessRogue.Engine.Components.ItemComponents.AmmoType>
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

        public static implicit operator NamelessRogue.Engine.Components.ItemComponents.AmmoType (AmmoTypeStorage thisType)
        {
            NamelessRogue.Engine.Components.ItemComponents.AmmoType result = new NamelessRogue.Engine.Components.ItemComponents.AmmoType();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator AmmoTypeStorage (NamelessRogue.Engine.Components.ItemComponents.AmmoType  component)
        {
            AmmoTypeStorage result = new AmmoTypeStorage();
            result.FillFrom(result);
            return result;
        }

    }

}