
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
    public class PositionStorage : IStorage<NamelessRogue.Engine.Components.Physical.Position>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public PointStorage Point { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Physical.Position component)
        {

            this.Point = component.Point;

            this.Id = component.Id == null ? null : component.Id.ToString();

            this.ParentEntityId = component.ParentEntityId == null ? null : component.ParentEntityId.ToString();

        }

        public void FillTo(NamelessRogue.Engine.Components.Physical.Position component)
        {

            component.Point = this.Point;

            component.Id = new Guid(this.Id);

            component.ParentEntityId = new Guid(this.ParentEntityId);


        }

        public static implicit operator NamelessRogue.Engine.Components.Physical.Position (PositionStorage thisType)
        {
            if(thisType == null) { return null; }
            NamelessRogue.Engine.Components.Physical.Position result = new NamelessRogue.Engine.Components.Physical.Position();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator PositionStorage (NamelessRogue.Engine.Components.Physical.Position  component)
        {
            if(component == null) { return null; }
            PositionStorage result = new PositionStorage();
            result.FillFrom(component);
            return result;
        }

    }

}