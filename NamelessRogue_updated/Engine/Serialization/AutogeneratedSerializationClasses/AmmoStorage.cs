
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
    public class AmmoStorage : IStorage<NamelessRogue.Engine.Components.ItemComponents.Ammo>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public AmmoTypeStorage Type { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.ItemComponents.Ammo component)
        {

            this.Type = component.Type;

            this.Id = component.Id.ToString();

            this.ParentEntityId = component.ParentEntityId.ToString();

        }

        public void FillTo(NamelessRogue.Engine.Components.ItemComponents.Ammo component)
        {

            component.Type = this.Type;

            component.Id = new Guid(this.Id);

            component.ParentEntityId = new Guid(this.ParentEntityId);


        }

        public static implicit operator NamelessRogue.Engine.Components.ItemComponents.Ammo (AmmoStorage thisType)
        {
            NamelessRogue.Engine.Components.ItemComponents.Ammo result = new NamelessRogue.Engine.Components.ItemComponents.Ammo();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator AmmoStorage (NamelessRogue.Engine.Components.ItemComponents.Ammo  component)
        {
            AmmoStorage result = new AmmoStorage();
            result.FillFrom(result);
            return result;
        }

    }

}