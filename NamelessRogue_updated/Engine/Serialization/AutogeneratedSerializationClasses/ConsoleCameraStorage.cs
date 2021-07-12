
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
    public class ConsoleCameraStorage : IStorage<NamelessRogue.Engine.Components.Rendering.ConsoleCamera>
    {
            [FlatBufferItem(0)]  public Guid Id { get; set; }
            [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        public void FillFrom(NamelessRogue.Engine.Components.Rendering.ConsoleCamera component)
        {

            this.Id = component.Id;

            this.ParentEntityId = component.ParentEntityId;

        }

        public void FillTo(NamelessRogue.Engine.Components.Rendering.ConsoleCamera component)
        {

            component.Id = this.Id;

            component.ParentEntityId = this.ParentEntityId;


        }

        public static implicit operator NamelessRogue.Engine.Components.Rendering.ConsoleCamera (ConsoleCameraStorage thisType)
        {
            NamelessRogue.Engine.Components.Rendering.ConsoleCamera result = new NamelessRogue.Engine.Components.Rendering.ConsoleCamera();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator ConsoleCameraStorage (NamelessRogue.Engine.Components.Rendering.ConsoleCamera  component)
        {
            ConsoleCameraStorage result = new ConsoleCameraStorage();
            result.FillFrom(result);
            return result;
        }

    }

}