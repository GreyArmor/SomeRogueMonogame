
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
    public class FollowedByCameraStorage : IStorage<NamelessRogue.Engine.Components.Rendering.FollowedByCamera>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        public void FillFrom(NamelessRogue.Engine.Components.Rendering.FollowedByCamera component)
        {

            this.Id = component.Id.ToString();

            this.ParentEntityId = component.ParentEntityId.ToString();

        }

        public void FillTo(NamelessRogue.Engine.Components.Rendering.FollowedByCamera component)
        {

            component.Id = new Guid(this.Id);

            component.ParentEntityId = new Guid(this.ParentEntityId);


        }

        public static implicit operator NamelessRogue.Engine.Components.Rendering.FollowedByCamera (FollowedByCameraStorage thisType)
        {
            NamelessRogue.Engine.Components.Rendering.FollowedByCamera result = new NamelessRogue.Engine.Components.Rendering.FollowedByCamera();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator FollowedByCameraStorage (NamelessRogue.Engine.Components.Rendering.FollowedByCamera  component)
        {
            FollowedByCameraStorage result = new FollowedByCameraStorage();
            result.FillFrom(result);
            return result;
        }

    }

}