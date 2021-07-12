
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
    public class RelationsModifierStorage : IStorage<NamelessRogue.Engine.Generation.World.Diplomacy.RelationsModifier>
    {
            [FlatBufferItem(0)]  public Guid Id { get; set; }
            [FlatBufferItem(1)]  public Guid ParentEntityId { get; set; }
        public void FillFrom(NamelessRogue.Engine.Generation.World.Diplomacy.RelationsModifier component)
        {

        }

        public void FillTo(NamelessRogue.Engine.Generation.World.Diplomacy.RelationsModifier component)
        {


        }

        public static implicit operator NamelessRogue.Engine.Generation.World.Diplomacy.RelationsModifier (RelationsModifierStorage thisType)
        {
            NamelessRogue.Engine.Generation.World.Diplomacy.RelationsModifier result = new NamelessRogue.Engine.Generation.World.Diplomacy.RelationsModifier();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator RelationsModifierStorage (NamelessRogue.Engine.Generation.World.Diplomacy.RelationsModifier  component)
        {
            RelationsModifierStorage result = new RelationsModifierStorage();
            result.FillFrom(result);
            return result;
        }

    }

}