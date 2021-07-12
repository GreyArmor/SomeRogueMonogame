
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
    public class ActionPointsStorage : IStorage<NamelessRogue.Engine.Components.Interaction.ActionPoints>
    {
            [FlatBufferItem(0)]  public Guid Id { get; set; }
            [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public Int32 Points { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Interaction.ActionPoints component)
        {

            this.Points = component.Points;

            this.Id = component.Id;

            this.ParentEntityId = component.ParentEntityId;

        }

        public void FillTo(NamelessRogue.Engine.Components.Interaction.ActionPoints component)
        {

            component.Points = this.Points;

            component.Id = this.Id;

            component.ParentEntityId = this.ParentEntityId;


        }

        public static implicit operator NamelessRogue.Engine.Components.Interaction.ActionPoints (ActionPointsStorage thisType)
        {
            NamelessRogue.Engine.Components.Interaction.ActionPoints result = new NamelessRogue.Engine.Components.Interaction.ActionPoints();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator ActionPointsStorage (NamelessRogue.Engine.Components.Interaction.ActionPoints  component)
        {
            ActionPointsStorage result = new ActionPointsStorage();
            result.FillFrom(result);
            return result;
        }

    }

}