
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
    public class ItemsHolderStorage : IStorage<NamelessRogue.Engine.Components.ItemComponents.ItemsHolder>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public IList<EntityStorage> Items { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.ItemComponents.ItemsHolder component)
        {
            
            this.Items = new List<EntityStorage>(component.Items.Select(x=>(EntityStorage)x));

            this.Id = component.Id.ToString();

            this.ParentEntityId = component.ParentEntityId.ToString();

        }

        public void FillTo(NamelessRogue.Engine.Components.ItemComponents.ItemsHolder component)
        {

            component.Items = new List<NamelessRogue.Engine.Abstraction.IEntity>(this.Items.Select(x=>(NamelessRogue.Engine.Abstraction.IEntity)x));

            component.Id = new Guid(this.Id);

            component.ParentEntityId = new Guid(this.ParentEntityId);


        }

        public static implicit operator NamelessRogue.Engine.Components.ItemComponents.ItemsHolder (ItemsHolderStorage thisType)
        {
            NamelessRogue.Engine.Components.ItemComponents.ItemsHolder result = new NamelessRogue.Engine.Components.ItemComponents.ItemsHolder();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator ItemsHolderStorage (NamelessRogue.Engine.Components.ItemComponents.ItemsHolder  component)
        {
            ItemsHolderStorage result = new ItemsHolderStorage();
            result.FillFrom(result);
            return result;
        }

    }

}