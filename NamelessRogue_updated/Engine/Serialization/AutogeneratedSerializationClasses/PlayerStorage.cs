
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
    public class PlayerStorage : IStorage<NamelessRogue.Engine.Components.Interaction.Player>
    {
            [FlatBufferItem(0)]  public Guid Id { get; set; }
            [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        public void FillFrom(NamelessRogue.Engine.Components.Interaction.Player component)
        {

            this.Id = component.Id;

            this.ParentEntityId = component.ParentEntityId;

        }

        public void FillTo(NamelessRogue.Engine.Components.Interaction.Player component)
        {

            component.Id = this.Id;

            component.ParentEntityId = this.ParentEntityId;


        }

        public static implicit operator NamelessRogue.Engine.Components.Interaction.Player (PlayerStorage thisType)
        {
            NamelessRogue.Engine.Components.Interaction.Player result = new NamelessRogue.Engine.Components.Interaction.Player();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator PlayerStorage (NamelessRogue.Engine.Components.Interaction.Player  component)
        {
            PlayerStorage result = new PlayerStorage();
            result.FillFrom(result);
            return result;
        }

    }

}