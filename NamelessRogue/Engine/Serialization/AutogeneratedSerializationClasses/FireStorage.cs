
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
    public class FireStorage : IStorage<NamelessRogue.Engine.Components.Environment.Fire>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        public void FillFrom(NamelessRogue.Engine.Components.Environment.Fire component)
        {

            this.Id = component.Id == null ? null : component.Id.ToString();

            this.ParentEntityId = component.ParentEntityId == null ? null : component.ParentEntityId.ToString();

        }

        public void FillTo(NamelessRogue.Engine.Components.Environment.Fire component)
        {

            component.Id = new Guid(this.Id);

            component.ParentEntityId = new Guid(this.ParentEntityId);


        }

        public static implicit operator NamelessRogue.Engine.Components.Environment.Fire (FireStorage thisType)
        {
            if(thisType == null) { return null; }
            NamelessRogue.Engine.Components.Environment.Fire result = new NamelessRogue.Engine.Components.Environment.Fire();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator FireStorage (NamelessRogue.Engine.Components.Environment.Fire  component)
        {
            if(component == null) { return null; }
            FireStorage result = new FireStorage();
            result.FillFrom(component);
            return result;
        }

    }

}