
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
    public class ResorcesStorage : IStorage<NamelessRogue.Engine.Generation.World.Meta.Resorces>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public Int32 Population { get; set; }

        [FlatBufferItem(3)]  public Int32 Wealth { get; set; }

        [FlatBufferItem(4)]  public Int32 Supply { get; set; }

        [FlatBufferItem(5)]  public Int32 Mana { get; set; }

        [FlatBufferItem(6)]  public Int32 Influence { get; set; }

        public void FillFrom(NamelessRogue.Engine.Generation.World.Meta.Resorces component)
        {

            this.Population = component.Population;

            this.Wealth = component.Wealth;

            this.Supply = component.Supply;

            this.Mana = component.Mana;

            this.Influence = component.Influence;

        }

        public void FillTo(NamelessRogue.Engine.Generation.World.Meta.Resorces component)
        {

            component.Population = this.Population;

            component.Wealth = this.Wealth;

            component.Supply = this.Supply;

            component.Mana = this.Mana;

            component.Influence = this.Influence;


        }

        public static implicit operator NamelessRogue.Engine.Generation.World.Meta.Resorces (ResorcesStorage thisType)
        {
            if(thisType == null) { return null; }
            NamelessRogue.Engine.Generation.World.Meta.Resorces result = new NamelessRogue.Engine.Generation.World.Meta.Resorces();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator ResorcesStorage (NamelessRogue.Engine.Generation.World.Meta.Resorces  component)
        {
            if(component == null) { return null; }
            ResorcesStorage result = new ResorcesStorage();
            result.FillFrom(component);
            return result;
        }

    }

}