
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
    public class ConcreteSettlementStorage : IStorage<NamelessRogue.Engine.Generation.World.ConcreteSettlement>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public PointStorage Center { get; set; }

        public void FillFrom(NamelessRogue.Engine.Generation.World.ConcreteSettlement component)
        {

            this.Center = component.Center;

        }

        public void FillTo(NamelessRogue.Engine.Generation.World.ConcreteSettlement component)
        {

            component.Center = this.Center;


        }

        public static implicit operator NamelessRogue.Engine.Generation.World.ConcreteSettlement (ConcreteSettlementStorage thisType)
        {
            NamelessRogue.Engine.Generation.World.ConcreteSettlement result = new NamelessRogue.Engine.Generation.World.ConcreteSettlement();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator ConcreteSettlementStorage (NamelessRogue.Engine.Generation.World.ConcreteSettlement  component)
        {
            ConcreteSettlementStorage result = new ConcreteSettlementStorage();
            result.FillFrom(result);
            return result;
        }

    }

}