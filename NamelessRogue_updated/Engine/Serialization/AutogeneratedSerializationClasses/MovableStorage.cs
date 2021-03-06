
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
    public class MovableStorage : IStorage<NamelessRogue.Engine.Components.Interaction.Movable>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        public void FillFrom(NamelessRogue.Engine.Components.Interaction.Movable component)
        {

            this.Id = component.Id.ToString();

            this.ParentEntityId = component.ParentEntityId.ToString();

        }

        public void FillTo(NamelessRogue.Engine.Components.Interaction.Movable component)
        {

            component.Id = new Guid(this.Id);

            component.ParentEntityId = new Guid(this.ParentEntityId);


        }

        public static implicit operator NamelessRogue.Engine.Components.Interaction.Movable (MovableStorage thisType)
        {
            NamelessRogue.Engine.Components.Interaction.Movable result = new NamelessRogue.Engine.Components.Interaction.Movable();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator MovableStorage (NamelessRogue.Engine.Components.Interaction.Movable  component)
        {
            MovableStorage result = new MovableStorage();
            result.FillFrom(result);
            return result;
        }

    }

}