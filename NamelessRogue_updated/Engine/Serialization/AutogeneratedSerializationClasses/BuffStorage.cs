
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
    public class BuffStorage : IStorage<NamelessRogue.Engine.Components.Interaction.Buff>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public Int32 DurationInTurns { get; set; }

        [FlatBufferItem(3)]  public IList<ModifierTypeStorage> Modifiers { get; set; }

        public void FillFrom(NamelessRogue.Engine.Components.Interaction.Buff component)
        {

            this.DurationInTurns = component.DurationInTurns;
            
            this.Modifiers = new List<ModifierTypeStorage>(component.Modifiers.Select(x=>(ModifierTypeStorage)x));

        }

        public void FillTo(NamelessRogue.Engine.Components.Interaction.Buff component)
        {

            component.DurationInTurns = this.DurationInTurns;

            component.Modifiers = new List<NamelessRogue.Engine.Components.Interaction.ModifierType>(this.Modifiers.Select(x=>(NamelessRogue.Engine.Components.Interaction.ModifierType)x));


        }

        public static implicit operator NamelessRogue.Engine.Components.Interaction.Buff (BuffStorage thisType)
        {
            if(thisType == null) { return null; }
            NamelessRogue.Engine.Components.Interaction.Buff result = new NamelessRogue.Engine.Components.Interaction.Buff();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator BuffStorage (NamelessRogue.Engine.Components.Interaction.Buff  component)
        {
            if(component == null) { return null; }
            BuffStorage result = new BuffStorage();
            result.FillFrom(component);
            return result;
        }

    }

}