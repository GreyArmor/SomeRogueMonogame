
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
    public class ConsumableStorage : IStorage<NamelessRogue.Engine.Components.ItemComponents.Consumable>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        public void FillFrom(NamelessRogue.Engine.Components.ItemComponents.Consumable component)
        {

            this.Id = component.Id == null ? null : component.Id.ToString();

            this.ParentEntityId = component.ParentEntityId == null ? null : component.ParentEntityId.ToString();

        }

        public void FillTo(NamelessRogue.Engine.Components.ItemComponents.Consumable component)
        {

            component.Id = new Guid(this.Id);

            component.ParentEntityId = new Guid(this.ParentEntityId);


        }

        public static implicit operator NamelessRogue.Engine.Components.ItemComponents.Consumable (ConsumableStorage thisType)
        {
            if(thisType == null) { return null; }
            NamelessRogue.Engine.Components.ItemComponents.Consumable result = new NamelessRogue.Engine.Components.ItemComponents.Consumable();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator ConsumableStorage (NamelessRogue.Engine.Components.ItemComponents.Consumable  component)
        {
            if(component == null) { return null; }
            ConsumableStorage result = new ConsumableStorage();
            result.FillFrom(component);
            return result;
        }

    }

}