
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
    public class FurnitureStorage : IStorage<NamelessRogue.Engine.Components.Environment.Furniture>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        public void FillFrom(NamelessRogue.Engine.Components.Environment.Furniture component)
        {

            this.Id = component.Id.ToString();

            this.ParentEntityId = component.ParentEntityId.ToString();

        }

        public void FillTo(NamelessRogue.Engine.Components.Environment.Furniture component)
        {

            component.Id = new Guid(this.Id);

            component.ParentEntityId = new Guid(this.ParentEntityId);


        }

        public static implicit operator NamelessRogue.Engine.Components.Environment.Furniture (FurnitureStorage thisType)
        {
            NamelessRogue.Engine.Components.Environment.Furniture result = new NamelessRogue.Engine.Components.Environment.Furniture();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator FurnitureStorage (NamelessRogue.Engine.Components.Environment.Furniture  component)
        {
            FurnitureStorage result = new FurnitureStorage();
            result.FillFrom(result);
            return result;
        }

    }

}