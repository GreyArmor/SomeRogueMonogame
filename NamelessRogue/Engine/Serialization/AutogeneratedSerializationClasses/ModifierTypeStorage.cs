
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
    public class ModifierTypeStorage : IStorage<NamelessRogue.Engine.Components.Interaction.ModifierType>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        public void FillFrom(NamelessRogue.Engine.Components.Interaction.ModifierType component)
        {

        }

        public void FillTo(NamelessRogue.Engine.Components.Interaction.ModifierType component)
        {


        }

        public static implicit operator NamelessRogue.Engine.Components.Interaction.ModifierType (ModifierTypeStorage thisType)
        {
            if(thisType == null) { return default; }
            NamelessRogue.Engine.Components.Interaction.ModifierType result = new NamelessRogue.Engine.Components.Interaction.ModifierType();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator ModifierTypeStorage (NamelessRogue.Engine.Components.Interaction.ModifierType  component)
        {
            if(component == null) { return null; }
            ModifierTypeStorage result = new ModifierTypeStorage();
            result.FillFrom(component);
            return result;
        }

    }

}