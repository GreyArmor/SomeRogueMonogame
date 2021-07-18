
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
    public class BlocksVisionStorage : IStorage<NamelessRogue.Engine.Components.Physical.BlocksVision>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        public void FillFrom(NamelessRogue.Engine.Components.Physical.BlocksVision component)
        {

            this.Id = component.Id.ToString();

            this.ParentEntityId = component.ParentEntityId.ToString();

        }

        public void FillTo(NamelessRogue.Engine.Components.Physical.BlocksVision component)
        {

            component.Id = new Guid(this.Id);

            component.ParentEntityId = new Guid(this.ParentEntityId);


        }

        public static implicit operator NamelessRogue.Engine.Components.Physical.BlocksVision (BlocksVisionStorage thisType)
        {
            NamelessRogue.Engine.Components.Physical.BlocksVision result = new NamelessRogue.Engine.Components.Physical.BlocksVision();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator BlocksVisionStorage (NamelessRogue.Engine.Components.Physical.BlocksVision  component)
        {
            BlocksVisionStorage result = new BlocksVisionStorage();
            result.FillFrom(result);
            return result;
        }

    }

}